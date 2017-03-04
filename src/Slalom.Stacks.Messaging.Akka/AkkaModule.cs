using System;
using System.Collections.Generic;
using Autofac;
using System.Linq;
using System.Reflection;
using Akka.Actor;
using Slalom.Stacks.Messaging.Routing;
using Slalom.Stacks.Messaging.Services;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Validation;
using Module = Autofac.Module;

namespace Slalom.Stacks.Messaging
{
    /// <summary>
    /// Autofac module that configures the Akka.NET messaging block.
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class AkkaModule : Module
    {
        private readonly Assembly[] _assemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="AkkaModule"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public AkkaModule(Assembly[] assemblies)
        {
            Argument.NotNull(assemblies, nameof(assemblies));

            _assemblies = assemblies.Union(new[] { typeof(AkkaModule).Assembly }).ToArray();
        }

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ServicesCoordinator>().AsSelf();
            builder.RegisterType<ServiceRegistryActor>().AsSelf();
            builder.RegisterType<RemoteCallActor>().AsSelf();
            builder.RegisterType<LogService>().AsSelf();

            //builder.Register(c=>new List<IMessageDispatcher> { new AkkaMessageDispatcher
            builder.Register(c => new[] { new AkkaMessageDispatcher(c.Resolve<ActorSystem>(), c.Resolve<IComponentContext>()) }).As<IEnumerable<IMessageDispatcher>>();

            builder.RegisterAssemblyTypes(_assemblies)
                   .Where(e => e.GetBaseAndContractTypes().Any(x => x == typeof(ActorBase)))
                   .AsSelf()
                   .InstancePerDependency()
                   .PropertiesAutowired();

            builder.RegisterGeneric(typeof(UseCaseActor<,>));
        }
    }
}
