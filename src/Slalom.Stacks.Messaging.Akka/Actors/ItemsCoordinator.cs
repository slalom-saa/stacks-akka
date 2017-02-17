//using System;
//using Akka.Actor;
//using Akka.DI.Core;
//using Akka.Routing;
//using Slalom.Stacks.Messaging.Routing;

//namespace Slalom.Stacks.Messaging.Actors
//{
//    [Path("items")]
//    public class ItemsCoordinator : AkkaSupervisor
//    {
//        protected override void PreStart()
//        {
//            Context.ActorOf(Context.DI().Props<AkkaHandler<AddItemActor>>()
//                .WithRouter(new RoundRobinPool(5)), "add-item");

//            base.PreStart();
//        }

//        protected override bool AroundReceive(Receive receive, object message)
//        {
//            Console.WriteLine("...");
//            return base.AroundReceive(receive, message);
//        }

//    }
//}