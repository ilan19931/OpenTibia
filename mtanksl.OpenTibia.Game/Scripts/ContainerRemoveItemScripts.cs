﻿using OpenTibia.Game.CommandHandlers;
using OpenTibia.Game.Commands;

namespace OpenTibia.Game.Scripts
{
    public class ContainerRemoveItemScripts : Script
    {
        public override void Start()
        {
            Context.Server.CommandHandlers.AddCommandHandler<ContainerRemoveItemCommand>(new ContainerRemoveItemTradingRejectHandler() );       
            
            Context.Server.CommandHandlers.AddCommandHandler<ContainerRemoveItemCommand>(new ContainerRemoveItemNpcTradingUpdateStatsHandler() );

            Context.Server.CommandHandlers.AddCommandHandler<ContainerRemoveItemCommand>(new ContainerRemoveItemUpdatePlayerCapacityHandler() );
        }

        public override void Stop()
        {
            
        }
    }
}