namespace Slalom.Stacks.Messaging.Routing
{
    /// <summary>
    /// Contains information needed to execute a request in Akka.NET.
    /// </summary>
    public class AkkaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AkkaRequest"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        public AkkaRequest(IMessage message, MessageExecutionContext context)
        {
            this.Message = message;
            this.Context = context;
        }

        /// <summary>
        /// Gets the current context.
        /// </summary>
        /// <value>The current context.</value>
        public MessageExecutionContext Context { get; }

        /// <summary>
        /// Gets the current message.
        /// </summary>
        /// <value>The current message.</value>
        public IMessage Message { get; }
    }
}