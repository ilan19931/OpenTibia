using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;
using OpenTibia.Network.Packets.Outgoing;
using System;

namespace OpenTibia.Game.CommandHandlers
{
    public class ChangeGuildRankHandler : CommandHandler<PlayerSayCommand>
    {
        public override Promise Handle(Func<Promise> next, PlayerSayCommand command)
        {
            if (command.Message.StartsWith("!changeguildrank ") )
            {
                string[] split = command.Message.Split(" ");

                if (split.Length == 3)
                {
                    string playerName = split[1];

                    Player observer = Context.Server.GameObjects.GetPlayerByName(playerName);

                    if (observer != null && observer != command.Player)
                    {
                        string rankName = split[2];

                        if (rankName.Length >= 3 && rankName.Length <= 29 && !string.Equals(rankName, "Leader", StringComparison.OrdinalIgnoreCase) )
                        {
                            Guild guild = Context.Server.Guilds.GetGuildByLeader(command.Player);

                            if (guild != null)
                            {
                                if (guild.ContainsInvitation(observer, out _) )
                                {
                                    guild.RemoveInvitation(observer);
                                    guild.AddInvitation(observer, rankName);

                                    Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(MessageMode.Look, observer.Name + "'s guild rank has been changed.") );

                                    return Promise.Completed;
                                }
                                else if (guild.ContainsMember(observer, out _) )
                                {
                                    guild.RemoveMember(observer);
                                    guild.AddMember(observer, rankName);

                                    Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(MessageMode.Look, observer.Name + "'s guild rank has been changed.") );

                                    Context.AddPacket(observer, new ShowWindowTextOutgoingPacket(MessageMode.Look, "Your guild's rank has been changed.") );

                                    return Promise.Completed;
                                }
                            }
                        }
                    }                    
                }

                return Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );
            }

            return next();
        }
    }
}