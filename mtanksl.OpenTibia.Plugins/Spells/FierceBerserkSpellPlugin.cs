﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Plugins;
using System;

namespace OpenTibia.Plugins.Spells
{
    public class FierceBerserkSpellPlugin : SpellPlugin
    {
        public FierceBerserkSpellPlugin(Spell spell) : base(spell)
        {

        }

        public override PromiseResult<bool> OnCasting(Player player, Creature target, string message, string parameter)
        {
            return Promise.FromResultAsBooleanTrue;
        }

        public override Promise OnCast(Player player, Creature target, string message, string parameter)
        {
            Offset[] area = new Offset[]
            {
                new Offset(-1, -1), new Offset(0, -1), new Offset(1, -1),
                new Offset(-1, 0),  new Offset(0, 0),  new Offset(1, 0),
                new Offset(-1, 1),  new Offset(0, 1),  new Offset(1, 1)
            };

            Item itemWeapon = Formula.GetKnightWeapon(player);

            if (itemWeapon == null)
            {
                var formula = Formula.FierceBerserkFormula(player.Level, player.Skills.GetClientSkillLevel(Skill.Fist), 7);

                return Context.AddCommand(new CreatureAttackAreaCommand(player, false, player.Tile.Position, area, null, MagicEffectType.BlackSpark,
                        
                    new DamageAttack(null, null, DamageType.Physical, formula.Min, formula.Max, true) ) );
            }
            else if (itemWeapon.Metadata.WeaponType == WeaponType.Sword)
            {
                var formula = Formula.FierceBerserkFormula(player.Level, player.Skills.GetClientSkillLevel(Skill.Sword), itemWeapon.Metadata.Attack.Value);

                return Context.AddCommand(new CreatureAttackAreaCommand(player, false, player.Tile.Position, area, null, MagicEffectType.BlackSpark,
                        
                    new DamageAttack(null, null, DamageType.Physical, formula.Min, formula.Max, true) ) );
            }
            else if (itemWeapon.Metadata.WeaponType == WeaponType.Axe)
            {
                var formula = Formula.FierceBerserkFormula(player.Level, player.Skills.GetClientSkillLevel(Skill.Axe), itemWeapon.Metadata.Attack.Value);

                return Context.AddCommand(new CreatureAttackAreaCommand(player, false, player.Tile.Position, area, null, MagicEffectType.BlackSpark,
                        
                    new DamageAttack(null, null, DamageType.Physical, formula.Min, formula.Max, true) ) );
            }
            else if (itemWeapon.Metadata.WeaponType == WeaponType.Club)
            {
                var formula = Formula.FierceBerserkFormula(player.Level, player.Skills.GetClientSkillLevel(Skill.Club), itemWeapon.Metadata.Attack.Value);

                return Context.AddCommand(new CreatureAttackAreaCommand(player, false, player.Tile.Position, area, null, MagicEffectType.BlackSpark,
                        
                    new DamageAttack(null, null, DamageType.Physical, formula.Min, formula.Max, true) ) );
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}