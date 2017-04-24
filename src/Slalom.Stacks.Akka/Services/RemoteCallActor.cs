using System;
using System.Linq;
using Akka.Actor;
using Newtonsoft.Json;
using Slalom.Stacks.Services.Messaging;

namespace Slalom.Stacks.Messaging.Services
{
    public class RemoteCallActor : ReceiveActor
    {
        public RemoteCallActor(IMessageGateway messages)
        {
            this.ReceiveAsync<RemoteCall>(async m =>
            {
                var result = await messages.Send(m.Path, m.Content);

                result.Response = JsonConvert.SerializeObject(result.Response);

                this.Sender.Tell(result);
            });
        }
    }
}
