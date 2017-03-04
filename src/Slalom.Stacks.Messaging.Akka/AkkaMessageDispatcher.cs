using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Autofac;
using Newtonsoft.Json;
using Slalom.Stacks.Messaging.Routing;
using Slalom.Stacks.Messaging.Services;
using Slalom.Stacks.Runtime;
using Slalom.Stacks.Services;

namespace Slalom.Stacks.Messaging
{
    public class AkkaMessageDispatcher : IMessageDispatcher
    {
        private readonly ActorSystem _system;
        private IActorRef _actorRef;
        private IExecutionContext _executionContext;
        private ServiceRegistry _services;

        public AkkaMessageDispatcher(ActorSystem system, IComponentContext components)
        {
            _system = system;
            _actorRef = system.ActorOf(system.DI().Props<CommandCoordinator>(), "commands");
            _executionContext = components.Resolve<IExecutionContext>();
            _services = components.Resolve<ServiceRegistry>();
        }

        public async Task<MessageResult> Dispatch(RequestContext request, EndPoint endPoint, MessageExecutionContext parentContext)
        {
            var executionContext = _executionContext.Resolve();
            var context = new MessageExecutionContext(request, endPoint, executionContext, parentContext);

            if (endPoint.RootPath.StartsWith("akka.tcp"))
            {
                var content = request.Message.Body;
                if (!(content is String))
                {
                    content = JsonConvert.SerializeObject(content);
                }
                await _system.ActorSelection(endPoint.RootPath + "/user/_services/remote").Ask(new RemoteCall(endPoint.Path, (string)content));
            }
            else
            {
                await _actorRef.Ask(context);
            }


            return new MessageResult(context);
        }

        public bool CanDispatch(EndPoint endPoint)
        {
            return endPoint.RootPath.StartsWith("akka") || endPoint.RootPath == Service.LocalPath;
        }
    }
}
