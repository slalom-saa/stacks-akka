using System;
using System.Threading.Tasks;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Runtime;

namespace ConsoleClient.Actors
{
    public class GoCommand : Command
    {
    }

    public class SendOnGo : Actor<ItemAddedEvent>
    {
        public override void Execute(ItemAddedEvent command)
        {
            ///Console.WriteLine(command.ToString());
        }
    }
}