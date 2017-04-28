using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Slalom.Stacks.Services.Messaging;

namespace Slalom.Stacks.Messaging.Services
{
    public class ScheduleRunner : UntypedActor
    {
        private readonly IMessageGateway _messages;

        public ScheduleRunner(IMessageGateway messages)
        {
            _messages = messages;
        }
        protected override void OnReceive(object message)
        {
            _messages.Send(message);
        }
    }

    public class ServicesCoordinator : ReceiveActor
    {
        protected override void PreStart()
        {
            base.PreStart();

            Context.ActorOf(Context.DI().Props<GetInventoryActor>(), "registry");
            Context.ActorOf(Context.DI().Props<RemoteCallActor>().WithRouter(new RoundRobinPool(15)), "remote");
            Context.ActorOf(Context.DI().Props<LogService>().WithRouter(new RoundRobinPool(15)), "logs");
            Context.ActorOf(Context.DI().Props<ScheduleRunner>().WithRouter(new RoundRobinPool(15)), "schedule");
        }   
    }
}
