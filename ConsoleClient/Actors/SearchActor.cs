using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Messaging;

namespace ConsoleClient.Actors
{
    [Path("items/search")]
    public class SearchActor : Actor<SearchCommand, string>
    {
        public override async Task<string> ExecuteAsync(SearchCommand command)
        {
            await Task.Delay(500);

            return "search";
        }
    }
}
