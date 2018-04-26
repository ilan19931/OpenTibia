﻿using OpenTibia.IO;

namespace OpenTibia.Network.Packets.Outgoing
{
    public class ContainerAdd : IOutgoingPacket
    {
        public ContainerAdd(byte containerId, Item item)
        {
            this.ContainerId = containerId;

            this.Item = item;
        }

        public byte ContainerId { get; set; }

        public Item Item { get; set; }
        
        public void Write(ByteArrayStreamWriter writer)
        {
            writer.Write( (byte)0x70 );

            writer.Write(ContainerId);

            writer.Write(Item);
        }
    }
}