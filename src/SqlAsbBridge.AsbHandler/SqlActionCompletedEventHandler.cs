using System;
using System.Threading.Tasks;
using NServiceBus;
using SqlMessages;

namespace SqlAsbBridge.AsbHandler
{
    public class SqlActionCompletedEventHandler : IHandleMessages<SqlActionCompletedEvent>
    {
        public Task Handle(SqlActionCompletedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"[Routed Message!] SqlActionCompletedEvent handler with id: {message.ActionId}");
            return Task.CompletedTask;
        }
    }
}