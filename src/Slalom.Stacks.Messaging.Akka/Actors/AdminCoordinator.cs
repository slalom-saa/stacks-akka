using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;

namespace Slalom.Stacks.Messaging.Actors
{
    public class AdminCoordinator : ReceiveActor
    {
        protected override void PreStart()
        {
            base.PreStart();

            Context.ActorOf(Context.DI().Props<RegistryService>(), "registry");
            Context.ActorOf(Context.DI().Props<RemoteCallActor>().WithRouter(new RoundRobinPool(15)), "inbound");
        }
    }
}
