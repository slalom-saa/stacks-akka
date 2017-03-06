using System;
using System.Collections.Generic;
using Akka.Actor;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Messaging.Persistence;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.Messaging.Logging
{
    /// <summary>
    /// A logging client that sends traces, requests and responses to a remote actor.
    /// </summary>
    /// <seealso cref="Slalom.Stacks.Logging.ILogger" />
    public class LogClient : ILogger, IRequestStore, IResponseStore
    {
        private readonly LoggingOptions _options;
        private readonly ActorSystem _system;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogClient" /> class.
        /// </summary>
        /// <param name="system">The configured actor system.</param>
        /// <param name="options">The configured options.</param>
        public LogClient(ActorSystem system, LoggingOptions options)
        {
            Argument.NotNull(system, nameof(system));
            Argument.NotNull(options, nameof(options));

            _system = system;
            _options = options;
        }

        /// <inheritdoc />
        public Task Append(RequestEntry entry)
        {
            return _system.ActorSelection(_options.LogUrl).Ask(entry);
        }

        /// <inheritdoc />
        public Task Append(ResponseEntry entry)
        {
            return _system.ActorSelection(_options.LogUrl).Ask(entry);
        }

        /// <inheritdoc />
        public void Debug(Exception exception, string template, params object[] properties)
        {
            _system.ActorSelection(_options.LogUrl).Tell(new LogMessage(LogSeverity.Debug, exception, template, properties));
        }

        /// <inheritdoc />
        public void Debug(string template, params object[] properties)
        {
            _system.ActorSelection(_options.LogUrl).Tell(new LogMessage(LogSeverity.Debug, null, template, properties));
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public void Error(Exception exception, string template, params object[] properties)
        {
            _system.ActorSelection(_options.LogUrl).Tell(new LogMessage(LogSeverity.Error, exception, template, properties));
        }

        /// <inheritdoc />
        public void Error(string template, params object[] properties)
        {
            _system.ActorSelection(_options.LogUrl).Tell(new LogMessage(LogSeverity.Error, null, template, properties));
        }

        /// <inheritdoc />
        public void Fatal(Exception exception, string template, params object[] properties)
        {
            _system.ActorSelection(_options.LogUrl).Tell(new LogMessage(LogSeverity.Fatal, exception, template, properties));
        }

        /// <inheritdoc />
        public void Fatal(string template, params object[] properties)
        {
            _system.ActorSelection(_options.LogUrl).Tell(new LogMessage(LogSeverity.Fatal, null, template, properties));
        }

        /// <inheritdoc />
        public void Information(Exception exception, string template, params object[] properties)
        {
            _system.ActorSelection(_options.LogUrl).Tell(new LogMessage(LogSeverity.Information, exception, template, properties));
        }

        /// <inheritdoc />
        public void Information(string template, params object[] properties)
        {
            _system.ActorSelection(_options.LogUrl).Tell(new LogMessage(LogSeverity.Information, null, template, properties));
        }

        /// <inheritdoc />
        public void Verbose(Exception exception, string template, params object[] properties)
        {
            _system.ActorSelection(_options.LogUrl).Tell(new LogMessage(LogSeverity.Verbose, exception, template, properties));
        }

        /// <inheritdoc />
        public void Verbose(string template, params object[] properties)
        {
            _system.ActorSelection(_options.LogUrl).Tell(new LogMessage(LogSeverity.Verbose, null, template, properties));
        }

        /// <inheritdoc />
        public void Warning(Exception exception, string template, params object[] properties)
        {
            _system.ActorSelection(_options.LogUrl).Tell(new LogMessage(LogSeverity.Warning, exception, template, properties));
        }

        /// <inheritdoc />
        public void Warning(string template, params object[] properties)
        {
            _system.ActorSelection(_options.LogUrl).Tell(new LogMessage(LogSeverity.Warning, null, template, properties));
        }

        Task<IEnumerable<RequestEntry>> IRequestStore.GetEntries(DateTimeOffset? start, DateTimeOffset? end)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<ResponseEntry>> IResponseStore.GetEntries(DateTimeOffset? start, DateTimeOffset? end)
        {
            throw new NotImplementedException();
        }
    }
}