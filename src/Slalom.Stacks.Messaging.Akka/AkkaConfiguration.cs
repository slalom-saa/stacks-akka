using Akka.Actor;
using Akka.DI.AutoFac;
using Autofac;
using Slalom.Stacks.Messaging.Routing;
using Slalom.Stacks.Runtime;

// ReSharper disable ObjectCreationAsStatement

namespace Slalom.Stacks.Messaging
{
    public static class AkkaConfiguration
    {
        public static Stack UseAkka(this Stack instance, string name)
        {
            var system = ActorSystem.Create(name);
            new AutoFacDependencyResolver(instance.Container, system);
            instance.Use(builder =>
            {
                builder.RegisterModule(new AkkaModule(instance.Assemblies));

                builder.Register(c => system).AsSelf().SingleInstance();

                builder.Register(c => new AkkaMessageDispatcher(system, c.Resolve<IComponentContext>()))
                       .As<IMessageDispatcher>()
                       .SingleInstance();

            });

            return instance;
        }
    }
}