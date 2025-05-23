﻿using OpenTibia.Common.Objects;
using OpenTibia.IO;

namespace OpenTibia.Network.Packets.Incoming
{
    public class EditTextDialogIncomingPacket : IIncomingPacket
    {
        public uint WindowId { get; set; }

        public string Text { get; set; }
        
        public void Read(IByteArrayStreamReader reader, IHasFeatureFlag features)
        {
            WindowId = reader.ReadUInt();

            Text = reader.ReadString();
        }        
    }
}