﻿using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Network.Packets.Outgoing;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class DestroyFieldHandler : CommandHandler<PlayerUseItemWithItemCommand>
    {
        private HashSet<ushort> destroyFields = new HashSet<ushort>() { 2261 };

        private HashSet<ushort> fields = new HashSet<ushort>() { 1492, 1493, 1494, 1495, 1496 };

        public override Promise Handle(Func<Promise> next, PlayerUseItemWithItemCommand command)
        {
            if (destroyFields.Contains(command.Item.Metadata.OpenTibiaId) )
            {
                if (fields.Contains(command.ToItem.Metadata.OpenTibiaId) )
                {
                    return Context.AddCommand(new ItemDecrementCommand(command.Item, 1) ).Then( () =>
                    {
                        return Context.AddCommand(new ShowMagicEffectCommand(command.ToItem, MagicEffectType.Puff) );

                    } ).Then( () =>
                    {
                        return Context.AddCommand(new ItemDestroyCommand(command.ToItem) );
                    } );
                }

                Context.AddPacket(command.Player, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.YouCanNotUseThisObject) );

                return Promise.Break;
            }

            return next();
        }
    }
}