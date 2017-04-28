using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Messaging.Messaging;
using Slalom.Stacks.Messaging.Modules;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Logging;

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

        //public static ServiceRegistry GetRegistry(this Stack instance, string path = "akka://local")
        //{
        //    if (!path.EndsWith("/"))
        //    {
        //        path += "/";
        //    }
        //    return (ServiceRegistry)instance.Container.Resolve<ActorSystem>().ActorSelection(path + "/user/_services/registry").Ask(new GetInventoryCommand(path)).Result;
        //}

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
      
        ///// <summary>
        ///// Configures the stack to use Akka.NET Messaging.
        ///// </summary>
        ///// <param name="instance">The this instance.</param>
        ///// <param name="configuration">The configuration routine.</param>
        ///// <returns>Stack.</returns>
        //public static Stack UseAkkaLogging(this Stack instance, Action<LoggingOptions> configuration = null)
        //{
        //    var options = new LoggingOptions();
        //    configuration?.Invoke(options);

        //    var system = ActorSystem.Create(options.SystemName);
        //    new AutoFacDependencyResolver(instance.Container, system);

        //    instance.Use(builder =>
        //    {
        //        builder.Register(c => new LogClient(system, options))
        //               .PreserveExistingDefaults()
        //               .SingleInstance()
        //               .As<ILogger>()
        //               .As<IRequestLog>()
        //               .As<IRequestLog>()
        //               .PreserveExistingDefaults();
        //    });

        //    return instance;
        //}

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
      hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
        }
        serialization-bindings {
            ""System.Object"" = hyperion
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

            return instance;
        }

        public static Stack Schedule(this Stack instance, TimeSpan delay, object message)
        {
            var system = instance.Container.Resolve<ActorSystem>();
            var actorSelection = system.ActorSelection("user/_services/schedule");
            system.Scheduler.ScheduleTellOnce(delay, actorSelection, message, ActorRefs.NoSender);
            return instance;
        }

        public static Stack Schedule(this Stack instance, TimeSpan delay, TimeSpan interval, object message)
        {
            var system = instance.Container.Resolve<ActorSystem>();
            var actorSelection = system.ActorSelection("user/_services/schedule");
            system.Scheduler.ScheduleTellRepeatedly(delay, interval, actorSelection, message, ActorRefs.NoSender);
            return instance;
        }
    }
}