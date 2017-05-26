/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Autofac;
using Slalom.Stacks.Services.Inventory;
using Slalom.Stacks.Services.Messaging;

namespace Slalom.Stacks.Akka.Messaging
{
    public class AkkaMessageDispatcher : IRequestRouter
    {
        private readonly ActorSystem _system;
        private readonly IActorRef _commands;

        public AkkaMessageDispatcher(ActorSystem system, IComponentContext components)
        {
            _system = system;
            _commands = system.ActorOf(system.DI().Props<CommandCoordinator>(), "commands");
        }

        public async Task<MessageResult> Route(Request request, EndPointMetaData endPoint, Services.Messaging.ExecutionContext parentContext, TimeSpan? timeout = null)
        {
            CancellationTokenSource source;
            if (timeout.HasValue || endPoint.Timeout.HasValue)
            {
                source = new CancellationTokenSource(timeout ?? endPoint.Timeout.Value);
            }
            else
            {
                source = new CancellationTokenSource();
            }

            var context = new Services.Messaging.ExecutionContext(request, endPoint, source.Token, parentContext);

            try
            {
                var result = await _commands.Ask(context, source.Token);
                return result as MessageResult;
            }
            catch (Exception exception)
            {
                context.SetException(exception);
            }

            return new MessageResult(context);
        }
    }
}