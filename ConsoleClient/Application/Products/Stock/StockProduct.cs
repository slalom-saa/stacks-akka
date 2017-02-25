using System;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Messaging;

namespace ConsoleClient.Application.Products.Stock
{
    [Path("products/stock-product")]
    public class StockProduct : UseCase<StockProductCommand>
    {
        public override async Task ExecuteAsync(StockProductCommand command)
        {
            await Task.Delay(500);
        }
    }
}
