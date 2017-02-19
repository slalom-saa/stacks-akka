using System;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Autofac;

namespace Slalom.Stacks.Messaging.Routing
{
    public class AkkaSupervisor : ReceiveActor
    {
        public AkkaMessageRouter Router { get; set; }
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

            var node = this.Router.RootNode.Find(this.Path);
            foreach (var child in node.Nodes)
            {
                var name = child.Path.Substring(child.Path.LastIndexOf('/') + 1);
                if (Context.Child(name).Equals(ActorRefs.Nobody))
                {
                    if (child.Type == null)
                    {
                        Context.ActorOf(Context.DI().Props<AkkaSupervisor>(), name);
                    }
                    else
                    {
                        try
                        {
                            Context.ActorOf(Context.DI().Props(typeof(AkkaActorHost<>).MakeGenericType(child.Type)).WithRouter(FromConfig.Instance), name);
                        }
                        catch
                        {
                            Context.ActorOf(Context.DI().Props(typeof(AkkaActorHost<>).MakeGenericType(child.Type)), name);
                        }
                    }
                }
            }
        }
    }
}