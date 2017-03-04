using System;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using Slalom.Stacks.Services;
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
        private ServiceRegistry _registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamListener"/> class.
        /// </summary>
        /// <param name="components">The components.</param>
        public EventStreamListener(IComponentContext components)
        {
            Argument.NotNull(components, nameof(components));

            _components = components;
            _registry = components.Resolve<ServiceRegistry>();

            this.ReceiveAsync<MessageExecutionContext>(this.Execute);
        }

        private async Task Execute(MessageExecutionContext arg)
        {
            foreach (var entry in new [] { _registry.Find(arg.Request.Path, arg.Request.Message) })
            {
                var handler = _components.Resolve(Type.GetType(entry.Type));
                if (handler is IUseMessageContext)
                {
                    ((IUseMessageContext)handler).UseContext(arg);
                }
                await (Task)typeof(IHandle<>).MakeGenericType(arg.Request.Message.GetType()).GetMethod("Handle").Invoke(handler, new object[] { arg.Request.Message });
            }
        }
    }
}