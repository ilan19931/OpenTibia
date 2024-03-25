﻿using OpenTibia.Game.Commands;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class BabySealDollHandler : CommandHandler<PlayerUseItemCommand>
    {
        private Dictionary<ushort, ushort> partyTrumpets = new Dictionary<ushort, ushort>() 
        {
            { 7183, 7184 }
        };

        private Dictionary<ushort, ushort> decay = new Dictionary<ushort, ushort>() 
        {
            { 7184, 7183 }
        };

        public override Promise Handle(Func<Promise> next, PlayerUseItemCommand command)
        {
            ushort toOpenTibiaId;

            if (partyTrumpets.TryGetValue(command.Item.Metadata.OpenTibiaId, out toOpenTibiaId) )
            {
                return Context.AddCommand(new PlayerAchievementCommand(command.Player, AchievementConstants.INeedAHugBabySealDoll, new[] { AchievementConstants.INeedAHugPandaTeddy, AchievementConstants.INeedAHugStuffedDragon, AchievementConstants.INeedAHugBabySealDoll, AchievementConstants.INeedAHugSantaDoll }, "I Need a Hug") ).Then( () =>
                {
                    return Context.AddCommand(new ItemTransformCommand(command.Item, toOpenTibiaId, 1) );

                } ).Then( (item) =>
                {
                    _ = Context.AddCommand(new ItemDecayTransformCommand(item, TimeSpan.FromSeconds(3), decay[item.Metadata.OpenTibiaId], 1) );

                    return Promise.Completed;
                } );
            }

            return next();
        }
    }
}