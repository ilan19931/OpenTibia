﻿using OpenTibia.Common.Objects;

namespace OpenTibia.Game.Events
{
    public class PlayerLoginEventArgs : GameEventArgs
    {
        public PlayerLoginEventArgs(Tile tile, Player player)
        {
            Tile = tile;

            Player = player;
        }

        public Tile Tile { get; set; }

        public Player Player { get; set; }
    }
}