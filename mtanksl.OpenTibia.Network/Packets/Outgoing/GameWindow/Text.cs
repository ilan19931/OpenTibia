﻿using OpenTibia.IO;

namespace OpenTibia.Network.Packets.Outgoing
{
    public class Text : IOutgoingPacket
    {
        public Text(TextColor textColor, string message)
        {
            this.TextColor = textColor;

            this.Message = message;
        }

        public TextColor TextColor { get; set; }

        public string Message { get; set; }
        
        public void Write(ByteArrayStreamWriter writer)
        {
            writer.Write( (byte)0xB4 );

            writer.Write( (byte)TextColor );

            writer.Write(Message);
        }
    }
}