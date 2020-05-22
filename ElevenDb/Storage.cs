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
        string TreePath;
        static int BlockSize;
        const int KB = 1024;
        int MetadataSize = 2; //1 byte flag + 1 byte blocksize
        FileStream fs;

        internal Storage(string DbPath)
        {
            this.DbPath = DbPath;
            BlockSize = ReadBlockSize().Data * KB;
            this.TreePath = Path.Combine(Path.GetDirectoryName(DbPath), Path.GetFileNameWithoutExtension(DbPath) + ".tree");
            SetFileClosedProperlyFlag();
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
                File.Create(dbPath).Close();
                File.WriteAllBytes(dbPath, new byte[2] { 1, options.BlockSizeinKb }.ToArray());
                return new Result<string>(Messages.Success, ResultType.Success);
            }
            catch (Exception e)
            {
                return new Result<string>(e.Message, ResultType.DbCreateFailure);
            }
        }
        internal Result<List<Block>> ReadBlocks(int BlockNumber)
        {
            try
            {
                fs = new FileStream(DbPath, FileMode.Open);
                List<Block> blockList = new List<Block>();
                byte[] block = new byte[BlockSize];
                do
                {
                    fs.Position = MetadataSize + BlockNumber * BlockSize;
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
        private Result<List<int>> ReadInitialBlocks()
        {
            try
            {
                List<int> initialBlocks = new List<int>();
                fs = new FileStream(DbPath, FileMode.Open);
                fs.Seek(MetadataSize, SeekOrigin.Begin);
                byte[] block = new byte[BlockSize];
                int blockNumber = 0;
                while (fs.Position < fs.Length)
                {
                    fs.Read(block, 0, BlockSize);
                    if (block[1] == 1)
                        initialBlocks.Add(blockNumber);
                    blockNumber++;
                }
                fs.Close();
                return new Result<List<int>>(initialBlocks, ResultType.Success);
            }
            catch (Exception e)
            {
                fs.Close();
                return new Result<List<int>>(null, ResultType.BlockReadFailure);
            }
        }

        internal Result<Record> ReadRecord(int BlockNumber)
        {
            var blockListResult = ReadBlocks(BlockNumber);
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
                fs = new FileStream(DbPath, FileMode.Open);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Seek(MetadataSize + BlockNumber * BlockSize, SeekOrigin.Begin);
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
            catch
            {
                return new Result<int>(-1, ResultType.RecordWriteFailure);
            }
        }

        private List<int> FindEmptyBlocks(int count)
        {
            fs = new FileStream(DbPath, FileMode.Open);
            fs.Seek(MetadataSize, SeekOrigin.Begin);
            List<int> emptyBlocks = new List<int>();
            int blockNumber = 0;
            byte[] block = new byte[BlockSize];
            while (fs.Position < fs.Length && emptyBlocks.Count < count)
            {
                fs.Read(block, 0, BlockSize);
                if (Convert.ToBoolean(block[0]))
                    emptyBlocks.Add(blockNumber);
                blockNumber++;
            }
            while (emptyBlocks.Count < count)
                emptyBlocks.Add(blockNumber++);
            fs.Close();
            return emptyBlocks;
        }

        internal List<TreeNode> ReadNodes()
        {
            List<TreeNode> treeNodes = new List<TreeNode>();
            Result<List<int>> initialBlockNumbersResult = ReadInitialBlocks();
            if (initialBlockNumbersResult.Message == ResultType.Success)
            {
                foreach (int blockNumber in initialBlockNumbersResult.Data)
                {
                    var readRecordResult = ReadRecord(blockNumber);
                    if (readRecordResult.Message == ResultType.Success)
                        treeNodes.Add(new TreeNode(readRecordResult.Data.Key, blockNumber));
                }
            }
            return treeNodes;
        }



        private List<Block> ByteArrayToBlockList(byte[] data)
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

        internal Result<string> WriteTree(string TreeString)
        {
            try
            {
                File.WriteAllText(TreePath, TreeString);
                return new Result<string>(Messages.Success, ResultType.Success);
            }
            catch (Exception e)
            {
                return new Result<string>(e.Message, ResultType.TreeWriteFailure);
            }
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

        internal Result<string> ReadTree()
        {
            try
            {
                string treeString = File.ReadAllText(TreePath);
                return new Result<string>(treeString, ResultType.Success);
            }
            catch (Exception e)
            {
                return new Result<string>(e.Message, ResultType.TreeReadFailure);
            }
        }

        internal Result<string> DeleteRecord(int blockNumber)
        {
            var blockListResult = ReadBlocks(blockNumber);
            var blockList = blockListResult.Data;
            int BlockNumber = blockNumber;
            foreach (var block in blockList)
            {
                WriteBlock(BlockNumber, new Block(1, 0, new byte[0], -1).GetAsByteArray());
                BlockNumber = block.NextBlock;
            }
            return new Result<string>(Messages.Success, ResultType.Success);
        }
        internal Result<string> Close()
        {
            return UnsetFileClosedProperlyFlag();
        }

        private Result<string> SetFileClosedProperlyFlag()
        {
            try
            {
                FileStream fs = new FileStream(DbPath, FileMode.Open);
                fs.Write(new byte[1] { 1 }, 0, 1);
                fs.Close();
                return new Result<string>(Messages.Success, ResultType.Success);
            }
            catch (Exception e)
            {
                return new Result<string>(e.Message, ResultType.FlagWriteError);
            }
        }

        private Result<string> UnsetFileClosedProperlyFlag()
        {
            try
            {
                FileStream fs = new FileStream(DbPath, FileMode.Open);
                fs.Write(new byte[1] { 0 }, 0, 1);
                fs.Close();
                return new Result<string>(Messages.Success, ResultType.Success);
            }
            catch (Exception e)
            {
                return new Result<string>(e.Message, ResultType.FlagWriteError);
            }
        }

        internal static Result<string> IsFileClosedProperly(string DbPath)
        {
            var result = ReadFileClosedProperlyFlag(DbPath);
            if (result.Data == 0)
                return new Result<string>(Messages.FileClosedProperly, ResultType.Success);
            else
                return new Result<string>(Messages.FileNotClosedProperly, ResultType.FileCheckFailure);
        }
        internal static Result<int> ReadFileClosedProperlyFlag(string DbPath)
        {
            try
            {
                FileStream fs = new FileStream(DbPath, FileMode.Open);
                int result = fs.ReadByte();
                fs.Close();
                return new Result<int>(result, ResultType.Success);
            }
            catch
            {
                return new Result<int>(-1, ResultType.FlagReadError);
            }
        }
        private Result<int> ReadBlockSize()
        {
            try
            {
                FileStream fs = new FileStream(DbPath, FileMode.Open);
                fs.Seek(1, SeekOrigin.Begin);
                int recordSize = fs.ReadByte();
                fs.Close();
                return new Result<int>(recordSize, ResultType.Success);
            }
            catch
            {
                return new Result<int>(-1, ResultType.FlagReadError);
            }
        }

    }
}
