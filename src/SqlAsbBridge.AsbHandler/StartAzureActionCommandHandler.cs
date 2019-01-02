using System;
using System.Threading.Tasks;
using AsbMessages;
using NServiceBus;

namespace SqlAsbBridge.AsbHandler
{
    public class StartAzureActionCommandHandler : IHandleMessages<StartAzureActionCommand>
    {
        public Task Handle(StartAzureActionCommand message, IMessageHandlerContext context)
        {
            Console.WriteLine($"StartAzureActionCommand handler with id: {message.ActionId}");
            return Task.CompletedTask;
        }
    }
}