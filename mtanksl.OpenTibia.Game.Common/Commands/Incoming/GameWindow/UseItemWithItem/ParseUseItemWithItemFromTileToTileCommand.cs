﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Common;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class ParseUseItemWithItemFromTileToTileCommand : ParseUseItemWithItemCommand
    {
        public ParseUseItemWithItemFromTileToTileCommand(Player player, Position fromPosition, byte fromIndex, ushort fromTibiaId, Position toPosition, byte toIndex, ushort toTibiaId) : base(player)
        {
            FromPosition = fromPosition;

            FromIndex = fromIndex;

            FromTibiaId = fromTibiaId;

            ToPosition = toPosition;

            ToIndex = toIndex;

            ToTibiaId = toTibiaId;
        }
        
        public Position FromPosition { get; set; }

        public byte FromIndex { get; set; }

        public ushort FromTibiaId { get; set; }

        public Position ToPosition { get; set; }

        public byte ToIndex { get; set; }

        public ushort ToTibiaId { get; set; }

        public override Promise Execute()
        {
            Tile fromTile = Context.Server.Map.GetTile(FromPosition);

            if (fromTile != null)
            {
                if (Player.Tile.Position.CanHearSay(fromTile.Position) )
                {
                    Item fromItem = Player.Client.GetContent(fromTile, FromIndex) as Item;

                    if (fromItem != null && fromItem.Metadata.TibiaId == FromTibiaId)
                    {
                        Tile toTile = Context.Server.Map.GetTile(ToPosition);

                        if (toTile != null)
                        {
                            if (Player.Tile.Position.Z == toTile.Position.Z)
                            {
                                if (Player.Tile.Position.CanHearSay(toTile.Position) )
                                {
                                    switch (Player.Client.GetContent(toTile, ToIndex) )
                                    {
                                        case Item toItem:

                                            if (toItem.Metadata.TibiaId == ToTibiaId)
                                            {
                                                if ( IsUseable(fromItem) )
                                                {
                                                    if ( !Player.Tile.Position.IsNextTo(fromTile.Position) )
                                                    {
                                                        return Context.AddCommand(new PlayerWalkToCommand(Player, fromTile) ).Then( () =>
                                                        {
                                                            return Execute();
                                                        } );
                                                    }

                                                    return Context.AddCommand(new PlayerUseItemWithItemCommand(Player, fromItem, toItem) );
                                                }
                                            }

                                            break;

                                        case Creature toCreature:

                                            if (ToTibiaId == 99)
                                            {
                                                if ( IsUseable(fromItem) )
                                                {
                                                    if ( !Player.Tile.Position.IsNextTo(fromTile.Position) )
                                                    {
                                                        return Context.AddCommand(new PlayerWalkToCommand(Player, fromTile) ).Then( () =>
                                                        {
                                                            return Execute();
                                                        } );
                                                    }

                                                    return Context.AddCommand(new PlayerUseItemWithCreatureCommand(Player, fromItem, toCreature) );
                                                }
                                            }

                                            break;
                                    }
                                }
                            }
                            else
                            {
                                if (Player.Tile.Position.Z > toTile.Position.Z)
                                {
                                    Context.AddPacket(Player, new ShowWindowTextOutgoingPacket(MessageMode.Failure, Constants.FirstGoUpstairs) );
                                }
                                else
                                {
                                    Context.AddPacket(Player, new ShowWindowTextOutgoingPacket(MessageMode.Failure, Constants.FirstGoDownstairs) );
                                }
                            }
                        }
                    }
                }
            }

            return Promise.Break;
        }
    }
}