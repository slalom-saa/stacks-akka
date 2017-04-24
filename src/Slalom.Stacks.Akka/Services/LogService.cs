using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Autofac;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Messaging.Logging;
using Slalom.Stacks.Services.Logging;
using Slalom.Stacks.Services.Messaging;

namespace Slalom.Stacks.Messaging.Services
{
    /// <summary>
    /// Default logging actor that takes traces, requests and responses.
    /// </summary>
    /// <seealso cref="Akka.Actor.ReceiveActor" />
    public class LogService : ReceiveActor
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IResponseLog> _reponses;
        private readonly IEnumerable<IRequestLog> _requests;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogService"/> class.
        /// </summary>
        /// <param name="components">The configured components.</param>
        public LogService(IComponentContext components)
        {
            _logger = components.Resolve<ILogger>();
            _requests = components.ResolveAll<IRequestLog>();
            _reponses = components.ResolveAll<IResponseLog>();

            this.Receive<LogMessage>(e => this.LogMessage(e));
            this.ReceiveAsync<Request>(this.LogRequest);
            this.ReceiveAsync<ResponseEntry>(this.LogResponse);
        }

        private void LogMessage(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Debug:
                    _logger.Debug(message.Exception, message.Template, message.Properties);
                    break;
                case LogSeverity.Error:
                    _logger.Error(message.Exception, message.Template, message.Properties);
                    break;
                case LogSeverity.Fatal:
                    _logger.Fatal(message.Exception, message.Template, message.Properties);
                    break;
                case LogSeverity.Information:
                    _logger.Information(message.Exception, message.Template, message.Properties);
                    break;
                case LogSeverity.Verbose:
                    _logger.Verbose(message.Exception, message.Template, message.Properties);
                    break;
                case LogSeverity.Warning:
                    _logger.Warning(message.Exception, message.Template, message.Properties);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task LogRequest(Request entry)
        {
            foreach (var item in _requests)
            {
                await item.Append(entry);
            }
            this.Sender.Tell("Complete");
        }

        private async Task LogResponse(ResponseEntry entry)
        {
            foreach (var item in _reponses)
            {
                await item.Append(entry);
            }
            this.Sender.Tell("Complete");
        }
    }
}