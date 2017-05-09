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
    [EndPointHost("home/parent")]
    public class AC : EndPointHost
    {
        public override int Retries => 2;
    }


    [EndPoint("home/parent")]
    public class Parent : EndPoint
    {


        private int i = 0;
        public override void Receive()
        {
            if (i++ > 5)
            {
                throw new Exception();
            }

            this.Respond("adf");
        }

        public override void OnStart()
        {
            Console.WriteLine("Starting...");
        }
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

                    for (int i = 0; i < 10; i++)
                    {
                        var result = stack.Send("home/parent").Result;

                        Console.WriteLine(result.IsSuccessful);
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