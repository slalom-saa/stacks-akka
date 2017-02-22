using System;
using System.Collections.Generic;
using Akka.Actor;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Slalom.Stacks.Messaging.Pipeline;

namespace Slalom.Stacks.Messaging.Routing
{
    public class AkkaActorHost : ReceiveActor
    {
        public IComponentContext Components { get; set; }

        public AkkaActorHost()
        {
            this.ReceiveAsync<MessageEnvelope>(this.HandleCommand);
        }

        private async Task<MessageResult> HandleCommand(MessageEnvelope envelope)
        {
            await envelope.Message.Recipient.Handle(envelope.Message.Message, envelope.Context);

            var result = new MessageResult(envelope.Context);

            this.Sender.Tell(result);

            return result;
        }
    }
}