﻿using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Plugins;
using System;

namespace OpenTibia.Game.CommandHandlers
{
    public class CreatureMoveTileAddingScriptingHandler : CommandHandler<CreatureMoveCommand>
    {
        public override Promise Handle(Func<Promise> next, CreatureMoveCommand command)
        {
            if (command.ToTile.Ground != null)
            {
                CreatureStepInPlugin plugin = Context.Server.Plugins.GetCreatureStepInPlugin(command.ToTile.Ground);

                if (plugin != null)
                {
                    return plugin.OnSteppingIn(command.Creature, command.ToTile).Then( (result) =>
                    {
                        if (result)
                        {
                            return Promise.Completed;
                        }

                        return next();
                    } );
                }
            }
            

            return next();
        }
    }
}