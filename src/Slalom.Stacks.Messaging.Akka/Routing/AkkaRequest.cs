namespace Slalom.Stacks.Messaging.Routing
{
    public class AkkaRequest
    {
        public IMessage Message { get; }
        public MessageContext Context { get; }

        public AkkaRequest(IMessage message, MessageContext context = null)
        {
            this.Message = message;
            this.Context = context;
        }
    }
}