﻿using OpenTibia.Common.Objects;
using OpenTibia.Game.Common;
using System;

namespace OpenTibia.Game.Commands
{
    public class ParseEditTextDialogCommand : IncomingCommand
    {
        public ParseEditTextDialogCommand(Player player, uint windowId, string text)
        {
            Player = player;

            WindowId = windowId;

            Text = text;
        }

        public Player Player { get; set; }

        public uint WindowId { get; set; }

        public string Text { get; set; }

        public override Promise Execute()
        {
            Window window = Player.Client.Windows.GetWindow(WindowId);

            if (window != null)
            {
                Player.Client.Windows.CloseWindow(WindowId);

                if (window.Item is ReadableItem readableItem && Text != readableItem.Text && Text.Length <= Constants.MaxBookCharacters)
                {
                    readableItem.Text = Text == "" ? null : Text;

                    readableItem.WrittenBy = Player.Name;

                    readableItem.WrittenDate = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                }

                return Promise.Completed;
            }

            return Promise.Break;
        }
    }
}