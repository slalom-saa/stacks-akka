using System;
using System.Linq;
using Akka.Actor;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Inventory;

namespace Slalom.Stacks.Messaging.Services
{
    public class GetInventoryActor : ReceiveActor
    {
        private readonly ServiceInventory _inventory;

        public GetInventoryActor(ServiceInventory inventory)
        {
            _inventory = inventory;

            this.Receive<GetInventoryCommand>(m =>
            {
                this.Sender.Tell(_inventory);
            });
        }
    }
}
