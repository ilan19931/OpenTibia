﻿using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class SantaDollHandler : CommandHandler<PlayerUseItemCommand>
    {
        private readonly HashSet<ushort> santaDolls;

        public SantaDollHandler()
        {
            santaDolls = Context.Server.Values.GetUInt16HashSet("values.items.santaDolls");
        }

        private static List<string> sounds = new List<string>() { "Ho ho ho!", "Jingle bells, jingle bells...", "Have you been naughty?", "Have you been nice?", "Merry Christmas!", "Can you stop squeezing me now... I'm starting to feel a little sick." };

        public override Promise Handle(Func<Promise> next, PlayerUseItemCommand command)
        {
            if (santaDolls.Contains(command.Item.Metadata.OpenTibiaId) )
            {                
                return Context.AddCommand(new PlayerAchievementCommand(command.Player, AchievementConstants.INeedAHugSantaDoll, new[] { AchievementConstants.INeedAHugPandaTeddy, AchievementConstants.INeedAHugStuffedDragon, AchievementConstants.INeedAHugBabySealDoll, AchievementConstants.INeedAHugSantaDoll }, "I Need a Hug") ).Then( () =>
                {
                    return Context.AddCommand(new ShowTextCommand(command.Player, MessageMode.MonsterSay, Context.Server.Randomization.Take(sounds) ) );
                } );
            }

            return next();
        }
    }
}