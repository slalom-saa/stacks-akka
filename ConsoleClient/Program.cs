using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using ConsoleClient.Application.Products.Add;
using ConsoleClient.Aspects;
using Slalom.Stacks;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Messaging.Logging;
using Slalom.Stacks.Messaging.Routing;

namespace ConsoleClient
{
    [Path("products")]
    public class AC : CommandCoordinator
    {
        public AC(IComponentContext components) : base(components)
        {
        }
        protected override bool Execute(AkkaRequest request)
        {
            //Console.WriteLine(request.Message.GetType());
            return base.Execute(request);
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
                    //stack.UseSimpleConsoleLogging();
                    stack.UseAkka("local");

                    stack.Use(builder =>
                    {
                        builder.RegisterType<ProductsCommandCoordinator>().As<CommandCoordinator>();
                        builder.RegisterType<RequestStore>().As<IRequestStore>();
                        builder.RegisterType<ResponseStore>().As<IResponseStore>();
                    });

                    var tasks = new List<Task>
                    {
                        stack.Send("products/add", new AddProductCommand("", 20)),
                        stack.Send("products/add", new AddProductCommand("", 20)),
                        stack.Send("products/add", new AddProductCommand("", 20)),
                        stack.Send("products/add", new AddProductCommand("", 20)),
                        //stack.Send("products/add", new AddProductCommand("asdf", 20)),
                        //stack.Send("products/add", new AddProductCommand("asdf", 20)),
                        //stack.Send("products/add", new AddProductCommand("asdf", 20)),
                        //stack.Send("products/add", new AddProductCommand("asdf", 20)),



                        //stack.Send("items/add-item", new AddProductCommand("asdf", 20)),
                        //stack.Send("items/add-item", new AddProductCommand("asdf", 20)),

                        //stack.Send("items/add-item", new AddProductCommand("asdf", 20))

                        //stack.Send("items/add-item", new AddProductCommand("adsf", 15)),
                        //stack.Send("items/add-item", new AddProductCommand("adsf", 15)),
                        //stack.Send("items/add-item", new AddProductCommand("adsf", 15)),
                        //stack.Send("items/add-item", new AddProductCommand("adsf", 15)),
                        //stack.Send("items/add-item", new AddProductCommand("adsf", 15))
                        ////stack.SendAsync("items/search", "{}"),
                        ////stack.SendAsync("items/search", "{}"),
                        ////stack.SendAsync("items/search", "{}"),
                        ////stack.SendAsync("items/search", "{}"),
                        ////stack.SendAsync("items/search", "{}")
                    };

                    await Task.WhenAll(tasks);
                    //foreach (var item in tasks)
                    //{
                    //    Console.WriteLine(item.Exception == null);   
                    //}

                    //Console.WriteLine((await stack.Domain.FindAsync<Product>()).Count());

                    Console.WriteLine("...");
                    Console.ReadLine();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
