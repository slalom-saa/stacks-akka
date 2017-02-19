using Slalom.Stacks.Domain;

namespace ConsoleClient.Actors
{
    public class Item : AggregateRoot
    {
        public string Name { get; }

        public Item(string name)
        {
            this.Name = name;
            this.AddEvent(new ItemAddedEvent());
        }
    }
}