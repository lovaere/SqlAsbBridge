using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence;
using NServiceBus.Persistence.NHibernate;
using SqlMessages;

namespace SqlAsbBridge.SqlPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            Console.Title = "ServiceBusRouter.SqlPublisher";
            Console.WriteLine($"-- {Console.Title} --");

            // Enable debug logging
            var defaultFactory = LogManager.Use<DefaultFactory>();
            defaultFactory.Level(LogLevel.Debug);

            // Create endpoint config
            var endpointConfiguration = new EndpointConfiguration("SqlPublisher");
            endpointConfiguration.EnableInstallers();

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
            routing.RegisterPublisher(
                assembly: typeof(SqlActionCompletedEvent).Assembly,
                publisherEndpoint: "SqlPublisher");
            routing.RouteToEndpoint(
                assembly: typeof(StartSqlActionCommand).Assembly,
                destination: "SqlHandler");

            // Add routing logic to ASB:
            var router = routing.ConnectToRouter("PoCRouter");
            router.RegisterPublisher(typeof(SqlActionCompletedEvent), publisherEndpointName: "SqlPublisher");

            // Start Service Bus:
            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            var random = new Random();
            var completedEvent = new SqlActionCompletedEvent(random.Next(1000).ToString());
            await endpointInstance.Publish(completedEvent);

            var startCommand = new StartSqlActionCommand(random.Next(1000).ToString());
            await endpointInstance.Send(startCommand);

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();

            await endpointInstance.Stop().ConfigureAwait(false);
        }
    }
}
