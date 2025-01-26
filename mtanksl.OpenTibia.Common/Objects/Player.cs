﻿using OpenTibia.Common.Structures;

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

            Kills = new PlayerKillCollection();

            Skills = new Skills(this);

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

        public PlayerKillCollection Kills { get; set; }

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
    }
}