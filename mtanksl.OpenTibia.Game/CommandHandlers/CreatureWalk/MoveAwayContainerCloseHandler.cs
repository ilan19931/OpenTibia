﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Commands;
using OpenTibia.Network.Packets.Outgoing;
using System;

namespace OpenTibia.Game.CommandHandlers
{
    public class MoveAwayContainerCloseHandler : CommandHandler<CreatureWalkCommand>
    {
        public override Promise Handle(Func<Promise> next, CreatureWalkCommand command)
        {
            if (command.Creature is Player player)
            {
                return next().Then( () =>
                {
                    foreach (var pair in player.Client.Containers.GetIndexedContainers() )
                    {
                        switch (pair.Value.Root() )
                        {
                            case Tile tile:

                                if ( !command.ToTile.Position.IsNextTo(tile.Position) )
                                {
                                    player.Client.Containers.CloseContainer(pair.Key);

                                    Context.AddPacket(player.Client.Connection, new CloseContainerOutgoingPacket(pair.Key) );
                                }

                                break;

                            case Safe safe:

                                player.Client.Containers.CloseContainer(pair.Key);

                                Context.AddPacket(player.Client.Connection, new CloseContainerOutgoingPacket(pair.Key) );

                                break;
                        }
                    }

                    return Promise.Completed;
                } );
            }

            return next();
        }
    }
}