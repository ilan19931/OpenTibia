﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Network.Packets.Outgoing;
using System.Linq;

namespace OpenTibia.Game.Commands
{
    public class WalkCommand : Command
    {
        public WalkCommand(Player player, MoveDirection moveDirection)
        {
            Player = player;

            MoveDirection = moveDirection;
        }

        public Player Player { get; set; }

        public MoveDirection MoveDirection { get; set; }


        private int index = 0;

        public override void Execute(Context context)
        {
            Tile fromTile = Player.Tile;

            Tile toTile = context.Server.Map.GetTile( fromTile.Position.Offset(MoveDirection) );

            if ( toTile == null || toTile.GetItems().Any(i => i.Metadata.Flags.Is(ItemMetadataFlags.NotWalkable) ) || toTile.GetCreatures().Any(c => c.Block) )
            {
                context.AddPacket(Player.Client.Connection, new ShowWindowTextOutgoingPacket(TextColor.WhiteBottomGameWindow, Constants.SorryNotPossible), 
                    
                                                            new StopWalkOutgoingPacket(Player.Direction) );
            }
            else
            {
                if (index++ == 0)
                {
                    context.Server.QueueForExecution(Constants.CreatureAttackSchedulerEvent(Player), 1000 * fromTile.Ground.Metadata.Speed / Player.Speed, this);
                }
                else
                {
                    new CreatureMoveCommand(Player, toTile).Execute(context);
           
                    base.OnCompleted(context);
                }
            }
        }
    }
}