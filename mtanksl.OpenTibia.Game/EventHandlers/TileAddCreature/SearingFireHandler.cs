﻿using OpenTibia.Common.Structures;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.EventHandlers;
using OpenTibia.Game.Events;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class SearingFireHandler : EventHandler<TileAddCreatureEventArgs>
    {
        private readonly HashSet<ushort> searingFires;

        public SearingFireHandler()
        {
            searingFires = Context.Server.Values.GetUInt16HashSet("values.items.searingFires");
        }

        public override Promise Handle(TileAddCreatureEventArgs e)
        {
            if (e.ToTile.Field)
            {
                foreach (var topItem in e.ToTile.GetItems() )
                {
                    if (searingFires.Contains(topItem.Metadata.OpenTibiaId) )
                    {
                        return Context.AddCommand(new CreatureAttackCreatureCommand(null, e.Creature,

                            new SimpleAttack(null, MagicEffectType.FirePlume, AnimatedTextColor.Orange, 300, 300) ) );
                    }
                }
            }

            return Promise.Completed;
        }
    }
}