﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Common;
using OpenTibia.Network.Packets.Outgoing;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OpenTibia.Game.Commands
{
    public class PlayerLookItemCommand : Command
    {
        public PlayerLookItemCommand(Player player, Item item)
        {
            Player = player;

            Item = item;

            ItemMetadata = item.Metadata;

            if (item is StackableItem stackableItem)
            {
                TypeCount = stackableItem.Count;
            }
            else if (item is FluidItem fluidItem)
            {
                TypeCount = (byte)fluidItem.FluidType;
            }
            else if (item is SplashItem splashItem)
            {
                TypeCount = (byte)splashItem.FluidType;
            }
            else
            {
                TypeCount = 1;
            }
        }

        public PlayerLookItemCommand(Player player, ItemMetadata itemMetadata, byte typeCount)
        {
            Player = player;

            ItemMetadata = itemMetadata;

            TypeCount = typeCount;
        }

        public Player Player { get; set; }

        public Item Item { get; set; }

        public ItemMetadata ItemMetadata { get; set; }

        public byte TypeCount { get; set; }

        public override Promise Execute()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("You see " + ItemMetadata.GetDescription(TypeCount) );

            if (Item != null)
            {
                if (ItemMetadata.ShowCharges && ItemMetadata.Charges != null && ItemMetadata.Charges > 0 && Item.Charges > 0)
                {
                    if (Item.Charges == 1)
                    {
                        builder.Append(" that has 1 charge left");
                    }
                    else
                    {
                        builder.Append(" that has " + Item.Charges + " charges left");
                    }
                }

                if (ItemMetadata.ShowDuration && ItemMetadata.DurationInMilliseconds != null && ItemMetadata.DurationInMilliseconds > 0 && Item.DurationInMilliseconds > 0)
                {
                    if (Item.DurationInMilliseconds == ItemMetadata.DurationInMilliseconds)
                    {
                        builder.Append(" that is brand-new");
                    }
                    else
                    {
                        int durationInSeconds = Item.DurationInMilliseconds / 1000;

                        if (durationInSeconds > 120)
                        {
                            builder.Append(" that has energy for " + (durationInSeconds / 60) + " minutes left");
                        }
                        else if (durationInSeconds > 60)
                        {
                            builder.Append(" that has energy for 1 minute left");
                        }
                        else
                        {
                            builder.Append(" that has energy for less than a minute left");
                        }
                    }
                }
            }

            List<string> attributes = new List<string>();

            if (ItemMetadata.RuneSpellName != null)
            {
                attributes.Add("\"" + ItemMetadata.RuneSpellName + "\"");
            }

            if (ItemMetadata.Flags.Is(ItemMetadataFlags.IsContainer) )
            {
                attributes.Add("Vol: " + ItemMetadata.Capacity);
            }

            if (ItemMetadata.Attack != null)
            {
                if (ItemMetadata.AttackModifier != null && ItemMetadata.AttackDamageType != null)
                {
                    attributes.Add("Atk: " + ItemMetadata.Attack + " physical " + (ItemMetadata.AttackModifier.Value > 0 ? "+" + ItemMetadata.AttackModifier.Value : ItemMetadata.AttackModifier.Value.ToString() ) + " " + ItemMetadata.AttackDamageType.Value.GetDescription() );
                }
                else
                {
                    attributes.Add("Atk: " + ItemMetadata.Attack);
                }
            }

            if (ItemMetadata.Range != null)
            {
                attributes.Add("Range: " + ItemMetadata.Range);
            }

            if (ItemMetadata.Defense != null)
            {
                if (ItemMetadata.DefenseModifier != null)
                {
                    attributes.Add("Def: " + ItemMetadata.Defense + " " + (ItemMetadata.DefenseModifier.Value > 0 ? "+" + ItemMetadata.DefenseModifier.Value : ItemMetadata.DefenseModifier.Value.ToString() ) );
                }
                else
                {
                    attributes.Add("Def: " + ItemMetadata.Defense);
                }
            }

            if (ItemMetadata.Armor != null)
            {
                if (ItemMetadata.DamageTakenFromElements.Count > 0)
                {
                    StringBuilder modifiers = new StringBuilder();

                    foreach (var item in ItemMetadata.DamageTakenFromElements)
                    {
                        var value = 100 - item.Value * 100;

                        modifiers.Append( (value > 0 ? "+" + value : value.ToString() ) + "% " + item.Key.GetDescription() + " ");
                    }

                    attributes.Add("Arm: " + ItemMetadata.Armor + " protection " + modifiers.Remove(modifiers.Length - 1, 1).ToString() );
                }
                else
                {
                    attributes.Add("Arm: " + ItemMetadata.Armor);
                }
            }

            if (ItemMetadata.SpeedModifier != null)
            {
                attributes.Add("speed " + (ItemMetadata.SpeedModifier.Value > 0 ? "+" + ItemMetadata.SpeedModifier.Value : ItemMetadata.SpeedModifier.Value.ToString() ) );
            }

            if (ItemMetadata.SkillModifier.Count > 0)
            {
                foreach (var item in ItemMetadata.SkillModifier)
                {
                    attributes.Add(item.Key.GetDescription() + " " + (item.Value > 0 ? "+" + item.Value : item.Value.ToString() ) );
                }
            }

            if (Player.Rank == Rank.Gamemaster)
            {
                attributes.Add("Item Id: " + ItemMetadata.OpenTibiaId);

                if (Item != null)
                {
                    if (Item.ActionId > 0)
                    {
                        attributes.Add("Action Id: " + Item.ActionId);
                    }

                    if (Item.UniqueId > 0)
                    {
                        attributes.Add("Unique Id: " + Item.UniqueId);
                    }
                }
            }

            if (attributes.Count > 0)
            {
                builder.Append(" (" + string.Join(", ", attributes) + ")");
            }

            builder.Append(".");

            List<string> descriptions = new List<string>();

            if (ItemMetadata.Description != null)
            {
                descriptions.Add(ItemMetadata.Description);
            }

            if (Item != null)
            {
                if (ItemMetadata.Flags.Is(ItemMetadataFlags.Pickupable) && Item.UniqueId == 0)
                {
                    uint weight = Item.GetWeight();

                    if (weight > 0 && ( (Item.Parent is Tile tile && Player.Tile.Position.IsInRange(tile.Position, 1) ) || Item.Parent is Inventory || Item.Parent is Container) )
                    {
                        descriptions.Add("It weights " + (weight / 100.0).ToString("0.00", CultureInfo.InvariantCulture) + " oz.");
                    }
                }

                if (ItemMetadata.Flags.Is(ItemMetadataFlags.AllowDistanceRead) && Item is ReadableItem readableItem)
                {
                    if (Player.Tile.Position.IsInRange( ( (Tile)Item.Parent).Position, 4) )
                    {
                        if (readableItem.Text != null)
                        {
                            descriptions.Add("You read: " + readableItem.Text + ".");
                        }
                        else
                        {
                            descriptions.Add("Nothing is written on it.");
                        }
                    }
                    else
                    {
                        descriptions.Add("You are too far away to read it.");
                    } 
                }

                if (Item.Parent is HouseTile houseTile && Item is DoorItem doorItem)
                {
                    if (houseTile.House.Owner != null)
                    {
                        descriptions.Add("It belongs to house '" + houseTile.House.Name + "'. " + houseTile.House.Owner + " owns this house.");
                    }
                    else
                    {
                        descriptions.Add("It belongs to house '" + houseTile.House.Name + "'. Nobody owns this house. It costs " + (houseTile.House.Rent * 5) + " gold coins.");
                    }
                }
            }
                        
            if (descriptions.Count > 0)
            {
                builder.Append(" " + string.Join(" ", descriptions) );
            }

            Context.AddPacket(Player, new ShowWindowTextOutgoingPacket(MessageMode.Look, builder.ToString() ) );

            return Promise.Completed;
        }
    }
}