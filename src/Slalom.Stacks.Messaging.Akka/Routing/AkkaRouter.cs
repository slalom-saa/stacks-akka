using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Autofac;
using Newtonsoft.Json;
using Slalom.Stacks.Reflection;

namespace Slalom.Stacks.Messaging.Routing
{
    public class AkkaRouter
    {
        private readonly ActorSystem _system;

        public AkkaRouter(ActorSystem system, IComponentContext context)
        {
            _system = system;
        }

        public AkkaActorNode RootNode { get; private set; }

        public void Arrange(Assembly[] assemblies)
        {
            var items = new List<ActorMapping>();
            var actors = assemblies.SafelyGetTypes(typeof(IHandle))
                .Union(assemblies.SafelyGetTypes(typeof(ActorBase)));

            foreach (var actor in actors)
            {
                var path = actor.GetCustomAttributes<PathAttribute>().FirstOrDefault()?.Path;
                if (path != null)
                {
                    items.Add(new ActorMapping(path, actor));
                }
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
                    _system.ActorOf(_system.DI().Props(child.Type), child.Path);
                }
            }
        }

        public async Task<CommandResult> Send(string path, string request)
        {
            var node = this.RootNode.Find(path);

            var command = (ICommand) JsonConvert.DeserializeObject(request, GetRequestType(node));

            var result = await _system.ActorSelection("user/" + node.Path).Ask(command);

            return result as CommandResult;
        }

        public async Task<CommandResult> Send(ICommand command)
        {
            var node = this.RootNode.Find(command);

            var result = await _system.ActorSelection("user/" + node.Path).Ask(command);

            return result as CommandResult;
        }

        public async Task<CommandResult> Send(string command, ICommand request)
        {
            var node = this.RootNode.Find(command);

            var result = await _system.ActorSelection("user/" + node.Path).Ask(request);

            return result as CommandResult;
        }

        private static Type GetRequestType(AkkaActorNode node)
        {
            return node.Type.BaseType.GetGenericArguments()[0];
        }

        private void PopulateActorNode(AkkaActorNode parent, IEnumerable<ActorMapping> paths)
        {
            var items = paths.Select(e => new KeyValuePair<string[], Type>(e.Path.Split('/'), e.Type)).OrderBy(e => e.Key.Length);

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

        public class ActorMapping
        {
            public ActorMapping(string path, Type type)
            {
                this.Path = path;
                if (type.IsGenericType && type.GetInterfaces().Any(e => e == typeof(IHandle)))
                {
                    this.Type = typeof(AkkaHandler<>).MakeGenericType(type);
                }
                else
                {
                    this.Type = type;
                }
            }

            public ActorMapping()
            {
            }

            public string Path { get; set; }

            public Type Type { get; set; }

            public static implicit operator ActorMapping(string value)
            {
                return new ActorMapping {Path = value};
            }
        }
    }
}