﻿/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Slalom.Stacks.Akka.Messaging;
using Slalom.Stacks.Akka.Modules;
using Slalom.Stacks.Services;

// ReSharper disable ObjectCreationAsStatement

namespace Slalom.Stacks.Akka
{
    /// <summary>
    /// Extension methods for configuring the Akka.NET messaging blocks.
    /// </summary>
    public static class AkkaExtensions
    {
        /// <summary>
        /// Gets the exit task to be executed on termination.
        /// </summary>
        /// <param name="instance">The this instance.</param>
        /// <returns>Task.</returns>
        public static Task GetExit(this Stack instance)
        {
            return instance.Container.Resolve<ActorSystem>().WhenTerminated;
        }

        /// <summary>
        /// Configures the stack to use Akka.NET Messaging and runs the Akka host.
        /// </summary>
        /// <param name="instance">The this instance.</param>
        /// <param name="configuration">The configuration routine.</param>
        /// <returns>Stack.</returns>
        public static void RunAkkaHost(this Stack instance, Action<MessagingOptions> configuration = null)
        {
            instance.UseAkka(configuration);

            instance.GetExit().Wait();
        }

        /// <summary>
        /// Schedules the specified message using the specified delay.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="delay">The time to delay.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>Stack.</returns>
        public static Stack Schedule(this Stack instance, TimeSpan delay, object message)
        {
            var system = instance.Container.Resolve<ActorSystem>();
            var actorSelection = system.ActorSelection("user/_services/schedule");
            system.Scheduler.ScheduleTellOnce(delay, actorSelection, message, ActorRefs.NoSender);
            return instance;
        }

        /// <summary>
        /// Schedules the request to run repeatedly using the specified delay and interval.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="delay">The time to delay delay.</param>
        /// <param name="interval">The time between sends.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>Stack.</returns>
        public static Stack Schedule(this Stack instance, TimeSpan delay, TimeSpan interval, object message)
        {
            var system = instance.Container.Resolve<ActorSystem>();
            var actorSelection = system.ActorSelection("user/_services/schedule");
            system.Scheduler.ScheduleTellRepeatedly(delay, interval, actorSelection, message, ActorRefs.NoSender);
            return instance;
        }

        /// <summary>
        /// Configures the stack to use Akka.NET Messaging.
        /// </summary>
        /// <param name="instance">The this instance.</param>
        /// <param name="configuration">The configuration routine.</param>
        /// <returns>Stack.</returns>
        public static Stack UseAkka(this Stack instance, Action<MessagingOptions> configuration = null)
        {
            var options = new MessagingOptions();
            configuration?.Invoke(options);

            var system = ActorSystem.Create(options.SystemName, @"akka {  
            actor {              
              serializers {
                wire = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
              }
              serialization-bindings {
                ""System.Object"" = wire
              }
            }
          }");

            new AutoFacDependencyResolver(instance.Container, system);

            instance.Use(builder =>
            {
                builder.RegisterModule(new AkkaModule(instance));

                builder.Register(c => system).AsSelf().SingleInstance();
            });

            system.ActorOf(system.DI().Props<ServicesCoordinator>(), "_services");

            var registry = instance.GetServices();
            foreach (var remote in options.Remotes)
            {
                //registry.Include(instance.GetRegistry(remote));
            }

            return instance;
        }
    }
}