using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slalom.Stacks.Messaging
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EndPointHostAttribute : Attribute
    {
        public string[] Paths { get; }

        public EndPointHostAttribute(params string[] paths)
        {
            this.Paths = paths;
        }
    }
}
