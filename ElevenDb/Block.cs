using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ElevenDb
{
    class Block
    {
        byte IsDeleted { get; set; }
        byte[] Data { get; set; }
        int NextBlock { get; set; }

        public Block(byte[] ByteArray)
        {
            int DataSize = ByteArray.Length - 1 - sizeof(int);
            IsDeleted = ByteArray[0];
            Data = new byte[DataSize];
            Buffer.BlockCopy(ByteArray, 1, Data, 0, DataSize);
            NextBlock = BitConverter.ToInt32(ByteArray, ByteArray.Length - sizeof(int));
        }

        public byte[] GetAsByteArray()
        {
            return new byte[1] { IsDeleted }.Concat(Data).Concat(BitConverter.GetBytes(NextBlock)).ToArray();
        }
    }
}
