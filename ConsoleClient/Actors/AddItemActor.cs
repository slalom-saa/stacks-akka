using System.Threading.Tasks;
using Slalom.Stacks.Messaging;

namespace ConsoleClient.Actors
{
    [Path("items/add-item")]
    public class AddItemActor : UseCaseActor<GoCommand, GoEvent>
    {
        public override async Task<GoEvent> ExecuteAsync(GoCommand command)
        {
            await this.Domain.AddAsync(new Item("adf"));

            await Task.Delay(500);

            return new GoEvent();
        }

        //public override async Task<GoEvent> Executea(GoCommand command)
        //{
        //    await this.Domain.AddAsync(new Item("adf"));

        //    Console.WriteLine("Adding");
        //    return new GoEvent();
        //}
    }
}