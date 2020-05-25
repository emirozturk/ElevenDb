using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ElevenDb
{
    internal class Converter
    {
        internal static List<Block> ByteArrayToBlockList(byte[] data, int BlockSize)
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
            byte[] data = Array.Empty<byte>();
            foreach (Block block in blockList)
            {
                data = data.Concat(block.Data).ToArray();
            }

            return new Record(data);
        }
        internal static BitArray StringToBitArray(string Value)
        {
            return IntegerArrayToBitArray(StringToIntegerArray(Value));
        }
        internal static string BitArrayToString(BitArray bitArray)
        {
            return IntegerArrayToString(BitArrayToIntegerArray(bitArray));
        }

        internal static string IntegerArrayToString(int[] array)
        {
            string output = "";
            foreach (int i in array)
                output += IntegerToString(i);
            return output;
        }
        internal static int[] StringToIntegerArray(string Value)
        {
            int index = 0;
            List<int> result = new List<int>();
            int length = Value.Length / sizeof(int);
            while (index < length)
            {
                var charList = Value.Skip(index * sizeof(int)).Take(sizeof(int));
                result.Add(StringToInteger(charList));
                index++;
            }
            return result.ToArray();
        }
        internal static BitArray IntegerArrayToBitArray(int[] array)
        {
            return new BitArray(array);
        }
        internal static int[] BitArrayToIntegerArray(BitArray bitArray)
        {
            int intCount = bitArray.Length / (8 * sizeof(int));
            int[] array = new int[intCount];
            bitArray.CopyTo(array, 0);
            return array;
        }

        internal static int StringToInteger(string Value)
        {
            return BitConverter.ToInt32(Value.Select(x => Convert.ToByte(x)).ToArray());
        }
        internal static int StringToInteger(IEnumerable<char> Value)
        {
            return BitConverter.ToInt32(Value.Select(x => Convert.ToByte(x)).ToArray());
        }
        internal static string IntegerToString(int Value)
        {
            byte[] buffer = BitConverter.GetBytes(Value);
            return new string(buffer.Select(x => Convert.ToChar(x)).ToArray());
        }
    }
}
