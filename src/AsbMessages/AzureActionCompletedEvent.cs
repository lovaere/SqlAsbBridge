using NServiceBus;

namespace AsbMessages
{
    public class AzureActionCompletedEvent : IEvent
    {
        public AzureActionCompletedEvent(string actionId)
        {
            ActionId = actionId;
        }

        public string ActionId { get; set; }
    }
}
