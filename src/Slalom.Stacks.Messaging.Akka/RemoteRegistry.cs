using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slalom.Stacks.Messaging
{
    public class RemoteEntry
    {
        public string Path { get; set; }

        public string Root { get; }

        public List<RemoteEntry> Children { get; set; } = new List<RemoteEntry>();

        public RemoteEntry(string path, string root)
        {
            this.Path = path;
            this.Root = root;
        }

        public RemoteEntry Add(string path, string root)
        {
            var current = new RemoteEntry(path, root);
            this.Children.Add(current);
            return current;
        }

        public RemoteEntry Find(string path)
        {
            if (this.Path == path)
            {
                return this;
            }
            foreach (var child in this.Children)
            {
                var target = child.Find(path);
                if (target != null)
                {
                    return target;
                }
            }
            return null;
        }
    }

    public class RemoteRegistry
    {
        public RemoteEntry Root { get; set; }

        public RemoteRegistry(string path, LocalRegistry local)
        {
            this.Root = new RemoteEntry(path, null);

            this.Build(this.Root, local.Root);
        }

        public void Build(RemoteEntry current, RegistryEntry entry)
        {
            foreach (var child in entry.Children)
            {
                var t = current.Add(child.Path, this.Root.Path);
                this.Build(t, child);
            }
        }

        public RemoteEntry Find(string path)
        {
            return this.Root.Find(path);
        }
    }
}
