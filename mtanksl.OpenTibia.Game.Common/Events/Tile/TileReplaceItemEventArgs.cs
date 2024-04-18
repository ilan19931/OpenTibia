﻿using OpenTibia.Common.Objects;

namespace OpenTibia.Game.Events
{
    public class TileReplaceItemEventArgs : GameEventArgs
    {
        public TileReplaceItemEventArgs(Tile tile, Item fromItem, Item toItem, int index)
        {
            Tile = tile;

            FromItem = fromItem;

            ToItem = toItem;

            Index = index;
        }

        public Tile Tile { get; set; }

        public Item FromItem { get; set; }

        public Item ToItem { get; set; }

        public int Index { get; set; }
    }
}