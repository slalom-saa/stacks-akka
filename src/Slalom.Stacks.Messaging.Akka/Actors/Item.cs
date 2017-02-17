using Slalom.Stacks.Domain;

namespace Slalom.Stacks.Messaging.Actors
{
    public class Item : AggregateRoot
    {
        public string Name { get; }

        public Item(string name)
        {
            this.Name = name;
        }
    }
}