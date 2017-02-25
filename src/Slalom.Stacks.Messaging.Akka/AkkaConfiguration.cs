using Akka.Actor;
using Akka.DI.AutoFac;
using Autofac;
using Slalom.Stacks.Messaging.Routing;
using Slalom.Stacks.Runtime;

// ReSharper disable ObjectCreationAsStatement

namespace Slalom.Stacks.Messaging
{
    /// <summary>
    /// Extension methods for configuring the Akka.NET messaging blocks.
    /// </summary>
    public static class AkkaConfiguration
    {
        /// <summary>
        /// Configures the stack to use Akka.NET Messaging.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The name.</param>
        /// <returns>Stack.</returns>
        public static Stack UseAkka(this Stack instance, string name)
        {
            var system = ActorSystem.Create(name);
            new AutoFacDependencyResolver(instance.Container, system);
            instance.Use(builder =>
            {
                builder.RegisterModule(new AkkaModule(instance.Assemblies));

                builder.Register(c => system).AsSelf().SingleInstance();

                builder.Register(c => new AkkaMessagingGatewayAdapter(system, c.Resolve<IComponentContext>()))
                       .As<IMessageGatewayAdapter>()
                       .SingleInstance();

            });

            return instance;
        }
    }
}