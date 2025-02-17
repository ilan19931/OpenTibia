﻿using OpenTibia.Common.Structures;
using System.Collections.Generic;

namespace OpenTibia.Common.Objects
{
    public class MonsterMetadata
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ushort Speed { get; set; }

        public uint Experience { get; set; }

        public Race Race { get; set; }

        public ushort Health { get; set; }

        public ushort MaxHealth { get; set; }

        public Outfit Outfit { get; set; }

        public ushort Corpse { get; set; }

        public VoiceCollection Voices { get; set; }

        public LootItem[] Loot { get; set; }

        public Dictionary<DamageType, double> DamageTakenFromElements { get; set; }

        public bool Summonable { get; set; }

        public bool Attackable { get; set; }

        public bool Hostile { get; set; }

        public bool Illusionable { get; set; }

        public bool Convinceable { get; set; }

        public bool Pushable { get; set; }

        public bool CanPushItems { get; set; }

        public bool CanPushCreatures { get; set; }

        public AttackItem[] Attacks { get; set; }

        public int Armor { get; set; }

        public int Defense { get; set; }

        public DefenseItem[] Defenses { get; set; }
    }
}