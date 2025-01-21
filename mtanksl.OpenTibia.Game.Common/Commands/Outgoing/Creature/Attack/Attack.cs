﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Common;

namespace OpenTibia.Game.Commands
{
    public abstract class Attack
    {
        protected Attack(DamageType damageType, int min, int max)
        {
            DamageType = damageType;

            Min = min;

            Max = max;
        }

        public DamageType DamageType { get; }

        public int Min { get; }

        public int Max { get; }

        public virtual int Calculate(Creature attacker, Creature target)
        {
            return Context.Current.Server.Randomization.Take(Min, Max);
        }

        public abstract Promise Missed(Creature attacker, Creature target);

        public abstract Promise Hit(Creature attacker, Creature target, int damage);
    }    
}