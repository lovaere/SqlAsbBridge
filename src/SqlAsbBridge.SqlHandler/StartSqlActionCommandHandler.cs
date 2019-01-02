using System;
using System.Threading.Tasks;
using NServiceBus;
using SqlMessages;

namespace SqlAsbBridge.SqlHandler
{
    public class StartSqlActionCommandHandler : IHandleMessages<StartSqlActionCommand>
    {
        public Task Handle(StartSqlActionCommand message, IMessageHandlerContext context)
        {
            Console.WriteLine($"StartSqlActionCommand handler with id: {message.ActionId}");
            return Task.CompletedTask;
        }
    }
}
