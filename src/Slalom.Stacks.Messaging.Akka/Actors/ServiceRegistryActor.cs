using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Slalom.Stacks.Messaging.Actors;
using Slalom.Stacks.Messaging.Registration;

namespace Slalom.Stacks.Messaging.Actors
{
    public class ServiceRegistryActor : ReceiveActor
    {
        private readonly ServiceRegistry _registry;

        public ServiceRegistryActor(ServiceRegistry registry)
        {
            _registry = registry;

            this.Receive<GetRegistryCommand>(m =>
            {
                this.Sender.Tell(_registry);
            });
        }
    }
}
