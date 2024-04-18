﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class LumpOfDoughHandler : CommandHandler<PlayerUseItemWithItemCommand>
    {
        private static HashSet<ushort> lumpOfDoughs = new HashSet<ushort>() { 2693 };

        private static HashSet<ushort> ovens = new HashSet<ushort>() { 1786, 1788, 1790, 1792, 6356, 6358, 6360, 6362 };

        private static ushort bread = 2689;

        public override Promise Handle(Func<Promise> next, PlayerUseItemWithItemCommand command)
        {
            if (lumpOfDoughs.Contains(command.Item.Metadata.OpenTibiaId) && ovens.Contains(command.ToItem.Metadata.OpenTibiaId) )
            {
                return Context.AddCommand(new ItemDecrementCommand(command.Item, 1) ).Then( () =>
                {
                    return Context.AddCommand(new TileCreateItemOrIncrementCommand( (Tile)command.ToItem.Parent, bread, 1) );
                } );
            }

            return next();
        }
    }
}