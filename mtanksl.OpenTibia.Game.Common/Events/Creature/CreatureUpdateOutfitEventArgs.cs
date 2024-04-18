﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;

namespace OpenTibia.Game.Events
{
    public class CreatureUpdateOutfitEventArgs : GameEventArgs
    {
        public CreatureUpdateOutfitEventArgs(Creature creature, Outfit baseOutfit, Outfit outfit)
        {
            Creature = creature;

            BaseOutfit = baseOutfit;

            Outfit = outfit;
        }

        public Creature Creature { get; set; }

        public Outfit BaseOutfit { get; set; }

        public Outfit Outfit { get; set; }
    }
}