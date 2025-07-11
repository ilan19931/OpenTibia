﻿using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class StuffedDragonHandler : CommandHandler<PlayerUseItemCommand>
    {
        private readonly HashSet<ushort> stuffedDragons;

        public StuffedDragonHandler()
        {
            stuffedDragons = Context.Server.Values.GetUInt16HashSet("values.items.stuffedDragons");
        }

        private static List<string> sounds = new List<string>() { "Fchhhhhh!", "Zchhhhhh!", "Grooaaaaar*cough*", "Aaa... CHOO!", "You... will.... burn!!" };

        public override Promise Handle(Func<Promise> next, PlayerUseItemCommand command)
        {
            if (stuffedDragons.Contains(command.Item.Metadata.OpenTibiaId) )
            {                  
                int value = Context.Server.Randomization.Take(0, sounds.Count - 1);

                return Context.AddCommand(new PlayerAchievementCommand(command.Player, AchievementConstants.INeedAHugStuffedDragon, new[] { AchievementConstants.INeedAHugPandaTeddy, AchievementConstants.INeedAHugStuffedDragon, AchievementConstants.INeedAHugBabySealDoll, AchievementConstants.INeedAHugSantaDoll }, "I Need a Hug") ).Then( () =>
                {
                    return Context.AddCommand(new ShowTextCommand(command.Player, MessageMode.MonsterSay, sounds[value] ) );

                } ).Then( () =>
                {
                    if (value == sounds.Count - 1)
                    {
                        return Context.AddCommand(new CreatureAttackCreatureCommand(null, command.Player, 
                            
                            new DamageAttack(null, MagicEffectType.ExplosionDamage, DamageType.Fire, 1, 1, false) ) );
                    }

                    return Promise.Completed;
                } );
            }

            return next();
        }
    }
}