using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ElevenDb
{
    class Block
    {
        public byte IsFirst { get; set; }
        public byte IsDeleted { get; set; }
        public byte[] Data { get; set; }
        public int NextBlock { get; set; }

        public Block(byte[] ByteArray)
        {
            int DataSize = ByteArray.Length - 2 - sizeof(int);
            IsDeleted = ByteArray[0];
            IsFirst = ByteArray[1];
            Data = new byte[DataSize];
            Buffer.BlockCopy(ByteArray, 1, Data, 0, DataSize);
            NextBlock = BitConverter.ToInt32(ByteArray, ByteArray.Length - sizeof(int));
        }
        public Block(byte IsDeleted,byte IsFirst, byte[] Data,int NextBlock)
        {
            this.IsDeleted = IsDeleted;
            this.IsFirst = IsFirst;
            this.Data = Data;
            this.NextBlock = NextBlock;
        }
        public byte[] GetAsByteArray()
        {
            return new byte[1] { IsDeleted }.Concat(new byte[1] { IsFirst }).Concat(Data).Concat(BitConverter.GetBytes(NextBlock)).ToArray();
        }
    }
}
