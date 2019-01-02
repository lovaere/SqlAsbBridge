using NServiceBus;

namespace AsbMessages
{
    public class StartAzureActionCommand : ICommand
    {
        public StartAzureActionCommand(string actionId)
        {
            ActionId = actionId;
        }

        public string ActionId { get; set; }
    }
}