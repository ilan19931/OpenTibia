﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Commands;
using OpenTibia.Network.Packets.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenTibia.Game.CommandHandlers
{
    public class ThrowAwayContainerCloseHandler : CommandHandler<PlayerMoveItemCommand>
    {
        public override Promise Handle(Func<Promise> next, PlayerMoveItemCommand command)
        {
            if (command.Item is Container container)
            {
                HashSet<Player> isNextFrom = new HashSet<Player>();

                switch (container.Parent)
                {
                    case Tile fromTile:

                        foreach (var observer in Context.Server.Map.GetObserversOfTypePlayer(fromTile.Position) )
                        {
                            if (observer.Tile.Position.IsNextTo(fromTile.Position) )
                            {
                                isNextFrom.Add(observer);
                            }                            
                        }

                        break;

                    case Inventory fromInventory:

                        isNextFrom.Add(fromInventory.Player);

                        break;

                    case Container fromContainer:

                        switch (fromContainer.Root() )
                        {
                            case Tile fromTile:

                                foreach (var observer in Context.Server.Map.GetObserversOfTypePlayer(fromTile.Position) )
                                {
                                    if (observer.Tile.Position.IsNextTo(fromTile.Position) )
                                    {
                                        isNextFrom.Add(observer);
                                    }
                                }

                                break;

                            case Inventory fromInventory:

                                isNextFrom.Add(fromInventory.Player);

                                break;

                            case Safe fromSafe:

                                isNextFrom.Add(command.Player);

                                break;
                        }

                        break;

                    default:

                        throw new NotImplementedException();
                }

                return next().Then( () =>
                {
                    HashSet<Player> isNextTo = new HashSet<Player>();

                    switch (command.Item.Parent)
                    {
                        case Tile toTile:

                            foreach (var observer in Context.Server.Map.GetObserversOfTypePlayer(toTile.Position) )
                            {
                                if (observer.Tile.Position.IsNextTo(toTile.Position) )
                                {
                                    isNextTo.Add(observer);
                                }
                            }

                            break;

                        case Inventory toInventory:

                            isNextTo.Add(toInventory.Player);

                            break;

                        case Container toContainer:

                            switch (toContainer.Root() )
                            {
                                case Tile toTile:

                                    foreach (var observer in Context.Server.Map.GetObserversOfTypePlayer(toTile.Position) )
                                    {
                                        if (observer.Tile.Position.IsNextTo(toTile.Position) )
                                        {
                                            isNextTo.Add(observer);
                                        }
                                    }

                                    break;

                                case Inventory toInventory:

                                    isNextTo.Add(toInventory.Player);

                                    break;

                                case Safe toSafe:

                                    isNextTo.Add(command.Player);

                                    break;
                            }

                            break;

                        default:

                            throw new NotImplementedException();
                    }

                    foreach (var observer in isNextFrom.Except(isNextTo) )
                    {
                        foreach (var pair in observer.Client.Containers.GetIndexedContainers() )
                        {
                            if (pair.Value.IsContentOf(container) )
                            {
                                observer.Client.Containers.CloseContainer(pair.Key);

                                Context.AddPacket(observer.Client.Connection, new CloseContainerOutgoingPacket(pair.Key) );
                            }
                        }
                    }

                    foreach (var observer in isNextFrom.Intersect(isNextTo) )
                    {
                        foreach (var pair in observer.Client.Containers.GetIndexedContainers() )
                        {
                            if (pair.Value == container)
                            {
                                List<Item> items = new List<Item>();

                                foreach (var item in container.GetItems() )
                                {
                                    items.Add(item);
                                }

                                Context.AddPacket(observer.Client.Connection, new OpenContainerOutgoingPacket(pair.Key, container.Metadata.TibiaId, container.Metadata.Name, container.Metadata.Capacity.Value, container.Parent is Container, items) );
                            }                           
                        }
                    }

                    return Promise.Completed;
                } );
            }

            return next();
        }
    }
}