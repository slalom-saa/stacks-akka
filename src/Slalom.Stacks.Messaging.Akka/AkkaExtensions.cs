﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Messaging.Actors;
using Slalom.Stacks.Messaging.Logging;
using Slalom.Stacks.Messaging.Routing;

// ReSharper disable ObjectCreationAsStatement

namespace Slalom.Stacks.Messaging
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

        public static RemoteRegistry GetRegistry(this Stack instance, string path = "akka.tcp://local@localhost:8080")
        {
            if (!path.EndsWith("/"))
            {
                path += "/";
            }
            return (RemoteRegistry)instance.Container.Resolve<ActorSystem>().ActorSelection(path + "/user/admin/registry").Ask(new GetRegistryCommand(path)).Result;
        }

        /// <summary>
        /// Configures the stack to use Akka.NET Messaging and runs the Akka host.
        /// </summary>
        /// <param name="instance">The this instance.</param>
        /// <param name="configuration">The configuration routine.</param>
        /// <returns>Stack.</returns>
        public static void RunAkkaHost(this Stack instance, Action<MessagingOptions> configuration = null)
        {
            instance.UseAkkaMessaging(configuration);

            instance.GetExit().Wait();
        }

        /// <summary>
        /// Configures the stack use the Akka.NET logging service.
        /// </summary>
        /// <param name="instance">The this instance.</param>
        /// <param name="configuration">The configuration routine.</param>
        /// <returns>Stack.</returns>
        public static void RunAkkaLoggingService(this Stack instance, Action<LoggingServiceOptions> configuration = null)
        {
            var options = new LoggingServiceOptions();
            configuration?.Invoke(options);

            var system = ActorSystem.Create(options.SystemName);
            new AutoFacDependencyResolver(instance.Container, system);

            instance.Use(builder =>
            {
                builder.RegisterModule(new AkkaModule(instance.Assemblies));

                builder.Register(c => system).AsSelf().SingleInstance();

                builder.RegisterType<LogService>();
            });

            system.ActorOf(system.DI().Props<LogService>(), "log");

            instance.GetExit().Wait();
        }

        /// <summary>
        /// Configures the stack to use Akka.NET Messaging.
        /// </summary>
        /// <param name="instance">The this instance.</param>
        /// <param name="configuration">The configuration routine.</param>
        /// <returns>Stack.</returns>
        public static Stack UseAkkaLogging(this Stack instance, Action<LoggingOptions> configuration = null)
        {
            var options = new LoggingOptions();
            configuration?.Invoke(options);

            var system = ActorSystem.Create(options.SystemName);
            new AutoFacDependencyResolver(instance.Container, system);

            instance.Use(builder =>
            {
                builder.Register(c => new LogClient(system, options))
                       .PreserveExistingDefaults()
                       .SingleInstance()
                       .As<ILogger>()
                       .As<IRequestStore>()
                       .As<IResponseStore>()
                       .PreserveExistingDefaults();
            });

            return instance;
        }

        /// <summary>
        /// Configures the stack to use Akka.NET Messaging.
        /// </summary>
        /// <param name="instance">The this instance.</param>
        /// <param name="configuration">The configuration routine.</param>
        /// <returns>Stack.</returns>
        public static Stack UseAkkaMessaging(this Stack instance, Action<MessagingOptions> configuration = null)
        {
            var options = new MessagingOptions();
            configuration?.Invoke(options);

            var system = ActorSystem.Create(options.SystemName);
            new AutoFacDependencyResolver(instance.Container, system);

            system.ActorOf(system.DI().Props<AdminCoordinator>(), "admin");

            instance.Use(builder =>
            {
                builder.RegisterModule(new AkkaModule(instance.Assemblies));

                builder.Register(c => system).AsSelf().SingleInstance();

                builder.Register(c => new AkkaMessagingGatewayAdapter(system, c.Resolve<IComponentContext>()))
                       .As<IMessageGatewayAdapter>()
                       .SingleInstance();
            });

            var target = new List<RemoteRegistry>();
            foreach (var remote in options.Remotes)
            {
                target.Add(instance.GetRegistry(remote));
            }
            instance.Use(builder =>
            {
                builder.Register(c => new RemoteRegistryCollection(target))
                       .AsSelf();
            });
            return instance;
        }
    }
}