using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Router;

namespace SqlAsbBridge.Router
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            Console.Title = "Router";
            Console.WriteLine($"-- {Console.Title} --");

            var defaultFactory = LogManager.Use<DefaultFactory>();
            defaultFactory.Level(LogLevel.Debug);

            // Configure router:
            var routerConfiguration = new RouterConfiguration("PoCRouter");
            routerConfiguration.AutoCreateQueues();
            /*var asbInterface = */routerConfiguration.AddInterface<AzureServiceBusTransport>(
                "ASB",
                transportExtensions =>
                {
                    transportExtensions.ConnectionString(Constants.AsbConnectionString);
                    transportExtensions.Transactions(TransportTransactionMode.ReceiveOnly);
                }
            );

            var sqlInterface = routerConfiguration.AddInterface<SqlServerTransport>(
                "SQLServer",
                transportExtensions =>
                {
                    transportExtensions.ConnectionString(Constants.SqlTransportConnectionString);
                    transportExtensions.Transactions(TransportTransactionMode.SendsAtomicWithReceive);
                }
            );
            sqlInterface.UseSubscriptionPersistence(new SqlSubscriptionStorage(
                () => new SqlConnection(Constants.SqlPersistenceConnectionString),
                null, new SqlDialect.MsSqlServer(), null));

            // Configure routing protocol:
            var staticRouting = routerConfiguration.UseStaticRoutingProtocol();
            //Forward all messages from ASB to SQLServer
            staticRouting.AddForwardRoute(
                incomingInterface: "ASB",
                outgoingInterface: "SQLServer");

            //Forward all messages from SQLServer to ASB
            staticRouting.AddForwardRoute(
                incomingInterface: "SQLServer",
                outgoingInterface: "ASB");

            // Start router:
            var router = NServiceBus.Router.Router.Create(routerConfiguration);

            await router.Start().ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();

            await router.Stop().ConfigureAwait(false);
        }
    }
}
