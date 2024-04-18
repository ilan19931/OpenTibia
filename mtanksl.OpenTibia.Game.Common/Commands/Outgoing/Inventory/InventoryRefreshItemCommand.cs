﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Common;
using OpenTibia.Game.Events;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class InventoryRefreshItemCommand : Command
    {
        public InventoryRefreshItemCommand(Inventory inventory, Item item)
        {
            Inventory = inventory;

            Item = item;
        }

        public Inventory Inventory { get; set; }

        public Item Item { get; set; }

        public override Promise Execute()
        {
            byte slot = (byte)Inventory.GetIndex(Item);

            Context.AddPacket(Inventory.Player, new SlotAddOutgoingPacket(slot, Item) );

            Context.AddEvent(new InventoryRefreshItemEventArgs(Inventory, Item, slot) );

            return Promise.Completed;
        }
    }
}