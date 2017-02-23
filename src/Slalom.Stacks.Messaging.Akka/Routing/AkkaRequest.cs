using Slalom.Stacks.Runtime;

namespace Slalom.Stacks.Messaging.Routing
{
    public class AkkaRequest
    {
        public IMessage Message { get; }

        public MessageContext Context { get; }

        public AkkaRequest(IMessage message, MessageContext context)
        {
            this.Message = message;
            this.Context = context;
        }
    }
}