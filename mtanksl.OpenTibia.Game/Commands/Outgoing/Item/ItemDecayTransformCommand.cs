﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Components;

namespace OpenTibia.Game.Commands
{
    public class ItemDecayTransformCommand : CommandResult<Item>
    {
        public ItemDecayTransformCommand(Item item, int executeInMilliseconds, ushort openTibiaId, byte count)
        {
            Item = item;

            ExecuteInMilliseconds = executeInMilliseconds;

            OpenTibiaId = openTibiaId;

            Count = count;
        }

        public Item Item { get; set; }

        public int ExecuteInMilliseconds { get; set; }

        public ushort OpenTibiaId { get; set; }

        public byte Count { get; set; }

        public override PromiseResult<Item> Execute()
        {
            return Context.Server.Components.AddComponent(Item, new ItemDecayBehaviour(ExecuteInMilliseconds) ).Promise.Then( () =>
            { 
                return Context.AddCommand(new ItemTransformCommand(Item, OpenTibiaId, Count) );
            } );
        }
    }
}