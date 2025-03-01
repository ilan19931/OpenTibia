﻿using OpenTibia.Common.Objects;
using OpenTibia.Network.Packets.Outgoing;
using System;
using System.Collections.Generic;

namespace OpenTibia.Tests
{
    public class MockMessageCollection : IMessageCollection
    {
        private List<IOutgoingPacket> outgoingPackets = new List<IOutgoingPacket>();

        public IEnumerable<IOutgoingPacket> OutgoingPackets
        {
            get
            {
                return outgoingPackets;
            }
        }

        public void Add(IOutgoingPacket packet)
        {
            outgoingPackets.Add(packet);
        }

        public IEnumerable<IMessage> GetMessages() { throw new NotImplementedException(); }
    }
}