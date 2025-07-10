using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;
using OpenTibia.Network.Packets.Outgoing;
using System;

namespace OpenTibia.Game.CommandHandlers
{
    public class PassLeadershipGuildHandler : CommandHandler<PlayerSayCommand>
    {
        public override Promise Handle(Func<Promise> next, PlayerSayCommand command)
        {
            if (command.Message.StartsWith("!passleadershipguild ") )
            {
                string playerName = command.Message.Substring(21);

                Player observer = Context.Server.GameObjects.GetPlayerByName(playerName);

                if (observer != null && observer != command.Player)
                {
                    Guild guild = Context.Server.Guilds.GetGuildByLeader(command.Player);

                    if (guild != null)
                    {
                        if (guild.ContainsMember(observer, out _) )
                        {
                            guild.Leader = observer.DatabasePlayerId;

                            guild.RemoveMember(command.Player);
                            guild.AddMember(command.Player, "Vice-Leader");

                            guild.RemoveMember(observer);
                            guild.AddMember(observer, "Leader");

                            Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(MessageMode.Look, observer.Name + " is now the leader of te guild.") );

                            Context.AddPacket(observer, new ShowWindowTextOutgoingPacket(MessageMode.Look, "You are now the leader of the guild.") );

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