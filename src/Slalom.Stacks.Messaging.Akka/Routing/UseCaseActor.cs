using System;
using Akka.Actor;
using System.Linq;
using System.Threading.Tasks;

namespace Slalom.Stacks.Messaging.Routing
{
    public class UseCaseActor<THandler, TMessage> : ReceiveActor where THandler : IHandle<TMessage>
    {
        private readonly THandler _handler;

        public virtual int Retries { get; set; } = 0;

        private int _currentRetries;

        public UseCaseActor(THandler handler)
        {
            _handler = handler;

            this.ReceiveAsync<AkkaRequest>(this.Execute);
        }

        protected virtual async Task Execute(AkkaRequest request)
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

        protected override void PreRestart(Exception reason, object message)
        {
            _currentRetries++;

            if (_currentRetries >= this.Retries)
            {
                this.Sender.Tell(new MessageResult(((AkkaRequest)message).Context));
            }
            else
            {
                var item = (AkkaRequest)message;
                var context = new MessageExecutionContext(item.Context.RequestContext, item.Context.RegistryEntry, item.Context.ExecutionContext, item.Context);
                this.Self.Forward(new AkkaRequest(item.Message, context));
            }
        }
    }
}