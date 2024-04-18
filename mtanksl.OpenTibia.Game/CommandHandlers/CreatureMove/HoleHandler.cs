﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class HoleHandler : CommandHandler<CreatureMoveCommand>
    {
        private static HashSet<ushort> holes = new HashSet<ushort>() 
        { 
            // Pitfall
            294,

            // Hole
            383, 469, 470, 482, 484, 485, 

            // Invisible
            459, 

            // Wooden
            369, 370, 408, 409, 410, 411, 8276, 8277, 8279, 8280, 8281, 8282, 
            
            // Wooden
            423, 427, 428, 429, 8283, 8284, 8285, 8286, 
                        
            // White marble
            432, 433, 9606,

            // Dark wooden
            3135, 3136, 3137, 3138,
                       
            // Stone
            3219, 3220,

            // Black marble
            4834, 4835, 8170,

            // Gray marble
            4836, 4837,

            // Flat roof
            924, 5081,

            // Grass roof
            6173, 6174,
                      
            // Stone
            475, 476, 479, 480, 6127, 6128, 6129, 6130,

            // Snow
            6917, 6918, 6919, 6920, 6921, 6922, 6923, 6924,

            // Earth
            8559, 8560, 8561, 8562, 8563, 8564, 8565, 8566,

            // Corkscrew
            9574, 9846
        };

        public override Promise Handle(Func<Promise> next, CreatureMoveCommand command)
        {
            Tile hole = command.ToTile;

            if (hole.Ground != null && holes.Contains(hole.Ground.Metadata.OpenTibiaId) )
            {
                Tile down = Context.Server.Map.GetTile(hole.Position.Offset(0, 0, 1) );

                if (down != null)
                {
                    Tile toTile;

                    Direction direction;

                    if (down.FloorChange == FloorChange.North)
                    {
                        toTile = Context.Server.Map.GetTile(down.Position.Offset(0, 1, 0) );

                        direction = Direction.South;
                    }
                    else if (down.FloorChange == FloorChange.East)
                    {
                        toTile = Context.Server.Map.GetTile(down.Position.Offset(-1, 0, 0) );

                        direction = Direction.West;
                    }
                    else if (down.FloorChange == FloorChange.South)
                    {
                        toTile = Context.Server.Map.GetTile(down.Position.Offset(0, -1, 0) );

                        direction = Direction.North;
                    }
                    else if (down.FloorChange == FloorChange.West)
                    {
                        toTile = Context.Server.Map.GetTile(down.Position.Offset(1, 0, 0) );

                        direction = Direction.East;
                    }
                    else if (down.FloorChange == FloorChange.NorthEast)
                    {
                        toTile = Context.Server.Map.GetTile(down.Position.Offset(-1, 1, 0) );

                        direction = Direction.West;
                    }
                    else if (down.FloorChange == FloorChange.NorthWest)
                    {
                        toTile = Context.Server.Map.GetTile(down.Position.Offset(1, 1, 0) );

                        direction = Direction.East;
                    }
                    else if (down.FloorChange == FloorChange.SouthWest)
                    {
                        toTile = Context.Server.Map.GetTile(down.Position.Offset(1, -1, 0) );

                        direction = Direction.East;
                    }
                    else if (down.FloorChange == FloorChange.SouthEast)
                    {
                        toTile = Context.Server.Map.GetTile(down.Position.Offset(-1, -1, 0) );

                        direction = Direction.West;
                    }
                    else
                    {
                        toTile = down;

                        direction = Direction.South;
                    }

                    if (toTile != null)
                    {
                        return Context.AddCommand(new CreatureMoveCommand(command.Creature, toTile) ).Then( () =>
                        {
                            return Context.AddCommand(new CreatureUpdateDirectionCommand(command.Creature, direction) );
                        } );
                    }
                }
            }

            return next();
        }
    }
}