﻿using OpenTibia.Common.Objects;

namespace OpenTibia.Game.Commands
{
    public class ParseUseItemFromContainerCommand : ParseUseItemCommand
    {
        public ParseUseItemFromContainerCommand(Player player, byte fromContainerId, byte fromContainerIndex, ushort itemId, byte containerId) : base(player)
        {
            FromContainerId = fromContainerId;

            FromContainerIndex = fromContainerIndex;

            ItemId = itemId;

            ContainerId = containerId;
        }

        public byte FromContainerId { get; set; }

        public byte FromContainerIndex { get; set; }

        public ushort ItemId { get; set; }

        public byte ContainerId { get; set; }

        public override Promise Execute()
        {
            Container fromContainer = Player.Client.Containers.GetContainer(FromContainerId);

            if (fromContainer != null)
            {
                Item fromItem = fromContainer.GetContent(FromContainerIndex) as Item;

                if (fromItem != null && fromItem.Metadata.TibiaId == ItemId)
                {
                    return Context.AddCommand(new PlayerUseItemCommand(Player, fromItem, FromContainerId == ContainerId ? ContainerId : (byte?)null) );
                }
            }

            return Promise.Break;
        }
    }
}