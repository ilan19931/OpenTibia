﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Plugins;

namespace OpenTibia.Plugins.Ammunitions
{
    public class BurstArrowAmmunitionPlugin : AmmunitionPlugin
    {
        public BurstArrowAmmunitionPlugin(Ammunition ammunition) : base(ammunition)
        {

        }

        public override PromiseResult<bool> OnUsingAmmunition(Player player, Creature target, Item weapon, Item ammunition)
        {
            if (player.Combat.GetSkullIcon(null) == SkullIcon.Black)
            {
                return Promise.FromResultAsBooleanFalse;
            }

            return Promise.FromResultAsBooleanTrue;
        }

        public override Promise OnUseAmmunition(Player player, Creature target, Item weapon, Item ammunition)
        {
            var formula = Formula.DistanceFormula(player.Level, player.Skills.GetClientSkillLevel(Skill.Distance), ammunition.Metadata.Attack.Value, player.Client.FightMode);

            Offset[] area = new Offset[]
            {
                new Offset(-1, -1), new Offset(0, -1), new Offset(1, -1),
                new Offset(-1, 0) , new Offset(0, 0) , new Offset(1, 0),
                new Offset(-1, 1) , new Offset(0, 1) , new Offset(1, 1)
            };

            return Context.AddCommand(new CreatureAttackAreaCommand(player, false, target.Tile.Position, area, ammunition.Metadata.ProjectileType.Value, MagicEffectType.FireArea,

                new DamageAttack(null, null, DamageType.Physical, formula.Min, formula.Max, true) ) );
        }     
    }
}