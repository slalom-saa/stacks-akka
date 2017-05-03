using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Logging;

namespace RemoteClient
{
    public class RemoteEvent : Event
    {
        public string Message { get; }

        public RemoteEvent(string message)
        {
            this.Message = message;
        }
    }

    public class RemoteCommand
    {
    }

    [EndPoint("remote")]
    public class Remote : EndPoint<RemoteCommand, RemoteEvent>
    {
        public override async Task<RemoteEvent> ReceiveAsync(RemoteCommand command)
        {
            await Task.Delay(500);

            Console.WriteLine("1");

            return new RemoteEvent("Hello");

        }
    }

    [EndPoint("remote2")]
    public class Remote2 : EndPoint<RemoteCommand, RemoteEvent>
    {
        public override async Task<RemoteEvent> ReceiveAsync(RemoteCommand command)
        {
            await Task.Delay(250);

            Console.WriteLine("2");

            return new RemoteEvent("Hello 2");
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Remote Client";

            using (var stack = new Stack())
            {
                stack.UseSimpleConsoleLogging();

                stack.RunAkkaHost();
            }
        }
    }
}
