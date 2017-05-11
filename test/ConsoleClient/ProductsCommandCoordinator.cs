using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Autofac;
using ConsoleClient.Application.Products.Add;
using ConsoleClient.Application.Products.Stock;
using Slalom.Stacks.Akka.Messaging;
using Slalom.Stacks.Text;
using Slalom.Stacks.Reflection;

namespace ConsoleClient
{
    public class ProductsCommandCoordinator : CommandCoordinator
    {
        public string Content { get; set; }

        public ProductsCommandCoordinator(IComponentContext components)
            : base(components)
        {
        }
    }
}
