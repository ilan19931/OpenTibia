﻿using OpenTibia.Common.Objects;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class PlayerUpdateStaminaCommand : Command
    {
        public PlayerUpdateStaminaCommand(Player player, ushort stamina)
        {
            Player = player;

            Stamina = stamina;
        }

        public Player Player { get; set; }

        public ushort Stamina { get; set; }

        public override void Execute(Context context)
        {
            if (Player.Stamina != Stamina)
            {
                Player.Stamina = Stamina;

                context.AddPacket(Player.Client.Connection, new SendStatusOutgoingPacket(Player.Health, Player.MaxHealth, Player.Capacity, Player.Experience, Player.Level, Player.LevelPercent, Player.Mana, Player.MaxMana, 0, 0, Player.Soul, Player.Stamina) );
            }

            OnComplete(context);
        }
    }
}