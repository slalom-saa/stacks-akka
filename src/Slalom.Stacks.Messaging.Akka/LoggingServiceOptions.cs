namespace Slalom.Stacks.Messaging
{
    /// <summary>
    /// Options for the Akka.NET logging service.
    /// </summary>
    public class LoggingServiceOptions
    {
        internal string SystemName { get; set; } = "logging";

        internal bool UseLoggingClient { get; set; }

        /// <summary>
        /// Configures the stack to use the Akka.NET actor system with the specified name.
        /// </summary>
        /// <param name="name">The actor system name.</param>
        /// <returns>This instance for method chaining.</returns>
        public LoggingServiceOptions WithName(string name)
        {
            this.SystemName = name;
            return this;
        }
    }
}