﻿using OpenTibia.Common.Objects;

namespace OpenTibia.Game.Commands
{
    public class ParseMoveItemFromContainerToContainerCommand : ParseMoveItemCommand
    {
        public ParseMoveItemFromContainerToContainerCommand(Player player, byte fromContainerId, byte fromContainerIndex, ushort tibiaId, byte toContainerId, byte toContainerIndex, byte count) : base(player)
        {
            FromContainerId = fromContainerId;

            FromContainerIndex = fromContainerIndex;

            TibiaId = tibiaId;

            ToContainerId = toContainerId;

            ToContainerIndex = toContainerIndex;

            Count = count;
        }

        public byte FromContainerId { get; set; }

        public byte FromContainerIndex { get; set; }

        public ushort TibiaId { get; set; }

        public byte ToContainerId { get; set; }

        public byte ToContainerIndex { get; set; }

        public byte Count { get; set; }

        public override Promise Execute()
        {
            Container fromContainer = Player.Client.Containers.GetContainer(FromContainerId);

            if (fromContainer != null)
            {
                Item fromItem = fromContainer.GetContent(FromContainerIndex) as Item;

                if (fromItem != null && fromItem.Metadata.TibiaId == TibiaId)
                {
                    Container toContainer = Player.Client.Containers.GetContainer(ToContainerId);

                    if (ToContainerIndex == 254)
                    {
                        toContainer = toContainer.Parent as Container;
                    }
                        
                    if (toContainer != null)
                    {
                        if (IsPossible(fromItem, toContainer) && IsPickupable(fromItem) && IsMoveable(fromItem, Count) )
                        {
                            return Context.AddCommand(new PlayerMoveItemCommand(Player, fromItem, toContainer, ToContainerIndex, Count, true) );
                        }
                    }
                }
            }

            return Promise.Break;
        }
    }
}