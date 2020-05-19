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
        public byte[] GetAsByteArray()
        {
            return new byte[1] { IsDeleted }.Concat(Data).Concat(BitConverter.GetBytes(NextBlock)).ToArray();
        }
    }
}
