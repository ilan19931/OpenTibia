﻿using OpenTibia.Common.Objects;

namespace OpenTibia.Game.Events
{
    public class PlayerUpdateStaminaEventArgs : GameEventArgs
    {
        public PlayerUpdateStaminaEventArgs(Player player, ushort stamina)
        {
            Player = player;

            Stamina = stamina;
        }

        public Player Player { get; set; }

        public ushort Stamina { get; set; }
    }
}