using System;
using Akka.Actor;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Slalom.Stacks.Services;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Messaging.Routing
{
    /// <summary>
    /// An Akka.NET actor that executes an endpoint.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <seealso cref="Akka.Actor.ReceiveActor" />
    public class EndPointHost<TService> : ReceiveActor where TService : Service
    {
        private readonly TService _service;

        private int _currentRetries;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndPointHost{TService}"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public EndPointHost(TService service)
        {
            Argument.NotNull(service, nameof(service));

            _service = service;

            this.ReceiveAsync<ExecutionContext>(this.Execute);
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
        protected virtual async Task Execute(ExecutionContext request)
        {
            Argument.NotNull(request, nameof(request));

            var service = _service as IService;
            if (service != null)
            {
                service.Context = request;
            }
            
            await (Task)typeof(IEndPoint<>).MakeGenericType(Type.GetType(request.EndPoint.RequestType)).GetMethod("Receive").Invoke(_service, new object[] { request.Request.Message.Body });

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
                //Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(.5), this.Self, context, this.Sender);
                this.Self.Forward(context);
            }
        }
    }
}