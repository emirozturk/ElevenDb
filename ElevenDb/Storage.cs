using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ElevenDb
{
    internal class Storage
    {
        private readonly string DbPath;
        private readonly string TreePath;
        private static int BlockSize;
        private const int KB = 1024;
        private readonly int MetadataSize = 2; //|flag|blocksize|blocks...|
        private FileStream fs;
        internal Storage(string DbPath)
        {
            this.DbPath = DbPath;
            BlockSize = ReadBlockSize().Data * KB;
            TreePath = Path.Combine(Path.GetDirectoryName(DbPath), Path.GetFileNameWithoutExtension(DbPath) + ".tree");
            SetFileClosedProperlyFlag();
        }
        internal static bool DbExists(string DbPath)
        {
            return File.Exists(DbPath);
        }
        internal static Result CreateDb(string DbPath, Options Options)
        {
            Result result = new Result();
            try
            {
                File.Create(DbPath).Close();
                File.WriteAllBytes(DbPath, new byte[2] { 1, Options.BlockSizeinKb }.ToArray());
                result.SetDataWithSuccess(null);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }
            return result;
        }
        internal Result ReadBlocks(int BlockNumber)
        {
            Result result = new Result();
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
                result.SetDataWithSuccess(blockList);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }
            finally
            {
                fs.Close();
            }
            return result;
        }
        private Result ReadInitialBlocks()
        {
            Result result = new Result();
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
                    {
                        initialBlocks.Add(blockNumber);
                    }

                    blockNumber++;
                }
                result.SetDataWithSuccess(initialBlocks);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }
            finally
            {
                fs.Close();
            }
            return result;
        }
        private Result FindEmptyBlocks(int count)
        {
            Result result = new Result();
            try
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
                    {
                        emptyBlocks.Add(blockNumber);
                    }

                    blockNumber++;
                }
                while (emptyBlocks.Count < count)
                {
                    emptyBlocks.Add(blockNumber++);
                }

                result.SetDataWithSuccess(emptyBlocks);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }
            finally
            {
                fs.Close();
            }
            return result;
        }
        private Result SetFileClosedProperlyFlag()
        {
            Result result = new Result();
            try
            {
                FileStream fs = new FileStream(DbPath, FileMode.Open);
                fs.Write(new byte[1] { 1 }, 0, 1);
                result.SetDataWithSuccess(null);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }
            finally
            {
                fs.Close();
            }
            return result;
        }
        private Result UnsetFileClosedProperlyFlag()
        {
            Result result = new Result();
            try
            {
                FileStream fs = new FileStream(DbPath, FileMode.Open);
                fs.Write(new byte[1] { 0 }, 0, 1);
                result.SetDataWithSuccess(null);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }
            finally
            {
                fs.Close();
            }
            return result;
        }
        private Result ReadBlockSize()
        {
            Result result = new Result();
            try
            {
                FileStream fs = new FileStream(DbPath, FileMode.Open);
                fs.Seek(1, SeekOrigin.Begin);
                int recordSize = fs.ReadByte();
                result.SetDataWithSuccess(recordSize);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }
            finally
            {
                fs.Close();
            }
            return result;
        }
        internal Result ReadRecord(int BlockNumber)
        {
            Result result = ReadBlocks(BlockNumber);
            if (result.IsSuccess)
            {
                Record record = Converter.BlockListToRecord(result.Data);
                result.SetDataWithSuccess(record);
            }
            return result;
        }
        internal Result WriteBlock(int BlockNumber, byte[] value)
        {
            Result result = new Result();
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                fs = new FileStream(DbPath, FileMode.Open);
                bw.Seek(MetadataSize + BlockNumber * BlockSize, SeekOrigin.Begin);
                bw.Write(value);
                result.SetDataWithSuccess(null);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }
            finally
            {
                bw.Close();
            }
            return result;
        }
        internal Result WriteRecord(Record record)
        {
            byte[] data = record.ToByteArray();
            List<Block> blockList = Converter.ByteArrayToBlockList(data, BlockSize);
            Result result = FindEmptyBlocks(blockList.Count);
            if (result.IsSuccess)
            {
                List<int> emptyList = result.Data;
                for (int i = 0; i < blockList.Count - 1; i++)
                {
                    blockList[i].NextBlock = emptyList[i + 1];
                }

                for (int i = 0; i < blockList.Count; i++)
                {
                    result = WriteBlock(emptyList[i], blockList[i].GetAsByteArray());
                    if (!result.IsSuccess)
                    {
                        return result;
                    }
                }
                result.SetDataWithSuccess(emptyList[0]);
            }
            return result;
        }

        internal Result ReadAllRecords()
        {
            Result result = ReadInitialBlocks();
            if (result.IsSuccess)
            {
                List<TreeNode> treeNodes = new List<TreeNode>();
                foreach (int blockNumber in result.Data)
                {
                    Result readRecordResult = ReadRecord(blockNumber);
                    if (readRecordResult.IsSuccess)
                    {
                        treeNodes.Add(new TreeNode(readRecordResult.Data.Key, blockNumber));
                    }
                    else
                    {
                        return readRecordResult;
                    }
                }
                result.SetDataWithSuccess(treeNodes);
            }
            return result;
        }
        internal Result WriteTree(string TreeString)
        {
            Result result = new Result();
            try
            {
                File.WriteAllText(TreePath, TreeString);
                result.SetDataWithSuccess(null);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }
            return result;
        }
        internal Result UpdateRecord(Record record, int blockNumber)
        {
            Result result = DeleteRecord(blockNumber);
            if (result.IsSuccess)
            {
                return WriteRecord(record);
            }

            return result;
        }
        internal Result ReadTree()
        {
            Result result = new Result();
            try
            {
                string treeString = File.ReadAllText(TreePath);
                result.SetDataWithSuccess(treeString);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }
            return result;
        }
        internal Result DeleteRecord(int BlockNumber)
        {
            Result result = ReadBlocks(BlockNumber);
            if (result.IsSuccess)
            {
                dynamic blockList = result.Data;
                int blockNumber = BlockNumber;
                foreach (dynamic block in blockList)
                {
                    result = WriteBlock(blockNumber, new Block(1, 0, new byte[0], -1).GetAsByteArray());
                    blockNumber = block.NextBlock;
                    if (!result.IsSuccess)
                    {
                        return result;
                    }
                }
            }
            return result;
        }
        internal Result Close()
        {
            return UnsetFileClosedProperlyFlag();
        }
        internal static Result IsFileClosedProperly(string DbPath)
        {
            Result result = new Result();
            Result flagResult = ReadFileClosedProperlyFlag(DbPath);
            if (flagResult.IsSuccess)
            {
                if (flagResult.Data == 0)
                {
                    result.SetDataWithSuccess(null);
                }
            }

            return result;
        }
        internal static Result ReadFileClosedProperlyFlag(string DbPath)
        {
            Result result = new Result();
            FileStream fs = new FileStream(DbPath, FileMode.Open);
            try
            {
                int firstByte = fs.ReadByte();
                result.SetDataWithSuccess(firstByte);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }
            finally
            {
                fs.Close();
            }
            return result;
        }

    }
}
