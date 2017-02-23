using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using Slalom.Stacks.Runtime;

namespace Slalom.Stacks.Messaging.Routing
{
    public class CommandCoordinator : ReceiveActor
    {
        protected readonly IComponentContext _components;

        public CommandCoordinator(IComponentContext components)
        {
            _components = components;

            this.ReceiveAsync<AkkaRequest>(this.Execute);
        }

        protected virtual async Task Execute(AkkaRequest request)
        {
            var handler = _components.Resolve(typeof(IHandle<>).MakeGenericType(request.Message.GetType()));

            var executionContext = _components.Resolve<IExecutionContextResolver>().Resolve();
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
