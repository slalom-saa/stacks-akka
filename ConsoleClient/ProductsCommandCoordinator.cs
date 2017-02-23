﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;
using Autofac;
using ConsoleClient.Application.Products.Add;
using ConsoleClient.Application.Products.Stock;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Messaging.Routing;
using Slalom.Stacks.Text;
using Slalom.Stacks.Reflection;

namespace ConsoleClient
{
    public class ProductsCommandCoordinator : CommandCoordinator
    {
        public ProductsCommandCoordinator(IComponentContext components)
            : base(components)
        {
        }
    }
}
