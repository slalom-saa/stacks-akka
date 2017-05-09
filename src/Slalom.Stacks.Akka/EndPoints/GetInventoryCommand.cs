using System;
using System.Linq;

namespace Slalom.Stacks.Messaging.EndPoints
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
