﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class HouseKickHandler : CommandHandler<PlayerSayCommand>
    {
        public override Promise Handle(Func<Promise> next, PlayerSayCommand command)
        {
            if (command.Message.StartsWith("alana sio ") )
            {
                List<string> parameters = command.Parameters(10);

                if (parameters.Count == 1)
                {
                    string name = parameters[0];

                    Player observer = Context.Server.GameObjects.GetPlayerByName(name);

                    if (observer != null)
                    {
                        if (command.Player.Tile is HouseTile houseTile1 && observer.Tile is HouseTile houseTile2 && houseTile1.House == houseTile2.House && (houseTile1.House.IsOwner(command.Player.Name) || houseTile1.House.IsSubOwner(command.Player.Name) || observer == command.Player) )
                        {
                            Tile toTile = Context.Server.Map.GetTile(houseTile1.House.Entry);

                            if (toTile != null)
                            {
                                Tile fromTile = observer.Tile;

                                return Context.AddCommand(new CreatureMoveCommand(observer, toTile) ).Then( () =>
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
            }

            return next();
        }
    }
}