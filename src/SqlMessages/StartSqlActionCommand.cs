using NServiceBus;

namespace SqlMessages
{
    public class StartSqlActionCommand : ICommand
    {
        public StartSqlActionCommand(string actionId)
        {
            ActionId = actionId;
        }

        public string ActionId { get; set; }
    }
}
