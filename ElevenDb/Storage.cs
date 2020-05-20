using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace ElevenDb
{
    class Storage
    {
        static int BlockSize;
        const int KB = 1024;
        readonly FileStream fs;

        internal Storage(string DbPath)
        {
            fs = new FileStream(DbPath, FileMode.Open);
        }
        internal static Result<string> DbExists(string dbPath)
        {
            if (File.Exists(dbPath))
                return new Result<string>(Messages.DbExists, ResultType.DbExists);
            else
                return new Result<string>(Messages.DbNotFound, ResultType.DbNotFound);
        }

        internal static Result<string> CreateDb(string dbPath, Options options)
        {
            try
            {
                File.Create(dbPath);
                BlockSize = options.BlockSizeinKb * KB;
                return new Result<string>(Messages.Success, ResultType.Success);
            }
            catch
            {
                return new Result<string>(Messages.Success, ResultType.DbCreateFailure);
            }
        }

        internal Result<Record> ReadRecord(int BlockNumber)
        {
            fs.Position = 0;
            Record r;
            if (BlockNumber == 0)
            {
                byte[] treeBlock = new byte[1024 * KB];
                fs.Read(treeBlock, 0, 1024 * KB);
                Block b = new Block(treeBlock);
                r = BlockListToRecord(new List<Block> { b });
                return new Result<Record>(r, ResultType.Success);
            }
            else
            {
                List<Block> blockList = new List<Block>();
                byte[] block = new byte[BlockSize];
                do
                {
                    fs.Read(block, BlockNumber * BlockSize, 1);
                    Block b = new Block(block);
                    blockList.Add(b);
                    BlockNumber = b.NextBlock;
                } while (BlockNumber != -1);
                r = BlockListToRecord(blockList);
            }
            return new Result<Record>(r, ResultType.Success);
        }

        private Record BlockListToRecord(List<Block> blockList)
        {
            byte[] data = new byte[0];
            foreach (var block in blockList)
                data.Concat(block.Data);
            return new Record(data);
        }

        internal void WriteBlock(int BlockNumber, byte[] value)
        {
            fs.Position = 0;
            if (BlockNumber == 0)
            {
                byte[] tree = new byte[KB * KB];
                value.CopyTo(tree, 0);
                fs.Write(tree, 0, tree.Length);
            }
            else
            {
                fs.Write(value, BlockSize * BlockNumber, BlockSize);
            }
        }

        internal Result<int> WriteRecord(Record record)
        {
            byte[] data = record.ToByteArray();
            List<Block> blockList = ByteArrayToBlockList(data);
            List<int> emptyList = FindEmptyBlocks(blockList.Count);
            for (int i = 0; i < blockList.Count - 1; i++)
                blockList[i].NextBlock = emptyList[i + 1];
            for (int i = 0; i < blockList.Count; i++)
                WriteBlock(emptyList[i], blockList[i].GetAsByteArray());
            return new Result<int>(emptyList[0], ResultType.Success);
        }

        private List<int> FindEmptyBlocks(int count)
        {
            fs.Position = 0;
            List<int> emptyBlocks = new List<int>();
            int offset = KB * KB;
            int blockNumber = 1;
            byte[] block = new byte[BlockSize];
            fs.Read(new byte[offset], 0, 1);
            while (fs.CanRead)
            {
                fs.Read(block, 0, 1);
                if (Convert.ToBoolean(new Block(block).IsDeleted))
                    emptyBlocks.Add(blockNumber++);
            }
            while (emptyBlocks.Count < count)
            {
                emptyBlocks.Add(blockNumber++);
            }
            return emptyBlocks;
        }

        private List<Block> ByteArrayToBlockList(byte[] data)
        {
            List<Block> blockList = new List<Block>();
            int blockCount = data.Length / (BlockSize - 1 - sizeof(int)) + 1;
            for (int i = 0; i < blockCount; i++)
                blockList.Add(new Block(0, data.Skip(i * BlockSize).Take(BlockSize).ToArray(), -1));
            return blockList;
        }

        internal Result<string> UpdateRecord(Record record)
        {
            throw new NotImplementedException();
        }

        internal void Close()
        {
            fs.Close();
        }
    }
}
