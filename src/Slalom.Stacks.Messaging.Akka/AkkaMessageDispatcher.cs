using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Autofac;
using Newtonsoft.Json;
using Slalom.Stacks.Messaging.Routing;
using Slalom.Stacks.Messaging.Services;
using Slalom.Stacks.Runtime;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;

namespace Slalom.Stacks.Messaging
{
    public class AkkaMessageDispatcher : IMessageDispatcher
    {
        private readonly ActorSystem _system;
        private IActorRef _commands;
        private IActorRef _events;

        public AkkaMessageDispatcher(ActorSystem system, IComponentContext components)
        {
            _system = system;
            _commands = system.ActorOf(system.DI().Props<CommandCoordinator>(), "commands");
            _events = system.ActorOf(system.DI().Props<EventStreamListener>(), "events");
        }

        public async Task<MessageResult> Dispatch(Request request, EndPointMetaData endPoint, ExecutionContext parentContext, TimeSpan? timeout = null)
        {
            CancellationTokenSource source;
            if (timeout.HasValue || endPoint.Timeout.HasValue)
            {
                source = new CancellationTokenSource(timeout ?? endPoint.Timeout.Value);
            }
            else
            {
                source = new CancellationTokenSource();
            }

            var context = new ExecutionContext(request, endPoint, source.Token, parentContext);

            try
            {
                if (endPoint.RootPath.StartsWith("akka.tcp"))
                {
                    var content = request.Message.Body;
                    if (!(content is String))
                    {
                        content = JsonConvert.SerializeObject(content);
                    }

                    var result = await _system.ActorSelection(endPoint.RootPath + "/user/_services/remote").Ask(new RemoteCall(endPoint.Path, (string)content), source.Token);
                    return result as MessageResult;
                }
                else
                {
                    await _commands.Ask(context, source.Token);
                }
            }
            catch (Exception exception)
            {
                context.SetException(exception);
            }

            return new MessageResult(context);
        }

        public bool CanDispatch(EndPointMetaData endPoint)
        {
            return endPoint.RootPath.StartsWith("akka") || endPoint.RootPath == ServiceHost.LocalPath;
        }

        public async Task<MessageResult> Dispatch(Request request, ExecutionContext context)
        {
            context = new ExecutionContext(request, context);

            var result = await _events.Ask(context);

            return result as MessageResult;
        }
    }
}
