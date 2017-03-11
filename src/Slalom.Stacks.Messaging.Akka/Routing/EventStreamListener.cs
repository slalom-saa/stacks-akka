using System;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Messaging.Routing
{
    /// <summary>
    /// Executes use cases by listening to the Akka.NET event stream.
    /// </summary>
    /// <seealso cref="Akka.Actor.ReceiveActor" />
    public class EventStreamListener : ReceiveActor
    {
        private readonly IComponentContext _components;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamListener"/> class.
        /// </summary>
        /// <param name="components">The components.</param>
        public EventStreamListener(IComponentContext components)
        {
            Argument.NotNull(components, nameof(components));

            _components = components;

            this.ReceiveAsync<ExecutionContext>(this.Execute);
        }

        private async Task Execute(ExecutionContext message)
        {
            var handlers = _components.ResolveAll(typeof(IHandle<>).MakeGenericType(message.Request.Message.MessageType));
            foreach (var handler in handlers)
            {
                await (Task)typeof(IHandle<>).MakeGenericType(message.Request.Message.MessageType).GetMethod("Handle").Invoke(handler, new[] { message.Request.Message.Body });
            }

            this.Sender.Tell(new MessageResult(message));
        }
    }
}