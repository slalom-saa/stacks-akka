using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
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
        private LocalRegistry _registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamListener"/> class.
        /// </summary>
        /// <param name="components">The components.</param>
        public EventStreamListener(IComponentContext components)
        {
            Argument.NotNull(components, nameof(components));

            _components = components;
            _registry = components.Resolve<LocalRegistry>();

            this.ReceiveAsync<AkkaRequest>(this.Execute);
        }

        private async Task Execute(AkkaRequest arg)
        {
            foreach (var entry in _registry.Find(arg.Message))
            {
                var handler = _components.Resolve(entry.Type);
                if (handler is IUseMessageContext)
                {
                    ((IUseMessageContext)handler).UseContext(arg.Context);
                }
                await (Task)typeof(IHandle<>).MakeGenericType(arg.Message.GetType()).GetMethod("Handle").Invoke(handler, new object[] { arg.Message });
            }
        }
    }
}