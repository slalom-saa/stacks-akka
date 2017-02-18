using System;
using System.Threading.Tasks;
using Slalom.Stacks.Runtime;

namespace Slalom.Stacks.Messaging.Routing
{
    public class AkkaCommandCoordinator : ICommandCoordinator
    {
        private readonly AkkaRouter _network;
        private readonly IExecutionContextResolver _context;

        public AkkaCommandCoordinator(AkkaRouter network, IExecutionContextResolver context)
        {
            _network = network;
            _context = context;
        }

        public Task<CommandResult> SendAsync(ICommand command, TimeSpan? timeout = null)
        {
            return _network.Send(command, _context.Resolve());
        }

        public Task<CommandResult> SendAsync(string path, ICommand command, TimeSpan? timeout = null)
        {
            return _network.Send(path, command, _context.Resolve());
        }

        public Task<CommandResult> SendAsync(string path, string command, TimeSpan? timeout = null)
        {
            return _network.Send(path, command, _context.Resolve());
        }
    }
}