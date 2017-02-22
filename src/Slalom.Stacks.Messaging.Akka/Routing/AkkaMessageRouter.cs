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
    public class AkkaMessageRouter : IMessageStream
    {
        private readonly ActorSystem _system;
        private readonly IExecutionContextResolver _context;
        private readonly IComponentContext _components;
        private IRequestRouting _routing;
        private IExecutionContextResolver _executionContext;

        public AkkaActorNode RootNode { get; private set; }

        public AkkaMessageRouter(ActorSystem system, IExecutionContextResolver context, IComponentContext components)
        {
            _system = system;
            _context = context;
            _components = components;
            _routing = _components.Resolve<IRequestRouting>();
            _executionContext = _components.Resolve<IExecutionContextResolver>();
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
            var actors = assemblies.SafelyGetTypes(typeof(IHandle<>));

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
            var requests = _routing.BuildRequests(instance).ToList();
            if (requests.Count() != 1)
            {
                throw new Exception("TBD");
            }

            context = new MessageContext(requests.Single(), _executionContext.Resolve(), context);

            //await requests.First().Recipient.Handle(instance, context);

            var node = this.RootNode.Find(instance);

            await _system.ActorSelection("user/" + node.Path).Ask(new MessageEnvelope(requests.First(), context, node));

            return new MessageResult(context);
        }

        public Task Publish(IEvent instance, MessageContext context = null)
        {
            var requests = _routing.BuildRequests(instance).ToList();
            foreach (var request in requests)
            {
                var node = this.RootNode.Find(request.Message);

                var current = new MessageContext(request, _executionContext.Resolve(), context);

                _system.ActorSelection("user/" + node.Path).Tell(new MessageEnvelope(request, current, node));
            }
            return Task.FromResult(0);
        }

        public async Task Publish(IEnumerable<IEvent> instance, MessageContext context = null)
        {
            foreach (var @event in instance)
            {
                await this.Publish(@event, context);
            }
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