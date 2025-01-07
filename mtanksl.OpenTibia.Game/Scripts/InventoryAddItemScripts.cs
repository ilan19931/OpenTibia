﻿using OpenTibia.Game.CommandHandlers;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Events;

namespace OpenTibia.Game.Scripts
{
    public class InventoryAddItemScripts : Script
    {
        public override void Start()
        {
            Context.Server.EventHandlers.Subscribe<InventoryAddItemEventArgs>(new InventoryAddItemScriptingHandler() );

            Context.Server.EventHandlers.Subscribe<InventoryAddItemEventArgs>(new RingEquipHandler() );

            Context.Server.EventHandlers.Subscribe<InventoryAddItemEventArgs>(new HelmetOfTheDeepEquipHandler() );

            Context.Server.CommandHandlers.AddCommandHandler<InventoryAddItemCommand>(new InventoryAddItemNpcTradingUpdateStatsHandler() );
        }

        public override void Stop()
        {
            
        }
    }
}