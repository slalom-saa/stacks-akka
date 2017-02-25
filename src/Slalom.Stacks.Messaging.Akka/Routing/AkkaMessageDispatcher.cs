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
using Slalom.Stacks.Messaging.Logging;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Runtime;

namespace Slalom.Stacks.Messaging.Routing
{
    public class EventStreamListener : ReceiveActor
    {
        private readonly IComponentContext _components;
        private LocalRegistry _registry;

        public EventStreamListener(IComponentContext components)
        {
            _components = components;
            _registry = components.Resolve<LocalRegistry>();

            this.ReceiveAsync<AkkaRequest>(this.Execute);
        }

        private async Task Execute(AkkaRequest arg)
        {
            foreach (var entry in _registry.Find(arg.Message))
            {
                var handler = _components.Resolve(entry.Type);
                if (handler is IUseMessageContext)
                {
                    ((IUseMessageContext)handler).UseContext(arg.Context);
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
        private IExecutionContext _executionContextResolver;
        private LocalRegistry _registry;
        private IRequestContext _requestContext;
        private IEnumerable<IRequestStore> _requests;

        public AkkaActorNode RootNode { get; private set; }

        public AkkaMessageDispatcher(ActorSystem system, IComponentContext components)
        {
            _system = system;
            _components = components;
            _registry = _components.Resolve<LocalRegistry>();

            _actorRef = system.ActorOf(system.DI().Props<CommandCoordinator>(), "commands");
            _executionContextResolver = _components.Resolve<IExecutionContext>();
            _requests = components.ResolveAll<IRequestStore>();

            _requestContext = components.Resolve<IRequestContext>();

            var listener = system.ActorOf(system.DI().Props<EventStreamListener>());
            system.EventStream.Subscribe(listener, typeof(AkkaRequest));
        }

        public async Task<MessageResult> Send(ICommand instance, MessageExecutionContext parentContext = null, TimeSpan? timeout = null)
        {
            var request = _requestContext.Resolve(instance.CommandName, null, instance, parentContext?.Request);
            _requests.ToList().ForEach(async e => await e.Append(new RequestEntry(request)));

            var entries = _registry.Find(instance).ToList();
            if (entries.Count() != 1)
            {
                throw new Exception("TBD");
            }

            var executionContext = _components.Resolve<IExecutionContext>().Resolve();

            var context = new MessageExecutionContext(request, entries.First(), executionContext, parentContext);

            await _actorRef.Ask(new AkkaRequest(instance, context), timeout);

            return new MessageResult(context);
        }

        public async Task Publish(IEvent instance, MessageExecutionContext context = null)
        {
            //context = new MessageExecutionContext(instance.Id, instance.EventName, null, _executionContextResolver.Resolve(), context);

            //_system.EventStream.Publish(new AkkaRequest(instance, context));
        }

        public async Task Publish(IEnumerable<IEvent> instance, MessageExecutionContext context = null)
        {
            foreach (var @event in instance)
            {
                await this.Publish(@event, context);
            }
        }

        public async Task<MessageResult> Send(string path, ICommand instance, MessageExecutionContext parentContext = null, TimeSpan? timeout = null)
        {
            var request = _requestContext.Resolve(instance.CommandName, null, instance, parentContext?.Request);
            _requests.ToList().ForEach(async e => await e.Append(new RequestEntry(request)));

            var entry = _registry.Find(path);
            if (entry == null)
            {
                throw new Exception("TBD");
            }

            var executionContext = _components.Resolve<IExecutionContext>().Resolve();

            var context = new MessageExecutionContext(request, entry, executionContext, parentContext);

            await _actorRef.Ask(new AkkaRequest(instance, context), timeout);

            return new MessageResult(context);
        }

        public Task<MessageResult> Send(string path, string command, MessageExecutionContext context = null, TimeSpan? timeout = null)
        {
            throw new NotImplementedException();
        }
    }
}