using System;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Autofac;

namespace Slalom.Stacks.Messaging.Routing
{
    public class CommandCoordinator : ReceiveActor
    {
        public AkkaMessageDispatcher Dispatcher { get; set; }
        public IComponentContext ComponentContext { get; set; }

        public string Path
        {
            get
            {
                var path = Context.Self.Path.ToString();
                return path.Substring(path.IndexOf("user/", StringComparison.OrdinalIgnoreCase) + 5);
            }
        }

        protected override void PreStart()
        {
            base.PreStart();

            var node = this.Dispatcher.RootNode.Find(this.Path);
            foreach (var child in node.Nodes)
            {
                var name = child.Path.Substring(child.Path.LastIndexOf('/') + 1);
                if (Context.Child(name).Equals(ActorRefs.Nobody))
                {
                    if (child.Type == null)
                    {
                        Context.ActorOf(Context.DI().Props<CommandCoordinator>(), name);
                    }
                    else if (child.RequestType == null)
                    {
                        Context.ActorOf(Context.DI().Props(child.Type), child.Path);
                    }
                    else
                    {
                        try
                        {
                            Context.ActorOf(Context.DI().Props(typeof(AkkaActorHost)).WithRouter(FromConfig.Instance), name);
                        }
                        catch
                        {
                            Context.ActorOf(Context.DI().Props(typeof(AkkaActorHost)), name);
                        }
                    }
                }
            }
        }
    }
}