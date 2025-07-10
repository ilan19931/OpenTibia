using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;
using OpenTibia.Network.Packets.Outgoing;
using System;

namespace OpenTibia.Game.CommandHandlers
{
    public class CreateGuildHandler : CommandHandler<PlayerSayCommand>
    {
        public override Promise Handle(Func<Promise> next, PlayerSayCommand command)
        {
            if (command.Message.StartsWith("!createguild ") )
            {                
                string guildName = command.Message.Substring(13);

                Guild guild = Context.Server.Guilds.GetGuildByName(guildName);

                if (guild == null)
                {
                    guild = Context.Server.Guilds.GetGuildThatContainsMember(command.Player);

                    if (guild == null)
                    {
                        guild = new Guild()
                        {
                            Name = guildName,

                            Leader = command.Player.DatabasePlayerId
                        };

                        guild.AddMember(command.Player, "Leader");

                        Context.Server.Guilds.AddGuild(guild);

                        Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(MessageMode.Look, "Guild " + guild.Name + " has been created. Invite players with !inviteguild <player_name> <rank_name> command. Leave with !leaveguild command.") );

                        return Promise.Completed;
                    }
                }

                return Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );
            }
                
            return next();
        }
    }
}