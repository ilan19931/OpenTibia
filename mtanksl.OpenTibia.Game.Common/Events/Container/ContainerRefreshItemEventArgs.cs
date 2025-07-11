﻿using OpenTibia.Common.Objects;

namespace OpenTibia.Game.Events
{
    public class ContainerRefreshItemEventArgs : GameEventArgs
    {
        public ContainerRefreshItemEventArgs(Container container, Item item, ushort index)
        {
            Container = container;

            Item = item;

            Index = index;
        }

        public Container Container { get; }

        public Item Item { get; }

        public ushort Index { get; }
    }
}