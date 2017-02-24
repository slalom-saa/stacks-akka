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

            if (request.Context.Exception != null)
            {
                throw request.Context.Exception;
            }

            this.Sender.Tell(new MessageResult(request.Context));
        }

        private int retries = 0;

        protected override void PreRestart(Exception reason, object message)
        {
            retries++;
            if (retries > 1)
            {
                Sender.Tell(new MessageResult(((AkkaRequest) message).Context));
            }
            else
            {
                var item = (AkkaRequest) message;
                var context = new MessageContext(item.Context.RequestName, item.Context.RequestName, null, item.Context.Execution, item.Context);
                Self.Forward(new AkkaRequest(item.Message, context));
            }
        }

    }
}