﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Network.Packets.Outgoing;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class CommandsHandler : CommandHandler<PlayerSayCommand>
    {
        public override Promise Handle(Func<Promise> next, PlayerSayCommand command)
        {
            if (command.Message.StartsWith("!commands") )
            {
                List<string> commands = new List<string>();

                if (command.Player.Rank == Rank.Gamemaster)
                {
                    commands.AddRange(new[]
                    {
                        "/at <n> - Display nth animated text",
                        "/a <n> - Jump n tiles",
                        "/ban <ip_address> - Ban ip address",
                        "/ban <player_name> - Ban player",
                        "/ban <account_name> - Ban account",
                        "/ce <n> - Display nth cooldown effect",
                        "/c <player_name> - Teleport player",
                        "/down - Go down one floor",
                        "/ghost - Invisible",
                        "/goto <player_name> - Go to player",
                        "/i <item_id> [n] - Create an item with n count",
                        "/kick <player_name> - Kick player",
                        "/me <n> - Display nth magic effect",
                        "/m <monster_name> - Create a monster",
                        "/n <npc_name> - Create a NPC",
                        "/pe <n> - Display nth projectile effect",
                        "/raid <raid_name> - Start raid",
                        "/r - Destroy monster, NPC or item",
                        "/t [player_name] - Teleport player to home town",
                        "/town <town_name> - Go to town",
                        "/unban <ip_address> - UnBan ip address",
                        "/unban <player_name> - UnBan player",
                        "/unban <account_name> - UnBan account",
                        "/up - Go up one floor",
                        "/w <waypoint_name> - Go to waypont",
                        "#b <message> - Broadcast",
                        "#c <message> - Channel broadcast",
                        "#d <message> - Channel broadcast anonymous",
                        "@<player_name>@<message> - Send message to player",

                        "/cp - Set a checkpoint",
                        "/gc - Return to a checkpoint",
                        "/tp [seconds] - Create a teleport that will disappear after n seconds",

                        "/poll <question> - Start the poll with a yes-no question",
                        "/endpoll - End the poll"
                    } );
                }
                
                commands.AddRange(new[]
                {
                    "!poll - Display the current poll",
                    "!vote yes - Cast your yes vote",
                    "!vote no - Cast your no vote",

                    "!buyhouse - Buy a house",
                    "!leavehouse - Leave a house",

                    "!createguild <guild_name> - Create a guild",
                    "!inviteguild <player_name> <rank_name> - Invite player to join the guild",
                    "!excludeguild <player_name> - Exclude player or invitation from the guild",
                    "!changeguildrank <player_name> <rank_name> - Change player's guild rank",
                    "!passleadershipguild <player_name> - Pass leadership to player",
                    "!joinguild <guild_name> - Join a guild",
                    "!leaveguild - Leave the guild"
                } );

                string message = string.Join("\n", commands);

                Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(MessageMode.Say, command.Message) );

                foreach (var pair in command.Player.Client.Windows.GetIndexedWindows() )
                {
                    command.Player.Client.Windows.CloseWindow(pair.Key);
                }

                Window window = new Window();

                uint windowId = command.Player.Client.Windows.OpenWindow(window);

                Context.AddPacket(command.Player, new OpenShowOrEditTextDialogOutgoingPacket(windowId, 2819, (ushort)message.Length, message, null, null) );

                return Promise.Completed;
            }

            return next();
        }
    }
}