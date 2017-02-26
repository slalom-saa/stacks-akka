﻿using System;
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
using Newtonsoft.Json;
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
                using (var stack = new Stack())
                {
                    stack.UseAkkaMessaging(e =>
                    {
                        e.WithRemotes("akka.tcp://local@localhost:8081");
                    });

                    //var tasks = new List<Task>();
                    //for (int i = 0; i < 10; i++)
                    //{
                        stack.Send("remote").Wait();
                    //}

                    //Task.WaitAll(tasks.ToArray());



                    //stack.Container.Resolve<ActorSystem>().ActorSelection("user/admin/remote")

                    //var remote = stack.GetRegistry();

                    //Console.WriteLine(JsonConvert.SerializeObject(remote, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

                    //Console.ReadKey();

                    //var reg = stack.GetRegistry("akka.tcp://local@localhost:8081");

                    //Console.WriteLine(JsonConvert.SerializeObject(reg, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));


                    Console.WriteLine("...");

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
