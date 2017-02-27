using System;
using System.Linq;
using Akka.Actor;
using Slalom.Stacks.Messaging.Registration;

namespace Slalom.Stacks.Messaging.Services
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
