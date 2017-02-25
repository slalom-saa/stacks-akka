namespace Slalom.Stacks.Messaging
{
    /// <summary>
    /// Options for Akka.NET Messaging.
    /// </summary>
    public class MessagingOptions
    {
        internal string LogUrl { get; set; } = "akka.tcp://logging@localhost:8080/user/log";

        internal string SystemName { get; set; } = "local";

        internal bool UseLoggingClient { get; set; }

        /// <summary>
        /// Configures the stack to use the Akka.NET logging client.
        /// </summary>
        /// <param name="path">The remote path to the loggin service.</param>
        /// <returns>This instance for method chaining.</returns>
        public MessagingOptions WithLoggingClient(string path)
        {
            this.LogUrl = path;
            this.UseLoggingClient = true;
            return this;
        }

        /// <summary>
        /// Configures the stack to use the Akka.NET actor system with the specified name.
        /// </summary>
        /// <param name="name">The actor system name.</param>
        /// <returns>This instance for method chaining.</returns>
        public MessagingOptions WithName(string name)
        {
            this.SystemName = name;
            return this;
        }
    }
}