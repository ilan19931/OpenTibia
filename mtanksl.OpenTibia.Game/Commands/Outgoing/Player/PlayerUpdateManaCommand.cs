﻿using OpenTibia.Common.Objects;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class PlayerUpdateManaCommand : Command
    {
        public PlayerUpdateManaCommand(Player player, ushort mana, ushort maxMana)
        {
            Player = player;

            Mana = mana;

            MaxMana = maxMana;
        }

        public Player Player { get; set; }

        public ushort Mana { get; set; }

        public ushort MaxMana { get; set; }

        public override void Execute(Context context)
        {
            if (Player.Mana != Mana || Player.MaxMana != MaxMana)
            {
                Player.Mana = Mana;

                Player.MaxMana = MaxMana;

                context.AddPacket(Player.Client.Connection, new SendStatusOutgoingPacket(Player.Health, Player.MaxHealth, Player.Capacity, Player.Experience, Player.Level, Player.LevelPercent, Player.Mana, Player.MaxMana, 0, 0, Player.Soul, Player.Stamina) );
            }

            OnComplete(context);
        }
    }
}