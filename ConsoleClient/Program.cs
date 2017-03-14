using System;
using Akka.Util;
using ConsoleClient.Application.Products.Add;
using Slalom.Stacks;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Messaging.Application;
using Slalom.Stacks.Messaging.Routing;
using Slalom.Stacks.Services.Registry;

#pragma warning disable 4014

namespace ConsoleClient
{
    //[EndPoint("products/add")]
    //public class B : ServiceActor<AddProduct, AddProductCommand>
    //{
    //    public B(AddProduct handler)
    //        : base(handler)
    //    {
    //    }

    //    public override int Retries => 3;

    //}

    [EndPoint("products/add")]
    public class AC : EndPointHost<AddProduct>
    {
        public AC(AddProduct service)
            : base(service)
        {
        }

        public override int Retries => 5;
    }

    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Console.Title = "Console Client";

            try
            {
                using (var stack = new Stack(typeof(AddProduct), typeof(GetAkkaStatus)))
                {
                    stack.UseAkkaMessaging(e =>
                    {
                        //e.WithRemotes("akka.tcp://local@localhost:8081");
                    });

                    for (var i = 0; i < 100; i++)
                    {
                        StandardOutWriter.WriteLine(i.ToString());

                        var result = stack.Send(new AddProductCommand("name", 15)).Result;
                        if (!result.IsSuccessful)
                        {
                            StandardOutWriter.WriteLine(i.ToString(), ConsoleColor.DarkBlue);
                            Console.ReadKey();
                        }
                    }

                    Console.WriteLine("exit");
                    Console.ReadKey();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}