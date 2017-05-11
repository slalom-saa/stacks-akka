﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Services;

namespace ConsoleClient.Application.Products.Search
{
    public class SearchProductsCommand
    {
    }

    [EndPoint("products/search")]
    public class SearchProducts : EndPoint<SearchProductsCommand, string>
    {
        public override async Task<string> ReceiveAsync(SearchProductsCommand command)
        {
            await Task.Delay(500);

            return "adsfaf";
        }
    }
}
