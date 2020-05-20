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
        string DbPath;
        static int BlockSize;
        const int KB = 1024;
        FileStream fs;

        internal Storage(string DbPath, Options options)
        {
            this.DbPath = DbPath;
            BlockSize = options.BlockSizeinKb * KB;
        }
        internal static Result<string> DbExists(string dbPath)
        {
            if (File.Exists(dbPath))
                return new Result<string>(Messages.DbExists, ResultType.DbExists);
            else
                return new Result<string>(Messages.DbNotFound, ResultType.DbNotFound);
        }

        internal static Result<string> CreateDb(string dbPath)
        {
            try
            {
                File.Create(dbPath).Close();
                return new Result<string>(Messages.Success, ResultType.Success);
            }
            catch (Exception e)
            {
                return new Result<string>(e.Message, ResultType.DbCreateFailure);
            }
        }
        internal Result<List<Block>> ReadBlock(int BlockNumber)
        {
            try
            {
                fs = new FileStream(DbPath, FileMode.Open);
                List<Block> blockList = new List<Block>();
                byte[] block = new byte[BlockSize];
                do
                {
                    fs.Position = BlockNumber * BlockSize;
                    fs.Read(block, 0, BlockSize);
                    Block b = new Block(block);
                    blockList.Add(b);
                    BlockNumber = b.NextBlock;
                } while (BlockNumber != -1);
                fs.Close();
                return new Result<List<Block>>(blockList, ResultType.Success);
            }
            catch (Exception e)
            {
                fs.Close();
                return new Result<List<Block>>(null, ResultType.BlockReadFailure);
            }
        }
        internal Result<Record> ReadRecord(int BlockNumber)
        {
            var blockListResult = ReadBlock(BlockNumber);
            if (blockListResult.Message == ResultType.Success)
            {
                Record r = BlockListToRecord(blockListResult.Data);
                return new Result<Record>(r, ResultType.Success);
            }
            else
                return new Result<Record>(null, ResultType.RecordReadFailure);
        }

        private Record BlockListToRecord(List<Block> blockList)
        {
            byte[] data = new byte[0];
            foreach (var block in blockList)
                data = data.Concat(block.Data).ToArray();
            return new Record(data);
        }

        internal Result<string> WriteBlock(int BlockNumber, byte[] value)
        {
            try
            {
                fs = new FileStream(DbPath,FileMode.Open);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Seek(BlockNumber * BlockSize, SeekOrigin.Begin);
                bw.Write(value);
                bw.Close();
                return new Result<string>(Messages.Success, ResultType.Success);
            }
            catch (Exception e)
            {
                return new Result<string>(e.Message, ResultType.BlockWriteFailure);
            }
        }

        internal Result<int> WriteRecord(Record record)
        {
            try
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
            catch (Exception e)
            {
                return new Result<int>(-1, ResultType.RecordWriteFailure);
            }
        }

        private List<int> FindEmptyBlocks(int count)
        {
            fs = new FileStream(DbPath, FileMode.Open);
            List<int> emptyBlocks = new List<int>();
            int blockNumber = 0;
            byte[] block = new byte[BlockSize];
            while (fs.Position < fs.Length && emptyBlocks.Count<count)
            {
                fs.Read(block, 0, block.Length);
                if (Convert.ToBoolean(block[0]))
                    emptyBlocks.Add(blockNumber);
                blockNumber++;
            }
            while (emptyBlocks.Count < count)
                emptyBlocks.Add(blockNumber++);
            fs.Close();
            return emptyBlocks;
        }

        private List<Block> ByteArrayToBlockList(byte[] data)
        {
            int MaxDataSize = BlockSize - 1 - sizeof(int);
            List<Block> blockList = new List<Block>();
            int blockCount = data.Length / (MaxDataSize) + 1;
            for (int i = 0; i < blockCount; i++)
            {
                byte[] block = new byte[MaxDataSize];
                byte[] blockData = data.Skip(i * MaxDataSize).Take(MaxDataSize).ToArray();
                blockData.CopyTo(block, 0);
                blockList.Add(new Block(0, block, -1));
            }
            return blockList;
        }

        internal Result<int> UpdateRecord(Record record, int blockNumber)
        {
            Result<string> result = DeleteRecord(blockNumber);
            if (result.Message == ResultType.Success)
            {
                Result<int> intResult = WriteRecord(record);
                return intResult;
            }
            return new Result<int>(-1, ResultType.RecordUpdateFailure);
        }

        internal Result<string> DeleteRecord(int blockNumber)
        {
            var blockListResult = ReadBlock(blockNumber);
            var blockList = blockListResult.Data;
            int BlockNumber = blockNumber;
            foreach (var block in blockList)
            {
                WriteBlock(BlockNumber, new Block(1,new byte[0],-1).GetAsByteArray());
                BlockNumber = block.NextBlock;
            }
            return new Result<string>(Messages.Success, ResultType.Success);
        }

        internal void Close()
        {
            //Not needed
        }
    }
}
