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

        internal Storage(string DbPath, Options options)
        {
            fs = new FileStream(DbPath, FileMode.Open);
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
                return new Result<string>(Messages.DbCreateSuccess, ResultType.Success);
            }
            catch
            {
                return new Result<string>(Messages.DbCreateFailure, ResultType.DbCreateFailure);
            }
        }

        internal Result<Record> ReadRecord(int BlockNumber)
        {
            fs.Position = 0;
            List<Block> blockList = new List<Block>();
            byte[] block = new byte[BlockSize];
            do
            {
                fs.Read(block, BlockNumber * BlockSize, BlockSize);
                Block b = new Block(block);
                blockList.Add(b);
                BlockNumber = b.NextBlock;
            } while (BlockNumber != -1);
            Record r = BlockListToRecord(blockList);
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
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Seek(KB * KB + BlockNumber* BlockSize, SeekOrigin.Begin);
            bw.Write(value);
            bw.Close();
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
            fs.Read(new byte[offset], 0, offset);
            while (fs.Position < fs.Length)
            {
                fs.Read(block, 0, block.Length);
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
            {
                byte[] block = new byte[BlockSize];
                byte[] blockData = data.Skip(i * BlockSize).Take(BlockSize).ToArray();
                blockData.CopyTo(block, 0);
                blockList.Add(new Block(0, block, -1));
            }
            return blockList;
        }

        internal Result<int> UpdateRecord(Record record,int blockNumber)
        {
            Result<string> result = DeleteRecord(blockNumber);
            if(result.Message == ResultType.Success)
            {
                Result<int> intResult = WriteRecord(record);
                return intResult;
            }
            return new Result<int>(-1, ResultType.RecordUpdateFailure);
        }

        private Result<string> DeleteRecord(int blockNumber)
        {
            throw new NotImplementedException();
        }

        internal void Close()
        {
            fs.Close();
        }
    }
}
