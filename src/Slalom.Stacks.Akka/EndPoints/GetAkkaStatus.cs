using System;
using System.Linq;
using Akka.Actor;
using Autofac;
using Slalom.Stacks.Services;

namespace Slalom.Stacks.Messaging.EndPoints
{
    [EndPoint("_system/akka")]
    public class GetAkkaStatus : EndPoint
    {
        private IComponentContext _components;

        public GetAkkaStatus(IComponentContext components)
        {
            _components = components;
        }

        public override void Receive()
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