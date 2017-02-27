﻿using System;
using Akka.Actor;
using Akka.DI.Core;
using Autofac;
using System.Linq;
using Akka.Routing;
using Slalom.Stacks.Messaging.Registration;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Messaging.Routing
{
    /// <summary>
    /// A default Akka.NET supervisor.
    /// </summary>
    /// <seealso cref="Akka.Actor.ReceiveActor" />
    public class CommandCoordinator : ReceiveActor
    {
        private readonly IComponentContext _components;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCoordinator"/> class.
        /// </summary>
        /// <param name="components">The configured <see cref="IComponentContext"/>.</param>
        public CommandCoordinator(IComponentContext components)
        {
            Argument.NotNull(components, nameof(components));

            _components = components;

            this.Receive<AkkaRequest>(e => this.Execute(e));
        }

        /// <summary>
        /// Gets the current path.
        /// </summary>
        /// <value>The current path.</value>
        protected string Path => this.Self.Path.ToString().Substring(this.Self.Path.ToString().IndexOf("user/commands", StringComparison.Ordinal) + 13).Trim('/');

        /// <summary>
        /// Executes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><c>true</c> if the request was successful, <c>false</c> otherwise.</returns>
        protected virtual bool Execute(AkkaRequest request)
        {
            var registry = _components.Resolve<ServiceRegistry>();
            var entries = registry.Find(request.Message);
            var types = _components.Resolve<IDiscoverTypes>();

            foreach (var entry in entries)
            {
                var name = entry.Path.Substring(this.Path.Length).Trim('/');
                if (name.Split('/').Count() > 1)
                {
                    var parent = registry.Find((this.Path + "/" + name.Split('/')[0]).Trim('/'));
                    if (Context.Child(parent.Path).Equals(ActorRefs.Nobody))
                    {
                        var full = (this.Path + "/" + parent.Path.Split('/').Last()).Trim('/');

                        var target = types.Find<CommandCoordinator>().FirstOrDefault(e => e.GetAllAttributes<PathAttribute>().Any(x => x.Path == full))
                                     ?? typeof(CommandCoordinator);

                        Context.ActorOf(Context.DI().Props(target), parent.Path.Split('/').Last());
                    }
                    Context.Child(parent.Path.Split('/').Last()).Forward(request);
                }
                else
                {
                    if (Context.Child(name).Equals(ActorRefs.Nobody))
                    {
                        var type = types.Find<ActorBase>().FirstOrDefault(e => e.GetAllAttributes<PathAttribute>().Any(x => x.Path == this.Path + "/" + name))
                                   ?? typeof(UseCaseActor<,>).MakeGenericType(Type.GetType(entry.Type), request.Message.GetType());
                        try
                        {
                            Context.ActorOf(Context.DI().Props(type).WithRouter(FromConfig.Instance), name);
                        }
                        catch
                        {
                            Context.ActorOf(Context.DI().Props(type), name);
                        }
                    }
                    Context.Child(name).Forward(request);
                }
            }

            return true;
        }

        /// <inheritdoc />
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy( //or AllForOneStrategy
                10,
                TimeSpan.FromSeconds(30),
                decider: Decider.From(x =>
                {
                    //Maybe we consider ArithmeticException to not be application critical
                    //so we just ignore the error and keep going.
                    if (x is ArithmeticException) return Directive.Resume;

                    //In all other cases, just restart the failing actor
                    return Directive.Restart;
                }));
        }
    }
}