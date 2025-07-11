﻿using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class FireBugHandler : CommandHandler<PlayerUseItemWithItemCommand>
    {
        private readonly HashSet<ushort> fireBugs;
        private readonly Dictionary<ushort, ushort> goodSugarCanes;
        private readonly Dictionary<ushort, ushort> emptyCoalBasins;

        public FireBugHandler()
        {
            fireBugs = Context.Server.Values.GetUInt16HashSet("values.items.fireBugs");
            goodSugarCanes = Context.Server.Values.GetUInt16IUnt16Dictionary("values.items.transformation.goodSugarCanes");
            emptyCoalBasins = Context.Server.Values.GetUInt16IUnt16Dictionary("values.items.transformation.emptyCoalBasins");
        }

        public override Promise Handle(Func<Promise> next, PlayerUseItemWithItemCommand command)
        {
            ushort toOpenTibiaId;

            if (fireBugs.Contains(command.Item.Metadata.OpenTibiaId) )
            {
                if (goodSugarCanes.TryGetValue(command.ToItem.Metadata.OpenTibiaId, out toOpenTibiaId) )
                {
                    if (Context.Server.Randomization.HasProbability(1.0 / 10) )
                    {
                        return Context.AddCommand(new ItemDestroyCommand(command.Item) ).Then( () =>
                        {
                            return Context.AddCommand(new ShowTextCommand(command.Player, MessageMode.MonsterSay, "Ouch!") );

                        } ).Then( () =>
                        {
                            return Context.AddCommand(new CreatureAttackCreatureCommand(null, command.Player, 
                            
                                new DamageAttack(null, MagicEffectType.ExplosionDamage, DamageType.Fire, 5, 5, false) ) );
                        } );
                    }
                    else
                    {
                        return Context.AddCommand(new ShowMagicEffectCommand(command.ToItem, MagicEffectType.FireDamage) ).Then( () =>
                        {
                            return Context.AddCommand(new ItemTransformCommand(command.ToItem, toOpenTibiaId, 1) );
                        } );
                    }
                }
                else if (emptyCoalBasins.TryGetValue(command.ToItem.Metadata.OpenTibiaId, out toOpenTibiaId) )
                { 
                    if (Context.Server.Randomization.HasProbability(1.0 / 10) )
                    {
                        return Context.AddCommand(new ItemDestroyCommand(command.Item) ).Then( () =>
                        {
                            return Context.AddCommand(new ShowTextCommand(command.Player, MessageMode.MonsterSay, "Ouch!") );

                        } ).Then( () =>
                        {
                            return Context.AddCommand(new CreatureAttackCreatureCommand(null, command.Player, 
                            
                                new DamageAttack(null, MagicEffectType.ExplosionDamage, DamageType.Fire, 5, 5, false) ) );
                        } );
                    }
                    else
                    {
                        return Context.AddCommand(new ShowMagicEffectCommand(command.ToItem, MagicEffectType.FireDamage) ).Then( () =>
                        {
                            return Context.AddCommand(new ItemTransformCommand(command.ToItem, toOpenTibiaId, 1) );
                        } );
                    }
                }
            }

            return next();
        }
    }
}