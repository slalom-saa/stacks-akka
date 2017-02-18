using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Runtime;

namespace Slalom.Stacks.Messaging.Routing
{
    public class CommandEnvelop
    {
        public object Command { get; }
        public ExecutionContext Context { get; }

        public CommandEnvelop(object command, ExecutionContext context)
        {
            this.Command = command;
            this.Context = context;
        }
    }
}
