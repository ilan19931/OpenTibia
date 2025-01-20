﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Common;

namespace OpenTibia.Game.Commands
{
    public class PlayerRookingCommand : Command
    {
        public PlayerRookingCommand(Player player)
        {
            Player = player;    
        }

        public Player Player { get; set; }

        public override Promise Execute()
        {
            Tile tile = Context.Server.Map.GetTile(Context.Server.Config.GameplayRooking.PlayerNewPosition);

            if (tile != null)
            {
                Player.Town = tile;
            }

            return Promise.Completed;
        }
    }
}