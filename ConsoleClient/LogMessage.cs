using Slalom.Stacks.Messaging;

namespace ConsoleClient
{
    public class LogMessage : Message
    {
        public string Template { get; }

        public object[] Properties { get; }

        public LogMessage(string template, object[] properties)
        {
            this.Template = template;
            this.Properties = properties;
        }
    }
}