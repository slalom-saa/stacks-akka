using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Autofac;
using ConsoleClient.Application.Products.Add;
using ConsoleClient.Application.Products.Stock;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Messaging.Routing;
using Slalom.Stacks.Text;
using Slalom.Stacks.Reflection;

namespace ConsoleClient
{
    public class ProductsCommandCoordinator : CommandCoordinator
    {
        public ProductsCommandCoordinator(IComponentContext components)
            : base(components)
        {
        }

        protected override async Task Execute(AkkaRequest request)
        {
            var handler = _components.Resolve(typeof(Slalom.Stacks.Messaging.IHandle<>).MakeGenericType(request.Message.GetType()));
            var name = handler.GetType().Name.ToDelimited("-");
            var attr = handler.GetType().GetAllAttributes<PathAttribute>().FirstOrDefault();
            if (attr != null)
            {
                name = attr.Path;
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
