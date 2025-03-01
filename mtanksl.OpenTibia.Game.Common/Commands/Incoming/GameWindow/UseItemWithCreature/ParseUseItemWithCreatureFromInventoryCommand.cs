﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Common;

namespace OpenTibia.Game.Commands
{
    public class ParseUseItemWithCreatureFromInventoryCommand : ParseUseItemWithCreatureCommand
    {
        public ParseUseItemWithCreatureFromInventoryCommand(Player player, byte fromSlot, ushort tibiaId, uint toCreatureId) : base(player)
        {
            FromSlot = fromSlot;

            TibiaId = tibiaId;

            ToCreatureId = toCreatureId;
        }

        public byte FromSlot { get; set; }

        public ushort TibiaId { get; set; }

        public uint ToCreatureId { get; set; }

        public override Promise Execute()
        {
            Inventory fromInventory = Player.Inventory;

            Item fromItem = fromInventory.GetContent(FromSlot) as Item;

            if (fromItem != null && fromItem.Metadata.TibiaId == TibiaId)
            {
                Creature toCreature = Context.Server.GameObjects.GetCreature(ToCreatureId);

                if (toCreature != null)
                {
                    if (Player.Tile.Position.CanHearSay(toCreature.Tile.Position) )
                    {
                        if ( IsUseable(fromItem) )
                        {
                            return Context.AddCommand(new PlayerUseItemWithCreatureCommand(Player, fromItem, toCreature) );
                        }
                    }
                }
            }

            return Promise.Break;
        }
    }
}