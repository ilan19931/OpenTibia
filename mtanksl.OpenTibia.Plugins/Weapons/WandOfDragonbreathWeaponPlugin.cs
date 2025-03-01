﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Plugins;

namespace OpenTibia.Plugins.Weapons
{
    public class WandOfDragonbreathWeaponPlugin : WeaponPlugin
    {
        public WandOfDragonbreathWeaponPlugin(Weapon weapon) : base(weapon)
        {

        }

        public override PromiseResult<bool> OnUsingWeapon(Player player, Creature target, Item weapon)
        {
            return Promise.FromResultAsBooleanTrue;
        }

        public override Promise OnUseWeapon(Player player, Creature target, Item weapon)
        {
           var formula = Formula.WandFormula(19, 6);

            return Context.AddCommand(new CreatureAttackCreatureCommand(player, target,

                new DamageAttack(weapon.Metadata.ProjectileType.Value, null, DamageType.Fire, formula.Min, formula.Max) ) );
        }
    }
}