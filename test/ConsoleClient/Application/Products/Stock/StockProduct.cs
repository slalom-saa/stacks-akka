using System;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Services;

namespace ConsoleClient.Application.Products.Stock
{
    [EndPoint("products/stock-product")]
    public class StockProduct : EndPoint<StockProductCommand>
    {
        public override async Task ReceiveAsync(StockProductCommand instance)
        {
            await Task.Delay(500);
        }
    }
}
