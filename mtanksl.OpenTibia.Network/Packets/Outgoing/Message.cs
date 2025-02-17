﻿using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.IO;
using OpenTibia.Security;
using System;

namespace OpenTibia.Network.Packets.Outgoing
{
    public class Message : IMessage
    {
        private ByteArrayMemoryStream stream;

        private ByteArrayStreamWriter writer;

        public Message()
        {
            stream = new ByteArrayMemoryStream();

            writer = new ByteArrayStreamWriter(stream);
        }

        public int Position
        {
            get
            {
                return stream.Position;
            }
        }

        public void Write(byte[] buffer, int offset, int length)
        {
            writer.Write(buffer, offset, length);
        }

        private byte[] Length(byte[] bytes)
        {
            byte[] length = BitConverter.GetBytes( (ushort)bytes.Length );

            return length.Combine(bytes);
        }

        private byte[] Hash(byte[] bytes)
        {
            byte[] hash = BitConverter.GetBytes( Adler32.Generate(bytes) );

            return hash.Combine(bytes);
        }
        
        private byte[] Encrypt(uint[] keys, byte[] bytes)
        {
            int padding = bytes.Length % 8;

            if (padding > 0)
            {
                bytes = bytes.Combine( new byte[8 - padding] );
            }

            return Xtea.Encrypt(bytes, 32, keys);
        }

        public byte[] GetBytes(MessageProtocol type, uint[] keys)
        {
            if (type == MessageProtocol.Raw)
            {
                return stream.GetBytes();
            }
            else if (type == MessageProtocol.Tibia)
            {
                if (keys == null)
                {
                    return Length(Hash(Length(stream.GetBytes() ) ) );
                }

                return Length(Hash(Encrypt(keys, Length(stream.GetBytes() ) ) ) );
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}