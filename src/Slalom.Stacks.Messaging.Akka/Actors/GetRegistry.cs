using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slalom.Stacks.Messaging.Actors
{
    public class GetRegistryCommand : Command
    {
        public string RemotePath { get; }

        public GetRegistryCommand(string remotePath)
        {
            this.RemotePath = remotePath;
        }
    }
}
