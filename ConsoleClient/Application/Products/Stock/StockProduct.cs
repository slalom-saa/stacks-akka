using System;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Services;

namespace ConsoleClient.Application.Products.Stock
{
    [EndPoint("products/stock-product")]
    public class StockProduct : UseCase<StockProductCommand>
    {
        public override async Task ExecuteAsync(StockProductCommand command)
        {
            await Task.Delay(500);
        }
    }
}
