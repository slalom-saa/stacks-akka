namespace Slalom.Stacks.Messaging.Messaging
{
    public class RemoteCall
    {
        public string Path { get; set; }

        public string Content { get; set; }

        public RemoteCall(string path, string content)
        {
            this.Path = path;
            this.Content = content;
        }
    }
}