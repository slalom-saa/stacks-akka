using System;
using Akka.Util;
using ConsoleClient.Application.Products.Add;
using Slalom.Stacks;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Messaging.EndPoints;
using Slalom.Stacks.Services;

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

    [EndPointHost("products/add")]
    public class AC : EndPointHost
    {
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
                    stack.UseAkka();

                    stack.Send(new AddProductCommand("here", 15)).Wait();

                    stack.Schedule(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), new AddProductCommand("name", 10));

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