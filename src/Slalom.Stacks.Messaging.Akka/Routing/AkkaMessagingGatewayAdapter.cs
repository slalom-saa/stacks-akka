using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.DI.Core;
using Autofac;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Slalom.Stacks.Messaging.Logging;
using Slalom.Stacks.Messaging.Services;
using Slalom.Stacks.Runtime;
using Slalom.Stacks.Services;

namespace Slalom.Stacks.Messaging.Routing
{
    /// <summary>
    /// An Akka.NET <see cref="IMessageGatewayAdapter"/> implementation.
    /// </summary>
    public class AkkaMessagingGatewayAdapter : IMessageGatewayAdapter
    {
        private readonly IComponentContext _components;
        private readonly ActorSystem _system;
        private readonly IActorRef _actorRef;
        private readonly IExecutionContext _executionContext;
        private readonly ServiceRegistry _registry;
        private readonly IRequestContext _requestContext;
        private readonly IEnumerable<IRequestStore> _requests;

        /// <summary>
        /// Initializes a new instance of the <see cref="AkkaMessagingGatewayAdapter"/> class.
        /// </summary>
        /// <param name="system">The actor system.</param>
        /// <param name="components">The configured components.</param>
        public AkkaMessagingGatewayAdapter(ActorSystem system, IComponentContext components)
        {
            _system = system;
            _components = components;
            _registry = _components.Resolve<ServiceRegistry>();

            _actorRef = system.ActorOf(system.DI().Props<CommandCoordinator>(), "commands");
            _executionContext = _components.Resolve<IExecutionContext>();
            _requests = components.ResolveAll<IRequestStore>();
            _requestContext = components.Resolve<IRequestContext>();

            var listener = system.ActorOf(system.DI().Props<EventStreamListener>());
            system.EventStream.Subscribe(listener, typeof(AkkaRequest));
        }

        /// <inheritdoc />
        public Task Publish(IEvent instance, MessageExecutionContext parentContext = null)
        {
            var request = _requestContext.Resolve(null, instance, parentContext?.RequestContext);
            _requests.ToList().ForEach(async e => await e.Append(new RequestEntry(request)));
            var executionContext = _executionContext.Resolve();

            var entries = _registry.Find(instance).ToList();
            foreach (var entry in entries)
            {
                var context = new MessageExecutionContext(request, entry, executionContext, parentContext);

                _system.EventStream.Publish(new AkkaRequest(instance, context));
            }
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public async Task Publish(IEnumerable<IEvent> instance, MessageExecutionContext context = null)
        {
            foreach (var @event in instance)
            {
                await this.Publish(@event, context);
            }
        }

        /// <inheritdoc />
        public async Task<MessageResult> Send(ICommand instance, MessageExecutionContext parentContext = null, TimeSpan? timeout = null)
        {
            var request = _requestContext.Resolve(null, instance, parentContext?.RequestContext);
            _requests.ToList().ForEach(async e => await e.Append(new RequestEntry(request)));

            var entries = _registry.Find(instance).ToList();
            if (entries.Count() != 1)
            {
                throw new Exception("TBD");
            }
            var entry = entries.First();

            var executionContext = _executionContext.Resolve();

            var context = new MessageExecutionContext(request, entry, executionContext, parentContext);

            await _actorRef.Ask(new AkkaRequest(instance, context), timeout);

            return new MessageResult(context);
        }

        /// <inheritdoc />
        public async Task<MessageResult> Send(string path, ICommand instance, MessageExecutionContext parentContext = null, TimeSpan? timeout = null)
        {
            var request = _requestContext.Resolve(null, instance, parentContext?.RequestContext);
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

        /// <inheritdoc />
        public async Task<MessageResult> Send(string path, string command, MessageExecutionContext parentContext = null, TimeSpan? timeout = null)
        {
            var entry = _registry.Find(path);
            if (entry == null)
            {
                throw new Exception("TBD");
            }
            if (!entry.IsLocal)
            {
                var result = await _system.ActorSelection(entry.RootPath + "/user/_services/remote").Ask(new RemoteCall(entry.Path, command));

                return result as MessageResult;
            }
            
            if (String.IsNullOrWhiteSpace(command))
            {
                command = "{}";
            }

            var instance = (ICommand)JsonConvert.DeserializeObject(command, Type.GetType(entry.RequestType));

            var request = _requestContext.Resolve(null, instance, parentContext?.RequestContext);
            _requests.ToList().ForEach(async e => await e.Append(new RequestEntry(request)));

            var executionContext = _components.Resolve<IExecutionContext>().Resolve();

            var context = new MessageExecutionContext(request, entry, executionContext, parentContext);

            await _actorRef.Ask(new AkkaRequest(instance, context), timeout);

            return new MessageResult(context);
        }
    }
}