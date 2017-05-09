using System;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Slalom.Stacks.Messaging.EndPoints;
using Slalom.Stacks.Services.Messaging;

namespace Slalom.Stacks.Messaging.Messaging
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
            Context.ActorOf(Context.DI().Props<ScheduleRunner>().WithRouter(new RoundRobinPool(15)), "schedule");
        }   
    }
}
