using System;
using Akka.Actor;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Messaging.Routing
{
    /// <summary>
    /// An Akka.NET actor that executes a use case.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <seealso cref="Akka.Actor.ReceiveActor" />
    public class UseCaseActor<THandler, TMessage> : ReceiveActor where THandler : IHandle<TMessage>
    {
        private readonly THandler _handler;

        private int _currentRetries;

        /// <summary>
        /// Initializes a new instance of the <see cref="UseCaseActor{THandler, TMessage}"/> class.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public UseCaseActor(THandler handler)
        {
            Argument.NotNull(handler, nameof(handler));

            _handler = handler;

            this.ReceiveAsync<AkkaRequest>(this.Execute);
        }

        /// <summary>
        /// Gets the number of retries.
        /// </summary>
        /// <value>The number of retries.</value>
        public virtual int Retries { get; } = 0;

        /// <summary>
        /// Executes the the use case given the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A task for asynchronous programming.</returns>
        protected virtual async Task Execute(AkkaRequest request)
        {
            Argument.NotNull(request, nameof(request));

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

        /// <inheritdoc />
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
                var context = new MessageExecutionContext(item.Context.RequestContext, item.Context.Service, item.Context.ExecutionContext, item.Context);
                this.Self.Forward(new AkkaRequest(item.Message, context));
            }
        }
    }
}