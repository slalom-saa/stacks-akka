using System;
using System.Linq;
using System.Reflection;
using Akka.Actor;
using Autofac;
using Slalom.Stacks.Configuration;
using Slalom.Stacks.Messaging.Routing;
using Slalom.Stacks.Reflection;
using Module = Autofac.Module;

namespace Slalom.Stacks.Messaging
{
    public class AkkaModule : Module
    {
        private readonly Assembly[] _assemblies;

        public AkkaModule(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new DefaultSupervisorStrategy())
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterAssemblyTypes(_assemblies)
                .Where(e => e.GetBaseAndContractTypes().Any(x => x == typeof(ActorBase)))
                .AsSelf().PropertiesAutowired(new AllUnsetPropertySelector());

            var types = _assemblies.SafelyGetTypes(typeof(IHandle));
            foreach (var type in types)
            {
                builder.RegisterType(typeof(AkkaHandler<>).MakeGenericType(type))
                    .AsSelf();
            }
        }
    }
}