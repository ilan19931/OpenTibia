﻿using OpenTibia.Game.Commands;
using OpenTibia.Game.Common;
using OpenTibia.Game.Common.ServerObjects;
using System;
using System.Collections.Generic;

namespace OpenTibia.Game.CommandHandlers
{
    public class RotateItemTransformHandler : CommandHandler<PlayerRotateItemCommand>
    {
        private readonly Dictionary<ushort, ushort> transformations;

        public RotateItemTransformHandler()
        {
            transformations = LuaScope.GetInt16Int16Dictionary(Context.Server.Values.GetValue("values.items.transformation.rotate") );
        }

        public override Promise Handle(Func<Promise> next, PlayerRotateItemCommand command)
        {
            ushort toOpenTibiaId;

            if (transformations.TryGetValue(command.Item.Metadata.OpenTibiaId, out toOpenTibiaId) )
            {
                return Context.AddCommand(new ItemTransformCommand(command.Item, toOpenTibiaId, 1) );
            }

            return next();
        }
    }
}