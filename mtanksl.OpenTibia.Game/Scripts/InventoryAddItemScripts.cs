﻿using OpenTibia.Game.CommandHandlers;

namespace OpenTibia.Game.Scripts
{
    public class InventoryAddItemScripts : Script
    {
        public override void Start()
        {
            Context.Server.EventHandlers.Subscribe(new InventoryAddItemScriptingHandler() );

            Context.Server.CommandHandlers.AddCommandHandler(new InventoryAddItemNpcTradingUpdateStatsHandler() );
        }

        public override void Stop()
        {
            
        }
    }
}