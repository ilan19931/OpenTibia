﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using System;

namespace OpenTibia.Game.CommandHandlers
{
    public class TeleportToWaypointHandler : CommandHandler<PlayerSayCommand>
    {
        public override Promise Handle(Func<Promise> next, PlayerSayCommand command)
        {
            if (command.Message.StartsWith("/w ") && command.Player.Rank == Rank.Gamemaster)
            {
                string name = command.Message.Substring(3);

                Waypoint waypoint = Context.Server.Map.GetWaypoint(name);

                if (waypoint != null)
                {
                    Tile toTile = Context.Server.Map.GetTile(waypoint.Position);

                    if (toTile != null)
                    {
                        return Context.AddCommand(new CreatureWalkCommand(command.Player, toTile) ).Then( () =>
                        {
                            return Context.AddCommand(new ShowMagicEffectCommand(toTile.Position, MagicEffectType.Teleport) );
                        } );
                    }
                }

                return Context.AddCommand(new ShowMagicEffectCommand(command.Player.Tile.Position, MagicEffectType.Puff) );
            }

            return next();
        }
    }
}