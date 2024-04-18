﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class GreatSpiritPotionHandler : CommandHandler<PlayerUseItemWithCreatureCommand>
    {
        private static HashSet<ushort> manaPotions = new HashSet<ushort>() { 8472 };

        private static ushort emptyPotionFlask = 7635;

        public override Promise Handle(Func<Promise> next, PlayerUseItemWithCreatureCommand command)
        {
            if (manaPotions.Contains(command.Item.Metadata.OpenTibiaId) && command.ToCreature is Player player)
            {
                if (player.Level < 80 || !(player.Vocation == Vocation.Paladin || player.Vocation == Vocation.RoyalPaladin) )
                {
                    return Context.AddCommand(new ShowTextCommand(player, TalkType.MonsterSay, "Only paladins of level 80 or above may drink this fluid.") );
                }

                Promise promise;

                if (Context.Current.Server.Config.GameplayInfinitePotions)
                {
                    promise = Promise.Completed;
                }
                else
                {
                    promise = Context.Current.AddCommand(new ItemDecrementCommand(command.Item, 1) ).Then( () =>
                    {
                        return Context.AddCommand(new PlayerCreateItemCommand(command.Player, emptyPotionFlask, 1) );
                    } );
                }

                return promise.Then( () =>
                {
                    return Context.AddCommand(new CreatureUpdateHealthCommand(player, player.Health + Context.Server.Randomization.Take(200, 400) ) );

                } ).Then( () =>
                {
                    return Context.AddCommand(new PlayerUpdateManaCommand(player, player.Mana + Context.Server.Randomization.Take(110, 190) ) );

                } ).Then( () =>
                {
                    return Context.AddCommand(new ShowMagicEffectCommand(player, MagicEffectType.BlueShimmer) );

                } ).Then( () =>
                {
                    return Context.AddCommand(new ShowTextCommand(player, TalkType.MonsterSay, "Aaaah...") );
                } );
            }

            return next();
        }
    }
}