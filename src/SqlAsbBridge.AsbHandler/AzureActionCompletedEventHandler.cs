using System;
using System.Threading.Tasks;
using AsbMessages;
using NServiceBus;

namespace SqlAsbBridge.AsbHandler
{
    public class AzureActionCompletedEventHandler : IHandleMessages<AzureActionCompletedEvent>
    {
        public Task Handle(AzureActionCompletedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"AzureActionCompletedEvent handler with id: {message.ActionId}");
            return Task.CompletedTask;
        }
    }
}