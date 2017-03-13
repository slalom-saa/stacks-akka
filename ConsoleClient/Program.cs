﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Akka.Actor;
using Akka.Configuration;
using Akka.Util;
using Autofac;
using ConsoleClient.Application.Products.Add;
using ConsoleClient.Domain.Products;
using Newtonsoft.Json;
using Slalom.Stacks;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Messaging.Application;
using Slalom.Stacks.Messaging.Logging;
using Slalom.Stacks.Messaging.Routing;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;
using Slalom.Stacks.Text;

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

                    for (int i = 0; i < 100; i++)
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
