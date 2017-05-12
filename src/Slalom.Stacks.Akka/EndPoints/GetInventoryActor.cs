/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using Akka.Actor;
using Slalom.Stacks.Services.Inventory;

namespace Slalom.Stacks.Akka.EndPoints
{
    public class GetInventoryActor : ReceiveActor
    {
        private readonly ServiceInventory _inventory;

        public GetInventoryActor(ServiceInventory inventory)
        {
            _inventory = inventory;

            this.Receive<GetInventoryCommand>(m => { this.Sender.Tell(_inventory); });
        }
    }
}