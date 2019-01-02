using System;
using System.Threading.Tasks;
using AsbMessages;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence;
using NServiceBus.Persistence.NHibernate;
using SqlMessages;

namespace SqlAsbBridge.SqlHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            Console.Title = "ServiceBusRouter.SqlHandler";
            Console.WriteLine($"-- {Console.Title} --");

            // Enable debug logging
            var defaultFactory = LogManager.Use<DefaultFactory>();
            defaultFactory.Level(LogLevel.Debug);

            // Create endpoint config
            var endpointConfiguration = new EndpointConfiguration("SqlHandler");
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.AutoSubscribe();

            // Configure persistence for NServiceBus config (subscriptions etc.)
            var persistence = endpointConfiguration.UsePersistence<NHibernatePersistence>();
            persistence.ConnectionString(Constants.SqlPersistenceConnectionString);
            endpointConfiguration.UsePersistence<NHibernatePersistence, StorageType.Subscriptions>().EnableCachingForSubscriptionStorage(TimeSpan.FromMinutes(5));

            // Use SQL server transport:
            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(Constants.SqlTransportConnectionString);
            transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);

            // Define routing rules:
            var routing = transport.Routing();
            routing.RegisterPublisher(typeof(SqlActionCompletedEvent).Assembly, "SqlPublisher");
            //routing.RegisterPublisher(typeof(SqlActionCompletedEvent), "SqlPublisher");

            // Add routing logic to ASB:
            var router = routing.ConnectToRouter("PoCRouter");
            router.RegisterPublisher(typeof(AzureActionCompletedEvent), publisherEndpointName: "AsbPublisher");

            // Start Service Bus:
            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();

            await endpointInstance.Stop().ConfigureAwait(false);
        }
    }
}
