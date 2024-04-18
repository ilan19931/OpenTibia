﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Common;
using OpenTibia.Network.Packets.Outgoing;

namespace OpenTibia.Game.Commands
{
    public class PlayerUpdateFishCommand : Command
    {
        public PlayerUpdateFishCommand(Player player, byte fish, byte fishPercent)
        {
            Player = player;

            Fish = fish;

            FishPercent = fishPercent;
        }

        public Player Player { get; set; }

        public byte Fish { get; set; }

        public byte FishPercent { get; set; }

        public override Promise Execute()
        {
            if (Player.Skills.Fish != Fish || Player.Skills.FishPercent != FishPercent)
            {
                Player.Skills.Fish = Fish;

                Player.Skills.FishPercent = FishPercent;

                Context.AddPacket(Player, new SendSkillsOutgoingPacket(Player.Skills.Fist, Player.Skills.FistPercent, Player.Skills.Club, Player.Skills.ClubPercent, Player.Skills.Sword, Player.Skills.SwordPercent, Player.Skills.Axe, Player.Skills.AxePercent, Player.Skills.Distance, Player.Skills.DistancePercent, Player.Skills.Shield, Player.Skills.ShieldPercent, Player.Skills.Fish, Player.Skills.FishPercent) );
            }

            return Promise.Completed;
        }
    }
}