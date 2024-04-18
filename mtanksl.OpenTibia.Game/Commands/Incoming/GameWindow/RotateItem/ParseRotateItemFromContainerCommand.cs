﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Common;

namespace OpenTibia.Game.Commands
{
    public class ParseRotateItemFromContainerCommand : ParseRotateItemCommand
    {
        public ParseRotateItemFromContainerCommand(Player player, byte fromContainerId, byte fromContainerIndex, ushort tibiaId) : base(player)
        {
            FromContainerId = fromContainerId;

            FromContainerIndex = fromContainerIndex;

            TibiaId = tibiaId;
        }

        public byte FromContainerId { get; set; }

        public byte FromContainerIndex { get; set; }

        public ushort TibiaId { get; set; }

        public override Promise Execute()
        {
            Container fromContainer = Player.Client.Containers.GetContainer(FromContainerId);

            if (fromContainer != null)
            {
                Item fromItem = fromContainer.GetContent(FromContainerIndex) as Item;

                if (fromItem != null && fromItem.Metadata.TibiaId == TibiaId)
                {
                    if ( IsRotatable(fromItem) )
                    {
                        return Context.AddCommand(new PlayerRotateItemCommand(Player, fromItem) );
                    }
                }
            }

            return Promise.Break;
        }
    }
}