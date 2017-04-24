using System;
using System.Linq;

namespace Slalom.Stacks.Messaging.Services
{
    public class GetInventoryCommand
    {
        public string RemotePath { get; }

        public GetInventoryCommand(string remotePath)
        {
            this.RemotePath = remotePath;
        }
    }
}
