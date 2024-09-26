﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;
using OpenTibia.Network.Packets.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenTibia.Game.CommandHandlers
{
    public class CloseDoorHandler : CommandHandler<PlayerUseItemCommand>
    {
        private readonly Dictionary<ushort, ushort> closeHorizontalDoors;
        private readonly Dictionary<ushort, ushort> closeVerticalDoors;

        public CloseDoorHandler()
        {
            closeHorizontalDoors = LuaScope.GetInt16Int16Dictionary(Context.Server.Values.GetValue("values.items.transformation.closeHorizontalDoors") );
            closeVerticalDoors = LuaScope.GetInt16Int16Dictionary(Context.Server.Values.GetValue("values.items.transformation.closeVerticalDoors") );
        }

        public override Promise Handle(Func<Promise> next, PlayerUseItemCommand command)
        {
            ushort toOpenTibiaId;

            if (closeHorizontalDoors.TryGetValue(command.Item.Metadata.OpenTibiaId, out toOpenTibiaId) )
            {
                if (command.Item.Parent is HouseTile houseTile && command.Item is DoorItem doorItem && !houseTile.House.CanOpenDoor(command.Player.Name, doorItem) )
                {
                    Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.YouCanNotUseThisObject) );

                    return Promise.Break;
                }

                Tile door = (Tile)command.Item.Parent;

                if (door.TopCreature == null)
                {
                    return Context.AddCommand(new ItemTransformCommand(command.Item, toOpenTibiaId, 1) );
                }
                else
                {
                    Tile south = Context.Server.Map.GetTile(door.Position.Offset(0, 1, 0) );

                    if (south != null && south.TopCreature == null)
                    {
                        return Context.AddCommand(new ItemTransformCommand(command.Item, toOpenTibiaId, 1) ).Then( (item) =>
                        {
                            List<Promise> promises = new List<Promise>();

                            foreach (var creature in door.GetCreatures().ToList() )
                            {
                                promises.Add(Context.AddCommand(new CreatureMoveCommand(creature, south) ) );
                            }

                            return Promise.WhenAll(promises.ToArray() );                                   
                        } );
                    }
                    else
                    {
                        Tile north = Context.Server.Map.GetTile(door.Position.Offset(0, -1, 0) );

                        if (north != null && north.TopCreature == null)
                        {
                            return Context.AddCommand(new ItemTransformCommand(command.Item, toOpenTibiaId, 1) ).Then( (item) =>
                            {
                                List<Promise> promises = new List<Promise>();

                                foreach (var creature in door.GetCreatures().ToList() )
                                {
                                    promises.Add(Context.AddCommand(new CreatureMoveCommand(creature, north) ) );
                                }

                                return Promise.WhenAll(promises.ToArray() );                                   
                            } );
                        }
                    }
                }
            }
            else if (closeVerticalDoors.TryGetValue(command.Item.Metadata.OpenTibiaId, out toOpenTibiaId) )
            {
                if (command.Item.Parent is HouseTile houseTile && command.Item is DoorItem doorItem && !houseTile.House.CanOpenDoor(command.Player.Name, doorItem) )
                {
                    Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.YouCanNotUseThisObject) );

                    return Promise.Break;
                }

                Tile door = (Tile)command.Item.Parent;

                if (door.TopCreature == null)
                {
                    return Context.AddCommand(new ItemTransformCommand(command.Item, toOpenTibiaId, 1) );
                }
                else
                {
                    Tile east = Context.Server.Map.GetTile(door.Position.Offset(1, 0, 0) );

                    if (east != null && east.TopCreature == null)
                    {
                        return Context.AddCommand(new ItemTransformCommand(command.Item, toOpenTibiaId, 1) ).Then( (item) =>
                        {
                            List<Promise> promises = new List<Promise>();

                            foreach (var creature in door.GetCreatures().ToList() )
                            {
                                promises.Add(Context.AddCommand(new CreatureMoveCommand(creature, east) ) );
                            }

                            return Promise.WhenAll(promises.ToArray() );
                        } );
                    }
                    else
                    {
                        Tile west = Context.Server.Map.GetTile(door.Position.Offset(-1, 0, 0) );

                        if (west != null && west.TopCreature == null)
                        {
                            return Context.AddCommand(new ItemTransformCommand(command.Item, toOpenTibiaId, 1) ).Then( (item) =>
                            {
                                List<Promise> promises = new List<Promise>();

                                foreach (var creature in door.GetCreatures().ToList() )
                                {
                                    promises.Add(Context.AddCommand(new CreatureMoveCommand(creature, west) ) );
                                }

                                return Promise.WhenAll(promises.ToArray() );
                            } );
                        }
                    }
                }
            }

            return next();
        }
    }
}