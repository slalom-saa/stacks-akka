using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;

namespace Slalom.Stacks.Messaging.Application
{
    public class GetAkkaStatusCommand
    {
    }

    [EndPoint("_systems/akka")]
    public class GetAkkaStatus : EndPoint<GetAkkaStatusCommand, object>
    {
        private IComponentContext _components;

        public GetAkkaStatus(IComponentContext components)
        {
            _components = components;
        }

        public override object Receive(GetAkkaStatusCommand command)
        {
            return _components.ResolveAll<ActorSystem>()
                       .Select(e => new
                       {
                           e.Name,
                           e.StartTime,
                           e.Uptime
                       });
        }
    }
}