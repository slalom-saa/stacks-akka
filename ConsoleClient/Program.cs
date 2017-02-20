using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Messaging;

namespace ConsoleClient
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
                    //stack.UseSimpleConsoleLogging();
                    //stack.UseAkka("local");

                    //var tasks = new List<Task>
                    //{
                    //    //stack.SendAsync("items/add-item", "{}"),
                    //    //stack.SendAsync("items/add-item", "{}"),
                    //    //stack.SendAsync("items/add-item", "{}"),
                    //    //stack.SendAsync("items/add-item", "{}"),
                    //    //stack.SendAsync("items/add-item", "{}"),
                    //    ////stack.SendAsync("items/search", "{}"),
                    //    ////stack.SendAsync("items/search", "{}"),
                    //    ////stack.SendAsync("items/search", "{}"),
                    //    ////stack.SendAsync("items/search", "{}"),
                    //    ////stack.SendAsync("items/search", "{}")
                    //};


                    //await Task.WhenAll(tasks);

                    //Console.WriteLine((await stack.Domain.FindAsync<Item>()).Count());
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
