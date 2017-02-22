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
            this.ReceiveAsync<Request>(this.HandleCommand);
        }

        private async Task<MessageResult> HandleCommand(Request request)
        {
            await request.Execute();

            var result = new MessageResult(request.Context);

            this.Sender.Tell(result);

            return result;
        }
    }
}