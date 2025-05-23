﻿using OpenTibia.Common.Objects;
using OpenTibia.IO;

namespace OpenTibia.Network.Packets.Incoming
{
    public class UseItemWithItemIncomingPacket : IIncomingPacket
    {
        public ushort FromX { get; set; }

        public ushort FromY { get; set; }

        public byte FromZ { get; set; }

        public ushort FromTibiaId { get; set; }

        public byte FromIndex { get; set; }

        public ushort ToX { get; set; }

        public ushort ToY { get; set; }

        public byte ToZ { get; set; }

        public ushort ToTibiaId { get; set; }

        public byte ToIndex { get; set; }
        
        public void Read(IByteArrayStreamReader reader, IHasFeatureFlag features)
        {
            FromX = reader.ReadUShort();

            FromY = reader.ReadUShort();

            FromZ = reader.ReadByte();

            FromTibiaId = reader.ReadUShort();

            FromIndex = reader.ReadByte();

            ToX = reader.ReadUShort();

            ToY = reader.ReadUShort();

            ToZ = reader.ReadByte();

            ToTibiaId = reader.ReadUShort();

            ToIndex = reader.ReadByte();
        }
    }
}