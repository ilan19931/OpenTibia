﻿using OpenTibia.Common.Structures;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OpenTibia.FileFormats.Xml.Monsters
{
    [XmlRoot("monster")]
    public class Monster
    {
        public static Monster Load(XElement monsterNode)
        {           
            Monster monster = new Monster();

            monster.Name = (string)monsterNode.Attribute("name");

            monster.NameDescription = (string)monsterNode.Attribute("nameDescription");

            monster.Speed = (int)monsterNode.Attribute("speed");

            monster.Experience = (uint)monsterNode.Attribute("experience");

            switch ( (string)monsterNode.Attribute("race") )
            {
                case "blood":

                    monster.Race = Race.Blood;

                    break;

                case "energy":

                    monster.Race = Race.Energy;

                    break;

                case "fire":

                    monster.Race = Race.Fire;

                    break;

                case "venom":

                    monster.Race = Race.Venom;

                    break;

                case "undead":

                    monster.Race = Race.Undead;

                    break;
            }

            XElement healthNode = monsterNode.Element("health");

            monster.Health = new Health()
            {
                Now = (int)healthNode.Attribute("now"),

                Max = (int)healthNode.Attribute("max")
            };

            XElement outfitNode = monsterNode.Element("look");

            monster.Look = new Look()
            {
                TypeEx = (int?)outfitNode.Attribute("typeex") ?? 0,

                Type = (int?)outfitNode.Attribute("type") ?? 0,

                Head = (int?)outfitNode.Attribute("head") ?? 0,

                Body = (int?)outfitNode.Attribute("body") ?? 0,

                Legs = (int?)outfitNode.Attribute("legs") ?? 0,

                Feet = (int?)outfitNode.Attribute("feet") ?? 0,

                Corpse = (int?)outfitNode.Attribute("corpse") ?? 3058
            };
 
            XElement voicesNode = monsterNode.Element("voices");

            if (voicesNode != null)
            {
                monster.Voices = new VoiceCollection()
                {
                    Interval = (int)voicesNode.Attribute("interval"),

                    Chance = Math.Max(0, Math.Min(100, (double)voicesNode.Attribute("chance") ) ),

                    Items = new List<VoiceItem>()
                };

                foreach (var voiceNode in voicesNode.Elements() )
                {
                    monster.Voices.Items.Add(new VoiceItem() 
                    { 
                        Sentence = (string)voiceNode.Attribute("sentence"),

                        Yell = (int?)voiceNode.Attribute("yell")
                    } );
                }
            }

            XElement lootNode = monsterNode.Element("loot");

            if (lootNode != null)
            {
                monster.Loot = new List<LootItem>();

                foreach (var itemNode in lootNode.Elements() )
                {
                    monster.Loot.Add(new LootItem() 
                    { 
                        Id = (ushort)(int)itemNode.Attribute("id"),

                        CountMin = (int?)itemNode.Attribute("countmin"),

                        CountMax = (int?)itemNode.Attribute("countmax"),

                        KillsToGetOne = (int)itemNode.Attribute("killsToGetOne")
                    } );
                }
            }

            XElement elementsNode = monsterNode.Element("elements");

            if (elementsNode != null)
            {
                monster.Elements = new List<ElementItem>();

                foreach (var elementNode in elementsNode.Elements() )
                {
                    monster.Elements.Add(new ElementItem() 
                    {
                        PhysicalPercent = (int?)elementNode.Attribute("physicalPercent"),

                        Earthpercent = (int?)elementNode.Attribute("earthpercent"),

                        FirePercent = (int?)elementNode.Attribute("firePercent"),

                        EnergyPercent = (int?)elementNode.Attribute("energyPercent"),

                        IcePercent = (int?)elementNode.Attribute("icePercent"),

                        DeathPercent = (int?)elementNode.Attribute("deathPercent"),

                        HolyPercent = (int?)elementNode.Attribute("holyPercent"),

                        DrownPercent = (int?)elementNode.Attribute("drownPercent"),

                        ManaDrainPercent = (int?)elementNode.Attribute("manaDrainPercent"),

                        LifeDrainPercent = (int?)elementNode.Attribute("lifeDrainPercent")
                    } );
                }
            }

            XElement flagsNode = monsterNode.Element("flags");

            if (flagsNode != null)
            {
                monster.Flags = new List<FlagItem>();

                foreach (var flagNode in flagsNode.Elements() )
                {
                    monster.Flags.Add(new FlagItem() 
                    {
                        Summonable = (int?)flagNode.Attribute("summonable"),

                        Attackable = (int?)flagNode.Attribute("attackable"),

                        Hostile = (int?)flagNode.Attribute("hostile"),

                        Illusionable = (int?)flagNode.Attribute("illusionable"),

                        Convinceable = (int?)flagNode.Attribute("convinceable"),

                        Pushable = (int?)flagNode.Attribute("pushable"),

                        CanPushItems = (int?)flagNode.Attribute("canpushitems"),

                        CanPushCreatures = (int?)flagNode.Attribute("canpushcreatures")
                    } );
                }
            }

            XElement attacksNode = monsterNode.Element("attacks");

            if (attacksNode != null)
            {
                monster.Attacks = new List<AttackItem>();

                foreach (var attackNode in attacksNode.Elements() )
                {
                    monster.Attacks.Add(new AttackItem() 
                    {
                        Name = (string)attackNode.Attribute("name"),

                        Interval = (int)attackNode.Attribute("interval"),

                        Chance = Math.Max(0, Math.Min(100, (double)voicesNode.Attribute("chance") ) ),

                        Min = (int?)attackNode.Attribute("min"),

                        Max = (int?)attackNode.Attribute("max")
                    } );
                }
            }

            XElement defensesNode = monsterNode.Element("defenses");

            if (defensesNode != null)
            {
                monster.Defenses = new DefenseCollection()
                {
                    Armor = (int)voicesNode.Attribute("armor"),
                    
                    Defense = (int)voicesNode.Attribute("defense"),

                    Items = new List<DefenseItem>()
                };

                foreach (var defenseNode in defensesNode.Elements() )
                {
                    monster.Defenses.Items.Add(new DefenseItem() 
                    {
                        Name = (string)defenseNode.Attribute("name"),

                        Interval = (int)defenseNode.Attribute("interval"),

                        Chance = Math.Max(0, Math.Min(100, (double)voicesNode.Attribute("chance") ) ),

                        Min = (int?)defenseNode.Attribute("min"),

                        Max = (int?)defenseNode.Attribute("max")
                    } );
                }
            }

            return monster;
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("nameDescription")]
        public string NameDescription { get; set; }

        [XmlAttribute("speed")]
        public int Speed { get; set; }

        [XmlAttribute("experience")]
        public uint Experience { get; set; }

        [XmlAttribute("race")]
        public Race Race { get; set; }

        [XmlElement("health")]
        public Health Health { get; set; }

        [XmlElement("look")]
        public Look Look { get; set; }

        [XmlElement("voices")]
        public VoiceCollection Voices { get; set; }

        [XmlArray("loot")]
        [XmlArrayItem("item")]
        public List<LootItem> Loot { get; set; }

        [XmlArray("elements")]
        [XmlArrayItem("element")]
        public List<ElementItem> Elements { get; set; }

        [XmlArray("flags")]
        [XmlArrayItem("flag")]
        public List<FlagItem> Flags { get; set; }

        [XmlArray("attacks")]
        [XmlArrayItem("attack")]
        public List<AttackItem> Attacks { get; set; }

        [XmlElement("defenses")]
        public DefenseCollection Defenses { get; set; }
    }
}