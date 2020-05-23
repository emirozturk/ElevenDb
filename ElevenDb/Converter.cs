using System.Collections.Generic;
using System.Linq;

namespace ElevenDb
{
    class Converter
    {
        internal static List<Block> ByteArrayToBlockList(byte[] data,int BlockSize)
        {
            int MaxDataSize = BlockSize - 2 - sizeof(int);
            List<Block> blockList = new List<Block>();
            int blockCount = data.Length / (MaxDataSize) + 1;
            for (int i = 0; i < blockCount; i++)
            {
                byte[] block = new byte[MaxDataSize];
                byte[] blockData = data.Skip(i * MaxDataSize).Take(MaxDataSize).ToArray();
                blockData.CopyTo(block, 0);
                blockList.Add(new Block(0, 0, block, -1));
            }
            blockList[0].IsFirst = 1;
            return blockList;
        }
        internal static Record BlockListToRecord(List<Block> blockList)
        {
            byte[] data = new byte[0];
            foreach (Block block in blockList)
            {
                data = data.Concat(block.Data).ToArray();
            }

            return new Record(data);
        }
    }
}
