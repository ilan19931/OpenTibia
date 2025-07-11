﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Common;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class ParseUseItemFromTileCommand : ParseUseItemCommand
    {
        public ParseUseItemFromTileCommand(Player player, Position fromPosition, byte fromIndex, ushort tibiaId) : base(player)
        {
            FromPosition = fromPosition;

            FromIndex = fromIndex;

            TibiaId = tibiaId;
        }

        public Position FromPosition { get; set; }

        public byte FromIndex { get; set; }

        public ushort TibiaId { get; set; }

        public override Promise Execute()
        {
            Tile fromTile = Context.Server.Map.GetTile(FromPosition);

            if (fromTile != null)
            {
                if (Player.Tile.Position.Z == fromTile.Position.Z)
                {
                    if (Player.Tile.Position.CanHearSay(fromTile.Position) )
                    {
                        Item fromItem = Player.Client.GetContent(fromTile, FromIndex) as Item;

                        if (fromItem != null && fromItem.Metadata.TibiaId == TibiaId)
                        {
                            if ( !Player.Tile.Position.IsNextTo(fromTile.Position) )
                            {
                                return Context.AddCommand(new PlayerWalkToCommand(Player, fromTile) ).Then( () =>
                                {
                                    return Execute();
                                } );
                            }

                            return Context.AddCommand(new PlayerUseItemCommand(Player, fromItem, null) );
                        }
                    }
                }
                else
                {
                    if (Player.Tile.Position.Z > fromTile.Position.Z)
                    {
                        Context.AddPacket(Player, new ShowWindowTextOutgoingPacket(MessageMode.Failure, Constants.FirstGoUpstairs) );
                    }
                    else
                    {
                        Context.AddPacket(Player, new ShowWindowTextOutgoingPacket(MessageMode.Failure, Constants.FirstGoDownstairs) );
                    }
                }
            }

            return Promise.Break;
        }
    }
}