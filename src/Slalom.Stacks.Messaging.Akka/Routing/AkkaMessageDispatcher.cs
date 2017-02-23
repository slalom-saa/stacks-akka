using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Autofac;
using Newtonsoft.Json;
using Slalom.Stacks.Domain;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Runtime;

namespace Slalom.Stacks.Messaging.Routing
{
    public class AkkaMessageDispatcher : IMessageDispatcher
    {
        private readonly ActorSystem _system;
        private readonly IComponentContext _components;

        public AkkaActorNode RootNode { get; private set; }

        public AkkaMessageDispatcher(ActorSystem system, IComponentContext components)
        {
            _system = system;
            _components = components;
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
            var actors = assemblies.SafelyGetTypes(typeof(IHandle<>))
                                   .Union(assemblies.SafelyGetTypes(typeof(ActorBase)));

            foreach (var actor in actors)
            {
                var path = actor.GetCustomAttributes<PathAttribute>().FirstOrDefault()?.Path;
                items.Add(new AkkaActorMapping(path, actor));
            }

            this.RootNode = new AkkaActorNode("root");
            this.PopulateActorNode(this.RootNode, items);

            _system.ActorOf(_system.DI().Props(typeof(AkkaActorHost)), "commands");

            foreach (var child in this.RootNode.Nodes)
            {
                if (child.Type == null)
                {
                    _system.ActorOf(_system.DI().Props<CommandCoordinator>(), child.Path);
                }
                else if (child.RequestType == null)
                {
                    _system.ActorOf(_system.DI().Props(child.Type), child.Path);
                }
                else
                {
                    try
                    {
                        _system.ActorOf(_system.DI().Props(typeof(AkkaActorHost)).WithRouter(FromConfig.Instance), child.Path);
                    }
                    catch
                    {
                        _system.ActorOf(_system.DI().Props(typeof(AkkaActorHost)), child.Path);
                    }
                }
            }
        }

        public async Task<MessageResult> Send(ICommand instance, MessageContext context = null, TimeSpan? timeout = null)
        {
            var node = this.RootNode.Find(instance);

            return await this.Execute(instance, context, timeout, node);
        }

        private async Task<MessageResult> Execute(ICommand instance, MessageContext context, TimeSpan? timeout, AkkaActorNode node)
        {
            var handler = _components.Resolve(typeof(IHandle<>).MakeGenericType(instance.GetType()));

            var executionContext = _components.Resolve<IExecutionContextResolver>().Resolve();
            context = new MessageContext(instance.Id, handler.GetType().Name, null, executionContext, context);

            if (handler is IUseMessageContext)
            {
                ((IUseMessageContext)handler).UseContext(context);
            }

            var result = await _system.ActorSelection("user/" + node.Path).Ask(new AkkaRequest(instance, context), timeout);

            return result as MessageResult;
        }

        public async Task Publish(IEvent instance, MessageContext context = null)
        {
            var handlers = _components.ResolveAll(typeof(IHandle<>).MakeGenericType(instance.GetType()));

            var executionContext = _components.Resolve<IExecutionContextResolver>().Resolve();
            foreach (var handler in handlers)
            {
                context = new MessageContext(instance.Id, handler.GetType().Name, null, executionContext, context);

                if (handler is IUseMessageContext)
                {
                    ((IUseMessageContext)handler).UseContext(context);
                }

                var node = this.RootNode.Find(instance);

                await _system.ActorSelection("user/" + node.Path).Ask(new AkkaRequest(instance, context));
            }
        }

        public async Task Publish(IEnumerable<IEvent> instance, MessageContext context = null)
        {
            foreach (var @event in instance)
            {
                await this.Publish(@event, context);
            }
        }

        public async Task<MessageResult> Send(string path, ICommand instance, MessageContext context = null, TimeSpan? timeout = null)
        {
            var node = this.RootNode.Find(path);

            return await this.Execute(instance, context, timeout, node);
        }

        public Task<MessageResult> Send(string path, string command, MessageContext context = null, TimeSpan? timeout = null)
        {
            var node = this.RootNode.Find(path);
            var instance = (ICommand)JsonConvert.DeserializeObject(command, node.RequestType);

            return this.Execute(instance, context, timeout, node);
        }

        //public IEnumerable<Request> BuildRequests(IMessage command)
        //{
        //    var node = this.RootNode.Find(command);

        //    yield return new Request("asdf", command, new AkkaRequestHandler(_system, node));
        //}

        //public Task<MessageExecutionResult> Send(string path, string message, TimeSpan? timeout = null)
        //{
        //    var node = this.RootNode.Find(path);

        //    var command = (IMessage)JsonConvert.DeserializeObject(message, node.Type.GetRequestType());

        //    return this.Send(command, node);
        //}

        //public Task<MessageExecutionResult> Send(IMessage message, TimeSpan? timeout = null)
        //{
        //    var node = this.RootNode.Find(message);

        //    return this.Send(message, node, timeout);
        //}

        //private async Task<MessageResult> Send(IMessage message, AkkaActorNode node, TimeSpan? timeout = null, ExecutionContext context = null)
        //{
        //    context = context ?? _context.Resolve();

        //    var request = new RequestHandlerReference(_components.Resolve(node.Type));
        //    var m = new MessageContext(request, context);
        //    var result = await _system.ActorSelection("user/" + node.Path).Ask(new RequestEnvelope(new Message("none", message, request), new MessageContext(), timeout);

        //    return result as MessageResult;
        //}

        //public Task<MessageExecutionResult> Send(string path, IMessage message, TimeSpan? timeout = null)
        //{
        //    var node = this.RootNode.Find(path);

        //    return this.Send(message, node, timeout);
        //}

        //public Task<MessageExecutionResult> Send(string path, TimeSpan? timeout)
        //{
        //    var node = this.RootNode.Find(path);

        //    var command = (IMessage)JsonConvert.DeserializeObject("{}", node.Type.GetRequestType());

        //    return this.Send(command, node);
        //}

        //public void Publish<TEvent>(TEvent instance, ExecutionContext context) where TEvent : Event
        //{
        //    var node = this.RootNode.Find(instance);

        //    this.Send(instance, node, context: context);
        //}
    }
}