﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class TeleportToTownHandler : CommandHandler<PlayerSayCommand>
    {
        public override Promise Handle(Func<Promise> next, PlayerSayCommand command)
        {
            if (command.Message.StartsWith("/town ") )
            {
                List<string> parameters = command.Parameters(6);

                if (parameters.Count == 1)
                {
                    string name = parameters[0];

                    Town town = Context.Server.Map.GetTown(name);

                    if (town != null)
                    {
                        Tile toTile = Context.Server.Map.GetTile(town.Position);

                        if (toTile != null)
                        {
                            Tile fromTile = command.Player.Tile;

                            return Context.AddCommand(new CreatureMoveCommand(command.Player, toTile) ).Then( () =>
                            {
                                return Context.AddCommand(new ShowMagicEffectCommand(fromTile.Position, MagicEffectType.Puff) );

                            } ).Then( () =>
                            {
                                return Context.AddCommand(new ShowMagicEffectCommand(toTile.Position, MagicEffectType.Teleport) );
                            } );
                        }
                    }
                }

                return Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );
            }

            return next();
        }
    }
}