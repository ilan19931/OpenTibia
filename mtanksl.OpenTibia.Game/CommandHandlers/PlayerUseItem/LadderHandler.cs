﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class LadderHandler : CommandHandler<PlayerUseItemCommand>
    {
        private static HashSet<ushort> ladders = new HashSet<ushort>() { 1386, 3678, 5543, 8599, 10035 };

        public override Promise Handle(Func<Promise> next, PlayerUseItemCommand command)
        {
            if (ladders.Contains(command.Item.Metadata.OpenTibiaId) )
            {
                return Context.AddCommand(new CreatureMoveCommand(command.Player, Context.Server.Map.GetTile( ( (Tile)command.Item.Parent ).Position.Offset(0, 1, -1) ) ) ).Then( () =>
                {
                    return Context.AddCommand(new CreatureUpdateDirectionCommand(command.Player, Direction.South) );
                } );
            }

            return next();
        }
    }
}