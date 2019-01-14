using System;
using System.Threading.Tasks;
using AsbMessages;
using NServiceBus;
using NServiceBus.Logging;
using SqlMessages;

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

            Console.Write("Next action (enter to exit): ");
            var keyInfo = Console.ReadKey();
            Console.WriteLine();

            while (keyInfo.Key != ConsoleKey.Enter)
            {
                if (keyInfo.Key == ConsoleKey.C) // C = new command
                {
                    var random = new Random();
                    var index = random.Next(1000).ToString();
                    var startCommand = new StartAzureActionCommand(index);
                    await endpointInstance.Send(startCommand);
                    Console.WriteLine($"-> Send StartAzureActionCommand {index}");
                }
                else if (keyInfo.Key == ConsoleKey.E) // E = new event
                {
                    var random = new Random();
                    var index = random.Next(1000).ToString();
                    var completedEvent = new AzureActionCompletedEvent(index);
                    await endpointInstance.Publish(completedEvent);
                    Console.WriteLine($"-> Send AzureActionCompletedEvent {index}");
                }
                else if (keyInfo.Key == ConsoleKey.S) // S = new SQL command
                {
                    var random = new Random();
                    var index = random.Next(1000).ToString();
                    var startCommand = new StartSqlActionCommand(index);
                    await endpointInstance.Send(startCommand);
                    Console.WriteLine($"-> Send StartSqlActionCommand {index}");
                }

                Console.Write("Next action (enter to exit): ");
                keyInfo = Console.ReadKey();
                Console.WriteLine();
            }

            await endpointInstance.Stop().ConfigureAwait(false);
        }
    }
}
