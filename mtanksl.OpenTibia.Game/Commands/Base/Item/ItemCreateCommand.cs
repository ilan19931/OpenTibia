﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;

namespace OpenTibia.Game.Commands
{
    public class ItemCreateCommand : Command
    {
        public ItemCreateCommand(ushort openTibiaId, Position position)
        {
            OpenTibiaId = openTibiaId;

            Position = position;
        }

        public ushort OpenTibiaId { get; set; }

        public Position Position { get; set; }

        public override void Execute(Context context)
        {
            Item item = context.Server.ItemFactory.Create(OpenTibiaId);

            if (item != null)
            {
                Tile tile = context.Server.Map.GetTile(Position);

                if (tile != null)
                {
                    new TileAddItemCommand(tile, item).Execute(context);

                    base.OnCompleted(context);
                }
            }
        }
    }
}
