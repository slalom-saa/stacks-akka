using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Messaging;

namespace ConsoleClient.Actors
{
    public class SearchCommand : Command
    {
    }

    [Path("items/search")]
    public class SearchActor : UseCaseActor<SearchCommand, string>
    {
        public override async Task<string> ExecuteAsync(SearchCommand command)
        {
            await Task.Delay(500);

            return "search";
        }
    }
}
