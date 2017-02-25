using Slalom.Stacks.Runtime;

namespace Slalom.Stacks.Messaging.Routing
{
    public class AkkaRequest
    {
        public IMessage Message { get; }

        public MessageExecutionContext Context { get; }

        public AkkaRequest(IMessage message, MessageExecutionContext context)
        {
            this.Message = message;
            this.Context = context;
        }
    }
}