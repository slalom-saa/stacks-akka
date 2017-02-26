using System;
using Autofac;
using System.Linq;
using System.Reflection;
using Akka.Actor;
using Slalom.Stacks.Messaging.Actors;
using Slalom.Stacks.Messaging.Routing;
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

            builder.RegisterType<AdminCoordinator>().AsSelf();
            builder.RegisterType<RegistryService>().AsSelf();
            builder.RegisterType<RemoteCallActor>().AsSelf();

            builder.RegisterAssemblyTypes(_assemblies)
                   .Where(e => e.GetBaseAndContractTypes().Any(x => x == typeof(ActorBase)))
                   .AsSelf()
                   .InstancePerDependency()
                   .PropertiesAutowired();

            builder.RegisterGeneric(typeof(UseCaseActor<,>));
        }
    }
}