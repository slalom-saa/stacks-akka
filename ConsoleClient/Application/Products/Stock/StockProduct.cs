using System;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;

namespace ConsoleClient.Application.Products.Stock
{
    [EndPoint("products/stock-product")]
    public class StockProduct : EndPoint<StockProductCommand>
    {
        public override async Task ReceiveAsync(StockProductCommand command)
        {
            await Task.Delay(500);
        }
    }
}
