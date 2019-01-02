using System;
using System.Threading.Tasks;
using NServiceBus;
using SqlMessages;

namespace SqlAsbBridge.SqlHandler
{
    public class SqlActionCompletedEventHandler : IHandleMessages<SqlActionCompletedEvent>
    {
        public Task Handle(SqlActionCompletedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"SqlActionCompletedEvent handler with id: {message.ActionId}");
            return Task.CompletedTask;
        }
    }
}