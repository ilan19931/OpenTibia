﻿using OpenTibia.Game.Commands;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class BunchOfSugarCaneHandler : CommandHandler<PlayerUseItemWithItemCommand>
    {
        private static HashSet<ushort> bunchOfSugarCanes = new HashSet<ushort>() { 5467 };

        private static Dictionary<ushort, ushort> distillingMachines = new Dictionary<ushort, ushort>()
        {
            { 5469, 5513 },
            { 5470, 5514 },
        };

        private static Dictionary<ushort, ushort> decay = new Dictionary<ushort, ushort>()
        {
            { 5513, 5469 },
            { 5514, 5470 }
        };

        public override Promise Handle(Func<Promise> next, PlayerUseItemWithItemCommand command)
        {
            ushort toOpenTibiaId;

            if (bunchOfSugarCanes.Contains(command.Item.Metadata.OpenTibiaId) && distillingMachines.TryGetValue(command.ToItem.Metadata.OpenTibiaId, out toOpenTibiaId) )
            {
                return Context.AddCommand(new ItemDecrementCommand(command.Item, 1) ).Then( () =>
                {
                    return Context.AddCommand(new ItemTransformCommand(command.ToItem, toOpenTibiaId, 1) );

                } ).Then( (item) =>
                {
                    _ = Context.AddCommand(new ItemDecayTransformCommand(item, TimeSpan.FromSeconds(10), decay[item.Metadata.OpenTibiaId], 1) );

                    return Promise.Completed;
                } );
            }

            return next();
        }
    }
}