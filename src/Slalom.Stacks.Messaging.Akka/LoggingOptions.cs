namespace Slalom.Stacks.Messaging
{
    /// <summary>
    /// Options for Akka.NET Logging.
    /// </summary>
    public class LoggingOptions : MessagingOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingOptions"/> class.
        /// </summary>
        public LoggingOptions()
        {
            this.SystemName = "log-client";
        }

        internal string LogUrl { get; set; } = "akka.tcp://logging@localhost:8080/user/log";

        /// <summary>
        /// Configures the stack to use the Akka.NET actor system with the specified name.
        /// </summary>
        /// <param name="name">The actor system name.</param>
        /// <returns>This instance for method chaining.</returns>
        public new LoggingOptions WithName(string name)
        {
            base.WithName(name);

            return this;
        }

        /// <summary>
        /// Configures the stack to use the specified URL.
        /// </summary>
        /// <param name="url">The URL to use.</param>
        /// <returns>This instance for method chaining.</returns>
        public LoggingOptions WithUrl(string url)
        {
            this.LogUrl = url;
            return this;
        }
    }
}