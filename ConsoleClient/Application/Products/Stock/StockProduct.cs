using System;
using System.Linq;
using Slalom.Stacks.Messaging;

namespace ConsoleClient.Application.Products.Stock
{
    public class StockProduct : UseCase<StockProductCommand>
    {
        public override void Execute(StockProductCommand message)
        {
        }
    }
}
