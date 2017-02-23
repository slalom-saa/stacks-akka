using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConsoleClient.Application.Products.Add;
using Slalom.Stacks;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Messaging.Routing;

namespace ConsoleClient
{
    [Path("products")]
    public class ProductsCommandCoordinator : CommandCoordinator
    {
        protected override void PreStart()
        {
            base.PreStart();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Press any key to halt...");
            Start();
            Console.ReadLine();
        }

        private static async Task Start()
        {
            try
            {
                using (var stack = new Stack(typeof(Program)))
                {
                    stack.UseSimpleConsoleLogging();
                    stack.UseAkka("local");


                    var tasks = new List<Task>
                    {
                        stack.Send("products/add", new AddProductCommand("adfa", 15)),
                        stack.Send("products/add", new AddProductCommand("adfa", 15)),

                        //stack.Send("items/add-item", new AddProductCommand("adfa", 15)),
                        //stack.Send("items/add-item", new AddProductCommand("adfa", 15)),
                        //stack.Send("items/add-item", new AddProductCommand("adfa", 15))

                        //stack.Send("items/add-item", new AddProductCommand("adsf", 15)),
                        //stack.Send("items/add-item", new AddProductCommand("adsf", 15)),
                        //stack.Send("items/add-item", new AddProductCommand("adsf", 15)),
                        //stack.Send("items/add-item", new AddProductCommand("adsf", 15)),
                        //stack.Send("items/add-item", new AddProductCommand("adsf", 15))

                        stack.Send("products/search", "{}"),
                        stack.Send("products/search", "{}"),
                        stack.Send("products/search", "{}"),
                        stack.Send("products/search", "{}")
                    };

                    await Task.WhenAll(tasks);

                    //Console.WriteLine((await stack.Domain.FindAsync<Product>()).Count());

                    Console.ReadLine();
                    Console.WriteLine("....");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}