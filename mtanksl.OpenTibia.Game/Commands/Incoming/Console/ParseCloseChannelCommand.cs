﻿using OpenTibia.Common.Objects;
using OpenTibia.Network.Packets.Outgoing;
using System.Linq;

namespace OpenTibia.Game.Commands
{
    public class ParseCloseChannelCommand : Command
    {
        public ParseCloseChannelCommand(Player player, ushort channelId)
        {
            Player = player;

            ChannelId = channelId;
        }

        public Player Player { get; set; }

        public ushort ChannelId { get; set; }

        public override Promise Execute()
        {
            Channel channel = Context.Server.Channels.GetChannel(ChannelId);

            if (channel != null)
            {
                if (channel.ContainsPlayer(Player) )
                {
                    channel.RemovePlayer(Player);
                }

                if (channel is PrivateChannel privateChannel)
                {
                    if (privateChannel.Owner == Player)
                    {
                        foreach (var observer in privateChannel.GetPlayers().ToList())
                        {
                            Context.AddPacket(observer.Client.Connection, new CloseChannelOutgoingPacket(channel.Id));

                            privateChannel.RemovePlayer(observer);
                        }

                        foreach (var observer in privateChannel.GetInvitations().ToList())
                        {
                            privateChannel.RemoveInvitation(observer);
                        }

                        Context.Server.Channels.RemoveChannel(privateChannel);
                    }
                }

                Context.AddPacket(Player.Client.Connection, new CloseChannelOutgoingPacket(channel.Id) );

                return Promise.Completed;
            }

            return Promise.Break;
        }
    }
}