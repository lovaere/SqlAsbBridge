using NServiceBus;

namespace SqlMessages
{
    public class SqlActionCompletedEvent : IEvent
    {
        public SqlActionCompletedEvent(string actionId)
        {
            ActionId = actionId;
        }

        public string ActionId { get; set; }
    }
}