﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.IO;

namespace OpenTibia.Network.Packets.Incoming
{
    public class TurnEastIncomingPacket: IIncomingPacket
    {
        public Direction Direction
        {
            get
            {
                return Direction.East;
            }
        }

        public void Read(IByteArrayStreamReader reader, IHasFeatureFlag features)
        {

        }
    }
}