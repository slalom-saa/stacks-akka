using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;

namespace Slalom.Stacks.Messaging.Application
{
    [EndPoint("_systems/akka")]
    public class GetAkkaStatus : ServiceEndPoint
    {
        private IComponentContext _components;

        public GetAkkaStatus(IComponentContext components)
        {
            _components = components;
        }

        public override void Execute()
        {
            this.Respond(_components.ResolveAll<ActorSystem>()
                       .Select(e => new
                       {
                           e.Name,
                           e.StartTime,
                           e.Uptime
                       }));
        }
    }
}