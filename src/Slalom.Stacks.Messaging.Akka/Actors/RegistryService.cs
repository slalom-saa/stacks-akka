using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Slalom.Stacks.Messaging.Actors;

namespace Slalom.Stacks.Messaging
{
    public class RegistryService : ReceiveActor
    {
        private readonly LocalRegistry _registry;

        public RegistryService(LocalRegistry registry)
        {
            _registry = registry;

            this.Receive<GetRegistryCommand>(m =>
            {
                this.Sender.Tell(new RemoteRegistry(m.RemotePath, _registry));
            });
        }
    }
}
