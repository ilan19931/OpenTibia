﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;
using OpenTibia.Game.Events;
using OpenTibia.Network.Packets.Outgoing;
using System;

namespace OpenTibia.Game.Commands
{
    public class PlayerAddSkillPointsCommand : Command
    {
        public PlayerAddSkillPointsCommand(Player player, Skill skill, ulong skillPoints)
        {
            Player = player;

            Skill = skill;

            SkillPoints = skillPoints;
        }

        public Player Player { get; set; }

        public Skill Skill { get; set; }

        public ulong SkillPoints { get; set; }

        public override async Promise Execute()
        {
            if (Skill == Skill.MagicLevel)
            {
                SkillPoints = (ulong)(SkillPoints * Context.Server.Config.GameplayMagicLevelRate);
            }
            else
            {
                SkillPoints = (ulong)(SkillPoints * Context.Server.Config.GameplaySkillRate);
            }

            byte currentSkillLevel = Player.Skills.GetSkillLevel(Skill);

            ulong currentSkillPoints = Player.Skills.GetSkillPoints(Skill);

            byte correctSkillLevel = currentSkillLevel;

            byte correctSkillPercent = 0;

            ulong minSkillPoints = Formula.GetRequiredSkillPoints(Skill, Player.Vocation, correctSkillLevel);

            while (true)
            {
                ulong maxSkillPoints = Formula.GetRequiredSkillPoints(Skill, Player.Vocation, (byte)(correctSkillLevel + 1) );

                if (currentSkillPoints + SkillPoints < maxSkillPoints)
                {
                    correctSkillPercent = (byte)Math.Max(0, Math.Min(100, Math.Floor(100.0 * (currentSkillPoints + SkillPoints - minSkillPoints) / (maxSkillPoints - minSkillPoints) ) ) );

                    break;
                }
                else
                {
                    correctSkillLevel++;

                    minSkillPoints = maxSkillPoints;
                }
            }

            await Context.AddCommand(new PlayerUpdateSkillCommand(Player, Skill, currentSkillPoints + SkillPoints, correctSkillLevel, correctSkillPercent) );

            if (correctSkillLevel > currentSkillLevel)
            {
                string name;

                switch (Skill)
                {
                    case Skill.MagicLevel:

                        name = "magic level";

                        break;

                    case Skill.Fist:

                        name = "fist fighting";

                        break;

                    case Skill.Club:

                        name = "club fighting";

                        break;

                    case Skill.Sword:

                        name = "sword fighting";

                        break;

                    case Skill.Axe:

                        name = "axe fighting";

                        break;

                    case Skill.Distance:

                        name = "distance fighting";

                        break;

                    case Skill.Shield:

                        name = "shielding";

                        break;

                    case Skill.Fish:

                        name = "fishing";

                        break;

                    default:

                        throw new NotImplementedException();
                }

                Context.AddPacket(Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteCenterGameWindowAndServerLog, "You advanced to " + name + " level " + correctSkillLevel + ".") );

                Context.AddEvent(Player, new PlayerAdvanceSkillEventArgs(Player, Skill, currentSkillLevel, correctSkillLevel) );
            }            
        }
    }
}