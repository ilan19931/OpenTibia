﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class GoldCoinHandler : CommandHandler<PlayerUseItemCommand>
    {
        private readonly Dictionary<ushort, ushort> goldCoinToPlatinumCoin;

        public GoldCoinHandler()
        {
            goldCoinToPlatinumCoin = Context.Server.Values.GetUInt16IUnt16Dictionary("values.items.transformation.goldCoinToPlatinumCoin");
        }

        public override Promise Handle(Func<Promise> next, PlayerUseItemCommand command)
        {
            ushort toOpenTibiaId;

            if (goldCoinToPlatinumCoin.TryGetValue(command.Item.Metadata.OpenTibiaId, out toOpenTibiaId) && ( (StackableItem)command.Item).Count == 100 && command.Item.Parent is Tile)
            {
                return Context.AddCommand(new ShowMagicEffectCommand(command.Item, MagicEffectType.BlueShimmer) ).Then( () =>
                {
                    return Context.AddCommand(new ItemTransformCommand(command.Item, toOpenTibiaId, 1) );
                } );
            }

            return next();
        }
    }
}