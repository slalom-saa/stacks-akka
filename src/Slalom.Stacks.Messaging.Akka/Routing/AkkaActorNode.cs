using System;
using System.Collections.Generic;

namespace Slalom.Stacks.Messaging.Routing
{
    public class AkkaActorNode
    {
        public AkkaActorNode(string path, Type type = null)
        {
            this.Path = path;
            this.Type = type;
            
            this.RequestType = type.GetRequestType();
           
        }

        public List<AkkaActorNode> Nodes { get; } = new List<AkkaActorNode>();

        public string Path { get; }

        public Type Type { get; }

        public Type RequestType { get; }

        public AkkaActorNode Add(string path, Type type)
        {
            var target = new AkkaActorNode(path, type);
            this.Nodes.Add(target);
            return target;
        }

        public AkkaActorNode Find(string path)
        {
            if (this.Path == path)
            {
                return this;
            }
            foreach (var node in this.Nodes)
            {
                var result = node.Find(path);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public AkkaActorNode Find(ICommand command)
        {
            if (this.RequestType == command.GetType())
            {
                return this;
            }
            foreach (var node in this.Nodes)
            {
                var result = node.Find(command);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}