using System;
using Akka.Actor;
using System.Linq;
using System.Threading.Tasks;

namespace Slalom.Stacks.Messaging.Routing
{
    public class AkkaActorHost<THandler> : ReceiveActor where THandler : IHandle
    {
        private readonly ActorSupervisor _supervisor;

        private readonly THandler _handler;

        public AkkaActorHost(THandler handler, ActorSupervisor supervisor)
        {
            _supervisor = supervisor;
            _handler = handler;

            this.ReceiveAsync<MessageEnvelope>(this.HandleCommand);
        }

        private async Task<MessageExecutionResult> HandleCommand(MessageEnvelope envelop)
        {
            var result = await _supervisor.Execute(envelop, _handler);

            this.Sender.Tell(result);

            return result;
        }
    }
}