using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Messaging;

namespace ConsoleClient.Application.Products.Search
{
    public class SearchProductsCommand : Command
    {
    }

    [Path("products/search")]
    public class SearchProducts : UseCase<SearchProductsCommand, string>
    {
        public override async Task<string> ExecuteAsync(SearchProductsCommand command)
        {
            await Task.Delay(500);

            return "adsfaf";
        }
    }
}
