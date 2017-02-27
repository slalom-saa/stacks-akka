using System;
using System.Linq;

namespace Slalom.Stacks.Messaging.Services
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
