﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Plugins;
using System;

namespace OpenTibia.Plugins.Ammunitions
{
    public class PoisonArrowAmmunitionPlugin : AmmunitionPlugin
    {
        public PoisonArrowAmmunitionPlugin(Ammunition ammunition) : base(ammunition)
        {

        }

        public override PromiseResult<bool> OnUsingAmmunition(Player player, Creature target, Item weapon, Item ammunition)
        {
            return Promise.FromResultAsBooleanTrue;
        }

        public override Promise OnUseAmmunition(Player player, Creature target, Item weapon, Item ammunition)
        {
            var formula = Formula.DistanceFormula(player.Level, player.Skills.GetClientSkillLevel(Skill.Distance), ammunition.Metadata.Attack.Value, player.Client.FightMode, weapon.Metadata.HitChance, weapon.Metadata.MaxHitChance, player.Tile.Position.ChebyshevDistance(target.Tile.Position) );

            return Context.AddCommand(new CreatureAttackCreatureCommand(player, target, 

                new DamageAttack(ammunition.Metadata.ProjectileType.Value, null, DamageType.Physical, formula.Min, formula.Max, true),
                                                                                                                                 
                new DamageCondition(SpecialCondition.Poisoned, null, DamageType.Earth, new[] { 3, 3, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, TimeSpan.FromSeconds(4) ) ) );
        }     
    }
}