using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;

namespace Slalom.Stacks.Messaging.Services
{
    public class ServicesCoordinator : ReceiveActor
    {
        protected override void PreStart()
        {
            base.PreStart();

            Context.ActorOf(Context.DI().Props<ServiceRegistryActor>(), "registry");
            Context.ActorOf(Context.DI().Props<RemoteCallActor>().WithRouter(new RoundRobinPool(15)), "remote");
            Context.ActorOf(Context.DI().Props<LogService>().WithRouter(new RoundRobinPool(15)), "logs");
        }
    }
}
