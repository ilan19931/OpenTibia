﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Common;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.Commands
{
    public class PlayerDestroyItemsCommand : CommandResult<bool>
    {
        public PlayerDestroyItemsCommand(Player player, ushort openTibiaId, byte type, int count)
        {
            Player = player;

            OpenTibiaId = openTibiaId;

            Type = type;

            Count = count;
        }

        public Player Player { get; set; }

        public ushort OpenTibiaId { get; set; }

        public byte Type { get; set; }

        public int Count { get; set; }

        public override async PromiseResult<bool> Execute()
        {
            List<Item> items = new List<Item>();

            int sum = Sum(Player.Inventory, OpenTibiaId, Type, items);

            if (sum < Count)
            {
                return false;
            }

            foreach (Item item in items)
            {
                if (Count == 0)
                {
                    break;
                }

                if (item is StackableItem stackableItem)
                {
                    byte stack = (byte)Math.Min(stackableItem.Count, Count);

                    await Context.AddCommand(new ItemDecrementCommand(item, stack) );

                    Count -= stack;
                }
                else
                {
                    await Context.AddCommand(new ItemDecrementCommand(item, 1) );

                    Count -= 1;
                }
            }

            return true;                
        }

        private static int Sum(IContainer parent, ushort openTibiaId, byte type, List<Item> items)
        {
            int sum = 0;

            foreach (Item content in parent.GetContents() )
            {
                if (content is Container container)
                {
                    sum += Sum(container, openTibiaId, type, items);
                }

                if (content.Metadata.OpenTibiaId == openTibiaId)
                {
                    if (content is StackableItem stackableItem)
                    {
                        items.Add(content);

                        sum += stackableItem.Count;
                    }
                    else if (content is FluidItem fluidItem)
                    {
                        if (fluidItem.FluidType == (FluidType)type)
                        {
                            items.Add(content);

                            sum += 1;
                        }
                    }
                    else
                    {
                        items.Add(content);

                        sum += 1;
                    }
                }
            }

            return sum;
        }
    }
}