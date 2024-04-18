﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Common;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class PlayerUpdateSwordCommand : Command
    {
        public PlayerUpdateSwordCommand(Player player, byte sword, byte swordPercent)
        {
            Player = player;

            Sword = sword;

            SwordPercent = swordPercent;
        }

        public Player Player { get; set; }

        public byte Sword { get; set; }

        public byte SwordPercent { get; set; }

        public override Promise Execute()
        {
            if (Player.Skills.Sword != Sword || Player.Skills.SwordPercent != SwordPercent)
            {
                Player.Skills.Sword = Sword;

                Player.Skills.SwordPercent = SwordPercent;

                Context.AddPacket(Player, new SendSkillsOutgoingPacket(Player.Skills.Fist, Player.Skills.FistPercent, Player.Skills.Club, Player.Skills.ClubPercent, Player.Skills.Sword, Player.Skills.SwordPercent, Player.Skills.Axe, Player.Skills.AxePercent, Player.Skills.Distance, Player.Skills.DistancePercent, Player.Skills.Shield, Player.Skills.ShieldPercent, Player.Skills.Fish, Player.Skills.FishPercent) );
            }

            return Promise.Completed;
        }
    }
}