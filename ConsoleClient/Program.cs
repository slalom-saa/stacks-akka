using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Autofac;
using ConsoleClient.Application.Products.Add;
using ConsoleClient.Aspects;
using ConsoleClient.Domain.Products;
using Slalom.Stacks;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Messaging.Logging;
using Slalom.Stacks.Messaging.Routing;
#pragma warning disable 4014

namespace ConsoleClient
{
    [Path("products/add")]
    public class B : UseCaseActor<AddProduct, AddProductCommand>
    {
        public B(AddProduct handler)
            : base(handler)
        {
        }

        public override int Retries => 3;

        protected override Task Execute(AkkaRequest request)
        {
            return base.Execute(request);
        }
    }

    [Path("products")]
    public class AC : CommandCoordinator
    {
        public AC(IComponentContext components) : base(components)
        {
        }

        protected override bool Execute(AkkaRequest request)
        {
            return base.Execute(request);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Press any key to halt...");
            //Task.Run(() => StartLogger());
            //Thread.Sleep(800);
            Task.Run(() => Start());
            Console.ReadLine();
        }

        private static async Task Start()
        {
            try
            {
                using (var stack = new Stack(typeof(Program)))
                {
                    //stack.UseSimpleConsoleLogging();
                    stack.UseAkkaMessaging(e =>
                    {
                        //e.WithLoggingClient("akka.tcp://logging@localhost:8080/user/log");
                    });

                    stack.UseAkkaLoggingService();

                    stack.Use(builder =>
                    {
                        // builder.RegisterType<ProductsCommandCoordinator>().As<CommandCoordinator>();
                        //   builder.RegisterType<RequestStore>().As<IRequestStore>();
                        //  builder.RegisterType<ResponseStore>().As<IResponseStore>();
                    });

                    var tasks = new List<Task>
                    {
                        stack.Send("products/add", (string)null),
                        //stack.Send("products/add", new AddProductCommand("", 20)),
                        //stack.Send("products/add", new AddProductCommand("", 20)),
                        //stack.Send("products/add", new AddProductCommand("", 20)),
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

                    Console.WriteLine("...");
                    Console.WriteLine((await stack.Domain.FindAsync<Product>()).Count());

                    await stack.GetExit();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
