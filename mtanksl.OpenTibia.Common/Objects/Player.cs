﻿using OpenTibia.Common.Structures;
using System;

namespace OpenTibia.Common.Objects
{
    public class Player : Creature
    {
        public Player()
        {
            Inventory = new Inventory(this);

            Lockers = new Safe(this);

            Outfits = new PlayerOutfitCollection();

            Storages = new PlayerStorageCollection();

            Achievements = new PlayerAchievementsCollection();

            Spells = new PlayerSpellCollection();

            Blesses = new PlayerBlessCollection();

            Vips = new PlayerVipCollection();

            Skills = new Skills(this)
            {
                MagicLevel = 0,

                Fist = 10,

                Club = 10,

                Sword = 10,

                Axe = 10,

                Distance = 10,

                Shield = 10,

                Fish = 10
            };

            Experience = 0;

            Level = 1;

            LevelPercent = 0;

            MaxMana = Mana = 55;

            Soul = 100;

            Capacity = 400 * 100;

            Stamina = 42 * 60;
        }

        private IClient client;

        public IClient Client
        {
            get
            {
                return client;
            }
            set
            {
                if (value != client)
                {
                    var current = client;

                                  client = value;

                    if (value == null)
                    {
                        current.Player = null;
                    }
                    else
                    {
                        client.Player = this;
                    }
                }
            }
        }

        public int DatabasePlayerId { get; set; }

        public int DatabaseAccountId { get; set; }

        public Inventory Inventory { get; }

        public Safe Lockers { get; }

        public PlayerOutfitCollection Outfits { get; }

        public PlayerStorageCollection Storages { get; }

        public PlayerAchievementsCollection Achievements { get; }

        public PlayerSpellCollection Spells { get; }

        public PlayerBlessCollection Blesses { get; }

        public PlayerVipCollection Vips { get; }
        
        public Skills Skills { get; set; }

        public ulong Experience { get; set; }

        public ushort Level { get; set; }

        public byte LevelPercent { get; set; }

        public ushort Mana { get; set; }

        public ushort MaxMana { get; set; }

        public byte Soul { get; set; }

        public uint Capacity { get; set; }

        public ushort Stamina { get; set; }

        public Gender Gender { get; set; }

        public Vocation Vocation { get; set; }

        public Rank Rank { get; set; }

        public bool Premium { get; set; }

        public ulong BankAccount { get; set; }

        private void Calculate(ushort level)
        {
            Level = level;

            BaseSpeed = Speed = (ushort)( 2 * level + 218 );

            Experience = (ulong)( ( 50 * Math.Pow(level - 1, 3) - 150 * Math.Pow(level - 1, 2) + 400 * (level - 1) ) / 3 );
        }

        /// <exception cref="InvalidOperationException"></exception>

        public void CalculateRook(ushort level)
        {
            if (level < 1 || level > 507)
            {
                throw new InvalidOperationException();
            }

            Calculate(level);

            MaxHealth = Health = (ushort)( 5 * level + 145 );

            MaxMana = Mana = (ushort)( 5 * level + 50 );

            Capacity = (uint)( 10 * level + 390 ) * 100;
        }

        /// <exception cref="InvalidOperationException"></exception>

        public void CalculateKnight(ushort level)
        {
            if (level < 8 || level > 507)
            {
                throw new InvalidOperationException();
            }

            Calculate(level);

            MaxHealth = Health = (ushort)( 15 * level + 65 );

            MaxMana = Mana = (ushort)( 5 * level + 50 );

            Capacity = (uint)( 25 * level + 270 ) * 100;
        }

        /// <exception cref="InvalidOperationException"></exception>

        public void CalculatePaladin(ushort level)
        {
            if (level < 8 || level > 507)
            {
                throw new InvalidOperationException();
            }

            Calculate(level);

            MaxHealth = Health = (ushort)( 10 * level + 105 );

            MaxMana = Mana = (ushort)( 15 * level - 30 );

            Capacity = (uint)( 20 * level + 310 ) * 100;
        }

        /// <exception cref="InvalidOperationException"></exception>

        public void CalculateSorcererAndDruid(ushort level)
        {
            if (level < 8 || level > 507)
            {
                throw new InvalidOperationException();
            }

            Calculate(level);

            MaxHealth = Health = (ushort)( 5 * level + 145 );

            MaxMana = Mana = (ushort)( 30 * level - 150 );

            Capacity = (uint)( 10 * level + 390 ) * 100;            
        }
    }
}