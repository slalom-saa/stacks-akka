using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slalom.Stacks.Messaging
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EndPointHostAttribute : Attribute
    {
        public string Path { get; }

        public EndPointHostAttribute(string path)
        {
            this.Path = path;
        }
    }
}
