using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;
using OpenTibia.Network.Packets.Outgoing;
using System;

namespace OpenTibia.Game.CommandHandlers
{
    public class JoinGuildHandler : CommandHandler<PlayerSayCommand>
    {
        public override Promise Handle(Func<Promise> next, PlayerSayCommand command)
        {
            if (command.Message.StartsWith("!joinguild ") )
            {
                string guildName = command.Message.Substring(11);

                Guild guild = Context.Server.Guilds.GetGuildByName(guildName);

                if (guild != null)
                {
                    Guild guild2 = Context.Server.Guilds.GetGuildThatContainsMember(command.Player);

                    if (guild2 == null)
                    {
                        string rankName;

                        if (guild.ContainsInvitation(command.Player, out rankName) )
                        {
                            guild.RemoveInvitation(command.Player);

                            guild.AddMember(command.Player, rankName);

                            Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(MessageMode.Look, "You have joined " + guild.Name + " guild. Leave with !leaveguild command.") );

                            return Promise.Completed;
                        }
                    }
                }

                return Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );
            }

            return next();
        }
    }
}