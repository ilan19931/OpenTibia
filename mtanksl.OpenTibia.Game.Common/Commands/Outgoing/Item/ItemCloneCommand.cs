﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Common;
using System;

namespace OpenTibia.Game.Commands
{
    public class ItemCloneCommand : CommandResult<Item>
    {
        public ItemCloneCommand(Item item, bool deepClone)
        {
            Item = item;

            DeepClone = deepClone;
        }

        public Item Item { get; set; }

        public bool DeepClone { get; set; }

        public override async PromiseResult<Item> Execute()
        {
            byte typeCount = 1;

            if (Item is StackableItem stackableItem)
            {
                typeCount = stackableItem.Count;
            }
            else if (Item is FluidItem fluidItem)
            {
                typeCount = (byte)fluidItem.FluidType;
            }
            else if (Item is SplashItem splashItem)
            {
                typeCount = (byte)splashItem.FluidType;
            }

            Item toItem = Context.Server.ItemFactory.Create(Item.Metadata.OpenTibiaId, typeCount);

            if (toItem != null)
            {
                toItem.ActionId = Item.ActionId;

                toItem.UniqueId = Item.UniqueId;

                if (Item.Charges > 0)
                {
                    toItem.Charges = Item.Charges;
                }

                if (Item.DurationInMilliseconds > 0)
                {
                    toItem.DurationInMilliseconds = Item.DurationInMilliseconds;
                }

                if (Item is Container fromContainer)
                {
                    if (toItem is Container toContainer)
                    {
                        if (DeepClone)
                        {
                            foreach (var item in fromContainer.GetItems() )
                            {
                                Item clone = await Context.AddCommand(new ItemCloneCommand(item, true) );

                                toContainer.AddContent(clone);
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("ToItem must be Container.");
                    }
                }
                else if (Item is DoorItem fromDoorItem)
                {
                    if (toItem is DoorItem toDoorItem)
                    {
                        toDoorItem.DoorId = fromDoorItem.DoorId;
                    }
                    else 
                    {
                        throw new InvalidOperationException("ToItem must be DoorItem."); 
                    }                    
                }
                else if (Item is ReadableItem fromReadableItem)
                {
                    if (toItem is ReadableItem toReadableItem)
                    {
                        toReadableItem.Text = fromReadableItem.Text;

                        toReadableItem.WrittenBy = fromReadableItem.WrittenBy;

                        toReadableItem.WrittenDate = fromReadableItem.WrittenDate;
                    }
                    else 
                    {
                        throw new InvalidOperationException("ToItem must be ReadableItem."); 
                    }                    
                }
                else if (Item is TeleportItem fromTeleportItem)
                {
                    if (toItem is TeleportItem toTeleportItem)
                    {
                        toTeleportItem.Position = fromTeleportItem.Position;
                    }
                    else 
                    {
                        throw new InvalidOperationException("ToItem must be TeleportItem."); 
                    }                    
                }
                
                Context.Server.ItemFactory.Attach(toItem);
            }

            return toItem;
        }
    }
}
