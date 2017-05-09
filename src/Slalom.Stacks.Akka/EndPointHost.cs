using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Messaging;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Messaging
{
    /// <summary>
    /// An Akka.NET actor that executes an endpoint.
    /// </summary>
    /// <seealso cref="Akka.Actor.ReceiveActor" />
    public class EndPointHost : ReceiveActor
    {
        private int _currentRetries;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndPointHost"/> class.
        /// </summary>
        public EndPointHost()
        {
            this.ReceiveAsync<ExecutionContext>(this.Execute);
        }

        public IComponentContext Components { get; set; }

        /// <summary>
        /// Gets the number of retries.
        /// </summary>
        /// <value>The number of retries.</value>
        public virtual int Retries { get; } = 0;

        private Dictionary<Type, Object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// Executes the the use case given the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A task for asynchronous programming.</returns>
        protected virtual async Task Execute(ExecutionContext request)
        {
            Argument.NotNull(request, nameof(request));

            var endPoint = request.EndPoint;
            object handler = null;
            if (_services.ContainsKey(endPoint.ServiceType))
            {
                handler = _services[endPoint.ServiceType];
            }
            else
            {
                handler = this.Components.Resolve(endPoint.ServiceType);
                _services.Add(endPoint.ServiceType, handler);
            }
            var service = handler as IEndPoint;
            if (service != null)
            {
                service.Context = request;
            }

            await (Task)endPoint.Method.Invoke(service, new object[] { request.Request.Message.Body });

            if (request.Exception != null)
            {
                throw request.Exception;
            }

            this.Sender.Tell(new MessageResult(request));
        }

        /// <inheritdoc />
        protected override void PreRestart(Exception reason, object message)
        {
            _currentRetries++;

            if (_currentRetries >= this.Retries)
            {
                this.Sender.Tell(new MessageResult((ExecutionContext)message));
            }
            else
            {
                var item = (ExecutionContext)message;
                var context = new ExecutionContext(item.Request, item.EndPoint, item.CancellationToken);
                this.Self.Forward(context);
            }
        }
    }
}