using Slalom.Stacks.Messaging;

namespace ConsoleClient.Actors
{
    public class SendOnGo : Actor<ItemAddedEvent>
    {
        public override void Execute(ItemAddedEvent command)
        {
            ///Console.WriteLine(command.ToString());
        }
    }
}