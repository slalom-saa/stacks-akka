using System;
using Akka.Actor;
using Akka.DI.Core;
using Autofac;
using System.Linq;
using System.Threading.Tasks;
using Akka.Routing;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Text;

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
            var name = handler.GetType().Name.ToDelimited("-");
            var attr = handler.GetType().GetAllAttributes<PathAttribute>().FirstOrDefault();
            if (attr != null)
            {
                //name = attr.Path;
            }
            if (Context.Child(name).Equals(ActorRefs.Nobody))
            {
                try
                {
                    Context.ActorOf(Context.DI().Props(typeof(AkkaActorHost<,>).MakeGenericType(handler.GetType(), request.Message.GetType()))
                                           .WithRouter(FromConfig.Instance), name);
                }
                catch
                {
                    Context.ActorOf(Context.DI().Props(typeof(AkkaActorHost<,>).MakeGenericType(handler.GetType(), request.Message.GetType())), name);
                }
            }
            Context.Child(name).Forward(request);
        }
    }
}