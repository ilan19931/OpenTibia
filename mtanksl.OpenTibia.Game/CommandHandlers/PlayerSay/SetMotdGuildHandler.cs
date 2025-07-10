using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;
using OpenTibia.Network.Packets.Outgoing;
using System;

namespace OpenTibia.Game.CommandHandlers
{
    public class SetMotdGuildHandler : CommandHandler<PlayerSayCommand>
    {
        public override Promise Handle(Func<Promise> next, PlayerSayCommand command)
        {
            if (command.Message.StartsWith("!setmotdguild ") )
            {
                string message = command.Message.Substring(14);

                if ( !string.IsNullOrEmpty(message) && message.Length < 255)
                {
                    Guild guild = Context.Server.Guilds.GetGuildByLeader(command.Player);

                    if (guild != null)
                    {
                        guild.MessageOfTheDay = message;

                        Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(MessageMode.Look, "Message of the day has been set.") );

                        return Promise.Completed;
                    }

                    return Context.AddCommand(new ShowMagicEffectCommand(command.Player, MagicEffectType.Puff) );
                }
            }

            return next();
        }
    }
}