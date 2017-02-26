using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slalom.Stacks.Messaging
{
    public class RemoteRegistryCollection
    {
        private List<RemoteRegistry> _registrations = new List<RemoteRegistry>();

        public RemoteRegistryCollection(IEnumerable<RemoteRegistry> items)
        {
            _registrations.AddRange(items);
        }

        public RemoteEntry Find(string path)
        {
            foreach (var registration in _registrations)
            {
                var current = registration.Find(path);
                if (current != null)
                {
                    return current;
                }
            }
            return null;
        }
    }
}
