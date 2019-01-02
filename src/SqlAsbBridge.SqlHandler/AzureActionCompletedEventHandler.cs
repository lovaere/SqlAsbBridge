using System;
using System.Threading.Tasks;
using AsbMessages;
using NServiceBus;

namespace SqlAsbBridge.SqlHandler
{
    public class AzureActionCompletedEventHandler : IHandleMessages<AzureActionCompletedEvent>
    {
        public Task Handle(AzureActionCompletedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"[Routed Message!] AzureActionCompletedEvent handler with id: {message.ActionId}");
            return Task.CompletedTask;
        }
    }
}