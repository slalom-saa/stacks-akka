using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slalom.Stacks.Messaging.Routing
{
    public class MessageEnvelope
    {
        public IMessage Message { get;  }

        public MessageContext Context { get; }

        public MessageEnvelope(IMessage message, MessageContext context)
        {
            this.Message = message;
            this.Context = context;
        }
    }
}
