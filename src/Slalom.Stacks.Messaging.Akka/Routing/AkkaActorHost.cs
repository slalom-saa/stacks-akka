using System;
using Akka.Actor;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Messaging.Pipeline;

namespace Slalom.Stacks.Messaging.Routing
{
    public class AkkaActorHost : ReceiveActor
    {
        public IMessageExecutionPipeline Pipeline { get; set; }

        public AkkaActorHost()
        {
            this.ReceiveAsync<MessageEnvelope>(this.HandleCommand);
        }

        private async Task<MessageResult> HandleCommand(MessageEnvelope envelop)
        {
            await this.Pipeline.Execute(envelop.Message, envelop.Context);

            var result = new MessageResult(envelop.Context);

            this.Sender.Tell(result);

            return result;
        }
    }
}