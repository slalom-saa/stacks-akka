using System;
using System.Linq;

namespace Slalom.Stacks.Messaging.Routing
{
    internal class AkkaActorMapping
    {
        public AkkaActorMapping(string path, Type type)
        {
            this.Path = path;
            if (type.IsGenericType && type.GetInterfaces().Any(e => e == typeof(IHandle<>)))
            {
                this.Type = typeof(AkkaActorHost);
            }
            else
            {
                this.Type = type;
            }
        }

        public AkkaActorMapping()
        {
        }

        public string Path { get; set; }

        public Type Type { get; set; }

        public static implicit operator AkkaActorMapping(string value)
        {
            return new AkkaActorMapping { Path = value };
        }
    }
}