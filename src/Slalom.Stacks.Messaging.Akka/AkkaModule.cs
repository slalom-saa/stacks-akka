using System;
using System.Collections.Generic;
using Autofac;
using System.Linq;
using System.Reflection;
using Akka.Actor;
using Slalom.Stacks.Messaging.Application;
using Slalom.Stacks.Messaging.Routing;
using Slalom.Stacks.Messaging.Services;
using Slalom.Stacks.Reflection;
using Module = Autofac.Module;

namespace Slalom.Stacks.Messaging
{
    /// <summary>
    /// Autofac module that configures the Akka.NET messaging block.
    /// </summary>
    /// <seealso cref="Autofac.Module" />
    public class AkkaModule : Module
    {
        private readonly Stack _stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="AkkaModule" /> class.
        /// </summary>
        /// <param name="stack">The current stack.</param>
        public AkkaModule(Stack stack)
        {
            _stack = stack;

            _stack.Include(this.GetType());
        }

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ServicesCoordinator>().AsSelf();
            builder.RegisterType<ServiceRegistryActor>().AsSelf();
            builder.RegisterType<RemoteCallActor>().AsSelf();
            builder.RegisterType<LogService>().AsSelf();

            builder.Register(c => new[] { new AkkaMessageDispatcher(c.Resolve<ActorSystem>(), c.Resolve<IComponentContext>()) }).As<IEnumerable<IMessageDispatcher>>();

            builder.RegisterAssemblyTypes(_stack.Assemblies.ToArray())
                   .Where(e => e.GetBaseAndContractTypes().Any(x => x == typeof(ActorBase)))
                   .AsSelf()
                   .InstancePerDependency()
                   .PropertiesAutowired();

            builder.RegisterGeneric(typeof(EndPointActor<>));

            _stack.Assemblies.CollectionChanged += (sender, args) =>
            {
                _stack.Use(b =>
                {
                    b.RegisterAssemblyTypes(args.NewItems.OfType<Assembly>().ToArray())
                     .Where(e => e.GetBaseAndContractTypes().Any(x => x == typeof(ActorBase)))
                     .AsSelf()
                     .InstancePerDependency()
                     .PropertiesAutowired();
                });
            };
        }
    }
}