﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Plugins;

namespace OpenTibia.Plugins.Spells
{
    public class DivineMissileSpellPlugin : SpellPlugin
    {
        public DivineMissileSpellPlugin(Spell spell) : base(spell)
        {

        }

        public override PromiseResult<bool> OnCasting(Player player, Creature target, string message)
        {
            if (target == null)
            {
                return Promise.FromResultAsBooleanTrue;
            }

            if (player.Tile.Position.IsInRange(target.Tile.Position, 4) && Context.Server.Pathfinding.CanThrow(player.Tile.Position, target.Tile.Position) )
            {
                return Promise.FromResultAsBooleanTrue;
            }

            return Promise.FromResultAsBooleanFalse;
        }

        public override Promise OnCast(Player player, Creature target, string message)
        {
            var formula = Formula.GenericFormula(player.Level, player.Skills.GetSkillLevel(Skill.MagicLevel), 1.79, 11, 3, 18);

            if (target == null)
            {
                Offset[] area = new Offset[]
                {
                    new Offset(0, 1)
                };

                return Context.AddCommand(new CreatureAttackAreaCommand(player, true, player.Tile.Position, area, null, MagicEffectType.HolyDamage, 
                        
                    new SimpleAttack(null, null, DamageType.Holy, formula.Min, formula.Max) ) );
            }
            else
            {
                return Context.AddCommand(new CreatureAttackCreatureCommand(player, target,

                    new SimpleAttack(ProjectileType.HolySmall, MagicEffectType.HolyDamage, DamageType.Holy, formula.Min, formula.Max) ) );
            }
        }
    }
}