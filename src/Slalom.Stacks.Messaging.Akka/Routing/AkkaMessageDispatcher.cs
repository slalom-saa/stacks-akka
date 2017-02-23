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

            system.ActorOf(system.DI().Props<CommandCoordinator>(), "commands");
        }

        public async Task<MessageResult> Send(ICommand instance, MessageContext context = null, TimeSpan? timeout = null)
        {
            context = new MessageContext(instance.Id, instance.CommandName, null, _components.Resolve<IExecutionContextResolver>().Resolve(), context);

            await _system.ActorSelection("user/commands").Ask(new AkkaRequest(instance, context), timeout);

            return new MessageResult(context);
        }

        public async Task Publish(IEvent instance, MessageContext context = null)
        {
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