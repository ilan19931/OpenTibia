﻿using System;

namespace OpenTibia.Common.Structures
{
    public enum DamageType : byte
    {
        Physical = 1,

        Earth = 2,

        Fire = 3,

        Energy = 4,

        Ice = 5,

        Death = 6,

        Holy = 7,

        Drown = 8,

        ManaDrain = 9,

        LifeDrain = 10
    }

    public static class DamageTypeExtensions
    {
        public static string GetDescription(this DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.Physical:

                    return "physical";

                case DamageType.Earth:

                    return "earth";

                case DamageType.Fire:

                    return "fire";

                case DamageType.Energy:

                    return "energy";

                case DamageType.Ice:

                    return "ice";

                case DamageType.Death:

                    return "death";

                case DamageType.Holy:

                    return "holy";

                case DamageType.Drown:

                    return "drown";

                case DamageType.ManaDrain:

                    return "mana drain";

                case DamageType.LifeDrain:

                    return "life drain";
            }

            return null;
        }

        public static MagicEffectType? ToMagicEffectType(this DamageType damageType, Race race)
        {
            switch (damageType)
            {
                case DamageType.Physical:

                    switch (race)
                    {
                        case Race.Blood:

                            return MagicEffectType.RedSpark;

                        case Race.Energy:

                            return MagicEffectType.PurpleEnergyDamage;

                        case Race.Fire:

                            return MagicEffectType.RedSpark;

                        case Race.Venom:

                            return MagicEffectType.GreenSpark;

                        case Race.Undead:

                            return MagicEffectType.BlackSpark;
                    }

                    break;                    

                case DamageType.Earth:

                    return MagicEffectType.GreenRings;

                case DamageType.Fire:

                    return MagicEffectType.FireDamage;

                case DamageType.Energy:

                    return MagicEffectType.EnergyDamage;

                case DamageType.Ice:

                    return MagicEffectType.IceDamage;

                case DamageType.Death:

                    return MagicEffectType.SmallClouds;

                case DamageType.Holy:

                    return MagicEffectType.HolyDamage;

                case DamageType.Drown:

                    return MagicEffectType.BlueRings;

                case DamageType.ManaDrain:

                    return MagicEffectType.BlueRings;

                case DamageType.LifeDrain:

                    return MagicEffectType.RedShimmer;
            }

            throw new NotImplementedException();
        }

        public static AnimatedTextColor? ToAnimatedTextColor(this DamageType damageType, Race race)
        {
            switch (damageType)
            {
                case DamageType.Physical:

                    switch (race)
                    {
                        case Race.Blood:

                            return AnimatedTextColor.Red;

                        case Race.Energy:

                            return AnimatedTextColor.Purple;

                        case Race.Fire:

                            return AnimatedTextColor.Orange;

                        case Race.Venom:

                            return AnimatedTextColor.LightGreen;

                        case Race.Undead:

                            return AnimatedTextColor.LightGrey;
                    }

                    break;

                case DamageType.Earth:

                    return AnimatedTextColor.Green;

                case DamageType.Fire:

                    return AnimatedTextColor.Orange;

                case DamageType.Energy:

                    return AnimatedTextColor.Purple;

                case DamageType.Ice:

                    return AnimatedTextColor.SeaBlue;

                case DamageType.Death:

                    return AnimatedTextColor.DarkRed;

                case DamageType.Holy:

                    return AnimatedTextColor.Yellow;

                case DamageType.Drown:

                    return AnimatedTextColor.Cyan;

                case DamageType.ManaDrain:

                    return AnimatedTextColor.Blue;

                case DamageType.LifeDrain:

                    return AnimatedTextColor.Red;
            }

            throw new NotImplementedException();
        }
    }
}