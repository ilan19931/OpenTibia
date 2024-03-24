﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using System;
using System.Linq;

namespace OpenTibia.Game.CommandHandlers
{
    public class TeleportPlayerHandler : CommandHandler<PlayerSayCommand>
    {
        public override Promise Handle(Func<Promise> next, PlayerSayCommand command)
        {
            if (command.Message.StartsWith("/c ") && command.Player.Rank == Rank.Gamemaster)
            {
                string name = command.Message.Substring(3);

                Player observer = Context.Server.GameObjects.GetPlayers()
                    .Where(p => p.Name == name)
                    .FirstOrDefault();

                if (observer != null && observer != command.Player)
                {
                    Tile toTile = command.Player.Tile;

                    if (toTile != null)
                    {
                        return Context.AddCommand(new CreatureMoveCommand(observer, toTile) ).Then( () =>
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