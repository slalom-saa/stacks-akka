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
    public class EventStreamListener : ReceiveActor
    {
        private readonly IComponentContext _context;

        public EventStreamListener(IComponentContext context)
        {
            _context = context;
            this.ReceiveAsync<AkkaRequest>(this.Execute);
        }

        private async Task Execute(AkkaRequest arg)
        {
            var handlers = _context.ResolveAll(typeof(IHandle<>).MakeGenericType(arg.Message.GetType()));
            foreach (var handler in handlers)
            {
                if (handler is IUseMessageContext)
                {
                    ((IUseMessageContext) handler).UseContext(arg.Context);
                }
                await (Task)typeof(IHandle<>).MakeGenericType(arg.Message.GetType()).GetMethod("Handle").Invoke(handler, new object[] { arg.Message });
            }
        }
    }

    public class AkkaMessageDispatcher : IMessageDispatcher
    {
        private readonly ActorSystem _system;
        private readonly IComponentContext _components;
        private IActorRef _actorRef;
        private IExecutionContextResolver _executionContextResolver;

        public AkkaActorNode RootNode { get; private set; }

        public AkkaMessageDispatcher(ActorSystem system, IComponentContext components)
        {
            _system = system;
            _components = components;

            _actorRef = system.ActorOf(system.DI().Props<CommandCoordinator>(), "commands");
            _executionContextResolver = _components.Resolve<IExecutionContextResolver>();

            var listener = system.ActorOf(system.DI().Props<EventStreamListener>());
            system.EventStream.Subscribe(listener, typeof(AkkaRequest));
        }

        public async Task<MessageResult> Send(ICommand instance, MessageContext context = null, TimeSpan? timeout = null)
        {
            context = new MessageContext(instance.Id, instance.CommandName, null, _executionContextResolver.Resolve(), context);

            await _actorRef.Ask(new AkkaRequest(instance, context), timeout);

            return new MessageResult(context);
        }

        public async Task Publish(IEvent instance, MessageContext context = null)
        {
            context = new MessageContext(instance.Id, instance.EventName, null, _executionContextResolver.Resolve(), context);

            _system.EventStream.Publish(new AkkaRequest(instance, context));
        }

        public async Task Publish(IEnumerable<IEvent> instance, MessageContext context = null)
        {
            if (instance.Any())
            {
                foreach (var @event in instance)
                {
                    await this.Publish(@event, context);
                }
            }
        }

        public async Task<MessageResult> Send(string path, ICommand instance, MessageContext context = null, TimeSpan? timeout = null)
        {
            context = new MessageContext(instance.Id, instance.CommandName, null, _executionContextResolver.Resolve(), context);

            await _actorRef.Ask(new AkkaRequest(instance, context), timeout);

            return new MessageResult(context);
        }

        public Task<MessageResult> Send(string path, string command, MessageContext context = null, TimeSpan? timeout = null)
        {
            throw new NotImplementedException();
        }
    }
}