﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Common;
using System;

namespace OpenTibia.Game.Commands
{
    public class CreatureAttackCreatureCommand : Command
    {
        public CreatureAttackCreatureCommand(Creature attacker, Creature target, Attack attack)

            : this(attacker, target, attack, null)
        {
           
        }

        public CreatureAttackCreatureCommand(Creature attacker, Creature target, Attack attack, Condition condition)
        {
            Attacker = attacker;

            Target = target;

            Attack = attack;

            Condition = condition;
        }

        public Creature Attacker { get; set; }

        public Creature Target { get; set; }

        public Attack Attack { get; set; }

        public Condition Condition { get; set; }

        public override async Promise Execute()
        {
            int damage = Attack.Calculate(Attacker, Target);

            if (damage == 0)
            {
                await Attack.Missed(Attacker, Target);
            }
            else
            {
                await Attack.Hit(Attacker, Target, damage);

                if (Condition != null)
                {
                    await Context.AddCommand(new CreatureAddConditionCommand(Target, Condition) );
                }
            }

            if (Attack is DamageAttack)
            {
                if (Attacker is Player attacker)
                {
                    await Context.Current.AddCommand(new CreatureAddConditionCommand(attacker, new LogoutBlockCondition(TimeSpan.FromSeconds(Context.Current.Server.Config.GameplayLogoutBlockSeconds) ) ) );
                }
            
                if (Target is Player target)
                {                               
                    await Context.Current.AddCommand(new CreatureAddConditionCommand(target, new LogoutBlockCondition(TimeSpan.FromSeconds(Context.Current.Server.Config.GameplayLogoutBlockSeconds) ) ) );
                }
            }
        }
    }
}