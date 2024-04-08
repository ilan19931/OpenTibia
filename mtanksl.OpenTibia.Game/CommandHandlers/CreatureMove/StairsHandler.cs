﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class StairsHandler : CommandHandler<CreatureMoveCommand>
    {
        private static HashSet<ushort> stairs = new HashSet<ushort>()
        { 
            // Stairs
            1385, 5258, 1396, 8709, 3687, 3688, 5259, 5260, 
                     
            // Ramps
            1388, 1390, 1392, 1394,

            3679, 3981, 3983, 3985,

            6909, 6911, 6913, 6915, 

            8372, 8374, 8376, 8378,

            // Pyramid
            1398, 1400, 1402, 1404, 1553, 1555, 1557, 1559
        };

        public override Promise Handle(Func<Promise> next, CreatureMoveCommand command)
        {
            Tile stair = command.ToTile;

            if (stair.TopItem != null && stairs.Contains(stair.TopItem.Metadata.OpenTibiaId) )
            {
                Tile toTile;

                Direction direction;

                if (stair.FloorChange == FloorChange.North)
                {
                    toTile = Context.Server.Map.GetTile(stair.Position.Offset(0, -1, -1) );

                    direction = Direction.North;
                }
                else if (stair.FloorChange == FloorChange.East)
                {
                    toTile = Context.Server.Map.GetTile(stair.Position.Offset(1, 0, -1) );

                    direction = Direction.East;
                }
                else if (stair.FloorChange == FloorChange.South)
                {
                    toTile = Context.Server.Map.GetTile(stair.Position.Offset(0, 1, -1) );

                    direction = Direction.South;
                }
                else if (stair.FloorChange == FloorChange.West)
                {
                    toTile = Context.Server.Map.GetTile(stair.Position.Offset(-1, 0, -1) );

                    direction = Direction.West;
                }
                else if (stair.FloorChange == FloorChange.NorthEast)
                {
                    toTile = Context.Server.Map.GetTile(stair.Position.Offset(1, -1, -1) );

                    direction = Direction.East;
                }
                else if (stair.FloorChange == FloorChange.NorthWest)
                {
                    toTile = Context.Server.Map.GetTile(stair.Position.Offset(-1, -1, -1) );

                    direction = Direction.West;
                }
                else if (stair.FloorChange == FloorChange.SouthWest)
                {
                    toTile = Context.Server.Map.GetTile(stair.Position.Offset(-1, 1, -1) );

                    direction = Direction.West;
                }
                else if (stair.FloorChange == FloorChange.SouthEast)
                {
                    toTile = Context.Server.Map.GetTile(stair.Position.Offset(1, 1, -1) );

                    direction = Direction.East;
                }
                else
                {
                    toTile = null;

                    direction = Direction.South;
                }

                if (toTile != null)
                {
                    return Context.AddCommand(new CreatureMoveCommand(command.Creature, toTile, direction) );
                }
            }

            return next();
        }
    }
}