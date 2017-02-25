using System;
using System.Linq;

namespace Slalom.Stacks.Messaging.Logging
{
    /// <summary>
    /// A message that is passed from the Akka.NET logging client the logging service.
    /// </summary>
    /// <seealso cref="Slalom.Stacks.Messaging.Message" />
    public class LogMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="template">The template.</param>
        /// <param name="properties">The properties.</param>
        public LogMessage(LogSeverity severity, Exception exception, string template, object[] properties)
        {
            this.Severity = severity;
            this.Exception = exception;
            this.Template = template;
            this.Properties = properties.Select(e => Convert.ToString(e)).ToArray();
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public object[] Properties { get; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>The severity.</value>
        public LogSeverity Severity { get; }

        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <value>The template.</value>
        public string Template { get; }
    }
}