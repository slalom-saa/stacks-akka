using System;
using System.Collections.Generic;
using System.Linq;
using Slalom.Stacks.Search;
using System.Threading.Tasks;
using Akka.Actor;
using Newtonsoft.Json;
using Slalom.Stacks.Configuration;
using Autofac;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Messaging.Actors;
using Slalom.Stacks.Runtime;

namespace Slalom.Stacks.Messaging
{
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
                        stack.SendAsync("items/add-item", "{}"),
                        stack.SendAsync("items/add-item", "{}"),
                        stack.SendAsync("items/add-item", "{}"),
                        stack.SendAsync("items/add-item", "{}"),
                        stack.SendAsync("items/add-item", "{}")
                    };


                    await Task.WhenAll(tasks);

                    //system.ActorOf(system.DI().Props<DefaultActorSupervisor>(), "commands");

                    //var result = await system.ActorSelection("user/items/add-item").Ask(new GoCommand());

                    //Console.WriteLine(result);

                    Console.WriteLine((await stack.Domain.FindAsync<Item>()).Count());

                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}