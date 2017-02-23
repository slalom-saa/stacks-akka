using System;
using System.Collections.Generic;
using Akka.Actor;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Slalom.Stacks.Messaging.Pipeline;
using Slalom.Stacks.Runtime;

namespace Slalom.Stacks.Messaging.Routing
{
    public class AkkaActorHost : ReceiveActor
    {
        public IComponentContext Components { get; set; }

        public AkkaActorHost()
        {
           this.ReceiveAsync<AkkaRequest>(this.HandleCommand);
        }

        private async Task HandleCommand(AkkaRequest request)
        {
            var handler = this.Components.Resolve(typeof(IHandle<>).MakeGenericType(request.Message.GetType()));

            var executionContext = this.Components.Resolve<IExecutionContextResolver>().Resolve();
            var context = new MessageContext(request.Message.Id, handler.GetType().Name, null, executionContext, request.Context);
            if (handler is IUseMessageContext)
            {
                ((IUseMessageContext)handler).UseContext(context);
            }
            await (Task)typeof(IHandle<>).MakeGenericType(request.Message.GetType()).GetMethod("Handle").Invoke(handler, new object[] { request.Message });

            this.Sender.Tell(new MessageResult(context));
        }
    }
}