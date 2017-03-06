using System;
using System.Linq;
using Akka.Actor;
using Slalom.Stacks.Services;

namespace Slalom.Stacks.Messaging.Application
{
    public class GetUptimeCommand
    {
    }

    [EndPoint("_systems/akka")]
    public class GetAkkaStatus : UseCase<GetUptimeCommand, object>
    {
        private readonly ActorSystem _system;

        public GetAkkaStatus(ActorSystem system)
        {
            _system = system;
        }

        public override object Execute(GetUptimeCommand command)
        {
            return new
            {
                _system.StartTime,
                _system.Uptime
            };
        }
    }
}