using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Newtonsoft.Json;

namespace Slalom.Stacks.Messaging.Actors
{
    public class RemoteCall
    {
        public string Path { get; set; }

        public string Content { get; set; }

        public RemoteCall(string path, string content)
        {
            this.Path = path;
            this.Content = content;
        }
    }

    public class RemoteCallActor : ReceiveActor
    {
        public RemoteCallActor(IMessageGatewayAdapter messages)
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
