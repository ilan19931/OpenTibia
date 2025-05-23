﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Plugins;
using System;

namespace OpenTibia.Plugins.Weapons
{
    public class ViperStarWeaponPlugin : WeaponPlugin
    {
        public ViperStarWeaponPlugin(Weapon weapon) : base(weapon)
        {

        }

        public override PromiseResult<bool> OnUsingWeapon(Player player, Creature target, Item weapon)
        {
            return Promise.FromResultAsBooleanTrue;
        }

        public override Promise OnUseWeapon(Player player, Creature target, Item weapon)
        {
            var formula = Formula.DistanceFormula(player.Level, player.Skills.GetClientSkillLevel(Skill.Distance), weapon.Metadata.Attack.Value, player.Client.FightMode, weapon.Metadata.HitChance, weapon.Metadata.MaxHitChance, player.Tile.Position.ChebyshevDistance(target.Tile.Position) );

            return Context.AddCommand(new CreatureAttackCreatureCommand(player, target,

                new DamageAttack(weapon.Metadata.ProjectileType.Value, null, DamageType.Physical, formula.Min, formula.Max, true),

                new DamageCondition(SpecialCondition.Poisoned, null, DamageType.Earth, new[] { 2, 2, 2, 2, 1, 1, 1, 1, 1, 1 }, TimeSpan.FromSeconds(4) ) ) );
        }
    }
}