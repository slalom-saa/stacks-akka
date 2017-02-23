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
    public class AkkaActorHost<THandler, TMessage> : ReceiveActor where THandler : IHandle<TMessage>
    {
        private readonly THandler _handler;

        public AkkaActorHost(THandler handler)
        {
            _handler = handler;

            this.ReceiveAsync<AkkaRequest>(this.Execute);
        }

        private async Task Execute(AkkaRequest request)
        {
            if (_handler is IUseMessageContext)
            {
                ((IUseMessageContext)_handler).UseContext(request.Context);
            }

            await _handler.Handle((TMessage)request.Message);

            this.Sender.Tell(new MessageResult(request.Context));
        }
    }
}