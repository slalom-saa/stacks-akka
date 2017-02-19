using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Newtonsoft.Json;
using Slalom.Stacks.Domain;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Runtime;

namespace Slalom.Stacks.Messaging.Routing
{
    public class AkkaMessageRouter : IMessageRouter, IEventStream
    {
        private readonly ActorSystem _system;
        private readonly IExecutionContextResolver _context;
        public AkkaActorNode RootNode { get; private set; }

        public AkkaMessageRouter(ActorSystem system, IExecutionContextResolver context)
        {
            _system = system;
            _context = context;
        }

        private void PopulateActorNode(AkkaActorNode parent, IEnumerable<AkkaActorMapping> paths)
        {
            foreach (var source in paths.Where(e => e.Path == null))
            {
                parent.Add(source.Type.Name, source.Type);
            }

            var items = paths.Where(e => e.Path != null).Select(e => new KeyValuePair<string[], Type>(e.Path.Split('/'), e.Type)).OrderBy(e => e.Key.Length);

            foreach (var item in items)
            {
                var current = parent;
                var last = parent;

                var sub = string.Empty;
                foreach (var part in item.Key)
                {
                    sub += part;
                    current = current.Find(sub);
                    if (current == null)
                    {
                        if (part == item.Key.Last())
                        {
                            current = last.Add(sub, item.Value);
                        }
                        else
                        {
                            current = last.Add(sub, null);
                        }
                    }
                    last = current;
                    sub += "/";
                }
            }
        }

        internal void Arrange(Assembly[] assemblies)
        {
            var items = new List<AkkaActorMapping>();
            var actors = assemblies.SafelyGetTypes(typeof(IHandle))
                                   .Union(assemblies.SafelyGetTypes(typeof(ActorBase)));

            foreach (var actor in actors)
            {
                var path = actor.GetCustomAttributes<PathAttribute>().FirstOrDefault()?.Path;
                items.Add(new AkkaActorMapping(path, actor));
            }

            this.RootNode = new AkkaActorNode("root");
            this.PopulateActorNode(this.RootNode, items);

            foreach (var child in this.RootNode.Nodes)
            {
                if (child.Type == null)
                {
                    _system.ActorOf(_system.DI().Props<AkkaSupervisor>(), child.Path);
                }
                else
                {
                    try
                    {
                        _system.ActorOf(_system.DI().Props(typeof(AkkaActorHost<>).MakeGenericType(child.Type)).WithRouter(FromConfig.Instance), child.Path);
                    }
                    catch
                    {
                        _system.ActorOf(_system.DI().Props(typeof(AkkaActorHost<>).MakeGenericType(child.Type)), child.Path);
                    }
                }
            }
        }

        public Task<MessageExecutionResult> Send(string path, string message, TimeSpan? timeout = null)
        {
            var node = this.RootNode.Find(path);

            var command = (IMessage)JsonConvert.DeserializeObject(message, node.Type.GetRequestType());

            return this.Send(command, node);
        }

        public Task<MessageExecutionResult> Send(IMessage message, TimeSpan? timeout = null)
        {
            var node = this.RootNode.Find(message);

            return this.Send(message, node, timeout);
        }

        private async Task<MessageExecutionResult> Send(IMessage message, AkkaActorNode node, TimeSpan? timeout = null, ExecutionContext context = null)
        {
            context = context ?? _context.Resolve();

            var result = await _system.ActorSelection("user/" + node.Path).Ask(new MessageEnvelope(message, context), timeout);

            return result as MessageExecutionResult;
        }

        public Task<MessageExecutionResult> Send(string path, IMessage message, TimeSpan? timeout = null)
        {
            var node = this.RootNode.Find(path);

            return this.Send(message, node, timeout);
        }

        public Task<MessageExecutionResult> Send(string path, TimeSpan? timeout)
        {
            var node = this.RootNode.Find(path);

            var command = (IMessage)JsonConvert.DeserializeObject("{}", node.Type.GetRequestType());

            return this.Send(command, node);
        }

        public void Publish<TEvent>(TEvent instance, ExecutionContext context) where TEvent : Event
        {
            var node = this.RootNode.Find(instance);

            this.Send(instance, node, context: context);
        }
    }
}