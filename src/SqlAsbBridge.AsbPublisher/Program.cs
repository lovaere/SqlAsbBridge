using System;
using System.Threading.Tasks;
using AsbMessages;
using NServiceBus;
using NServiceBus.Logging;

namespace SqlAsbBridge.AsbPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            Console.Title = "ServiceBusRouter.AsbPublisher";
            Console.WriteLine($"-- {Console.Title} --");

            // Enable debug logging
            var defaultFactory = LogManager.Use<DefaultFactory>();
            defaultFactory.Level(LogLevel.Debug);

            // Create endpoint config
            var endpointConfiguration = new EndpointConfiguration("AsbPublisher");
            endpointConfiguration.EnableInstallers();

            // Use ASB transport:
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(Constants.AsbConnectionString);
            transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);

            // Define routing rules:
            var routing = transport.Routing();
            routing.RouteToEndpoint(
                assembly: typeof(StartAzureActionCommand).Assembly,
                destination: "AsbHandler");

            // Add routing logic to ASB:
            var router = routing.ConnectToRouter("PoCRouter");
            router.RegisterPublisher(typeof(AzureActionCompletedEvent), publisherEndpointName: "AsbPublisher");

            // Start Service Bus:
            var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            var random = new Random();
            var completedEvent = new AzureActionCompletedEvent(random.Next(1000).ToString());
            await endpointInstance.Publish(completedEvent);

            var startCommand = new StartAzureActionCommand(random.Next(1000).ToString());
            await endpointInstance.Send(startCommand);

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();

            await endpointInstance.Stop().ConfigureAwait(false);
        }
    }
}
