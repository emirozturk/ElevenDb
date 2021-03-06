﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ElevenDb
{
    internal class Storage
    {
        private readonly string DbPath;
        private readonly string TreePath;
        private static int BlockSize;
        private const int KB = 1024;
        private readonly int MetadataSize = 2; //|flag|blocksize|blocks...|
        internal Storage(string DbPath)
        {
            this.DbPath = DbPath;
            BlockSize = ReadBlockSize().Value * KB;
            TreePath = Path.Combine(Path.GetDirectoryName(DbPath), Path.GetFileNameWithoutExtension(DbPath) + ".tree");
            UnsetFileClosedProperlyFlag();
        }
        internal static bool DbExists(string DbPath)
        {
            return File.Exists(DbPath);
        }
        internal static Result CreateDb(string DbPath)
        {
            Result result = new Result();
            try
            {
                File.Create(DbPath).Close();
                File.WriteAllBytes(DbPath, new byte[2] { 1, Options.BlockSizeinKb }.ToArray());
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, null);
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return result;
        }
        internal Result ReadBlocks(int BlockNumber)
        {
            Result result = new Result();
            FileStream fs=null;
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
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, blockList);
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
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
            FileStream fs = null;
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
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, initialBlocks);
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                fs.Close();
            }
            return result;
        }
        internal Result CreateBlockMap()
        {
            Result result = new Result();
            FileStream fs = null;
            try
            {
                List<int> blocks = new List<int>();
                fs = new FileStream(DbPath, FileMode.Open);
                fs.Seek(MetadataSize, SeekOrigin.Begin);
                byte[] block = new byte[BlockSize];
                int blockNumber = 0;
                while (fs.Position < fs.Length)
                {
                    fs.Read(block, 0, BlockSize);
                    blocks.Add(block[0]);
                    blockNumber++;
                }
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, blocks);
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
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
            FileStream fs = null;
            fs.Seek(MetadataSize, SeekOrigin.Begin);
            List<int> emptyBlocks = new List<int>();
            int blockNumber = 0;
            byte[] block = new byte[BlockSize];
            long length = fs.Length;
            while (fs.Position < length && emptyBlocks.Count < count)
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

            result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, emptyBlocks);
            return result;
        }
        private Result SetFileClosedProperlyFlag()
        {
            Result result = new Result();
            FileStream fs = null;
            try
            {
                fs = new FileStream(DbPath, FileMode.Open);
                fs.Write(new byte[1] { 1 }, 0, 1);
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, null);
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                fs.Close();
            }
            return result;
        }
        internal Result UnsetFileClosedProperlyFlag()
        {
            Result result = new Result();
            FileStream fs = null;
            try
            {
                fs = new FileStream(DbPath, FileMode.Open);
                fs.Write(new byte[1] { 0 }, 0, 1);
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, null);
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
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
            FileStream fs = null;
            try
            {
                fs = new FileStream(DbPath, FileMode.Open);
                fs.Seek(1, SeekOrigin.Begin);
                int recordSize = fs.ReadByte();
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, recordSize);
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                fs.Close();
            }
            return result;
        }
        internal Result ReadRecord(int BlockNumber)
        {
            Record record = new Record("", "");
            Result result = new Result();
            if (BlockNumber == -1)
            {
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, record);
            }
            else
            {
                result = ReadBlocks(BlockNumber);
                if (result.IsSuccess)
                {
                    record = Converter.BlockListToRecord(result.Value);
                    result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, record);
                }
            }
            return result;
        }
        private void WriteBlock(FileStream fs,int BlockNumber, byte[] value)
        {
            fs.Seek(MetadataSize + BlockNumber * BlockSize, SeekOrigin.Begin);
            fs.Write(value, 0, value.Length);
        }
        internal Result WriteRecord(Record record, List<int> emptyBlocks)
        {
            Result result = new Result();
            List<Block> blockList = Converter.ByteArrayToBlockList(record.ToByteArray(), BlockSize);
            FileStream fs = null;
            try
            {
                fs = new FileStream(DbPath, FileMode.Open);
                for (int i = 0; i < blockList.Count - 1; i++)
                {
                    blockList[i].NextBlock = emptyBlocks[i + 1];
                }
                for (int i = 0; i < blockList.Count; i++)
                {
                    WriteBlock(fs,emptyBlocks[i], blockList[i].GetAsByteArray());
                }
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, emptyBlocks);
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                fs.Close();
            }
            return result;
        }

        internal Result ReadAllRecords()
        {
            Result result = ReadInitialBlocks();
            if (result.IsSuccess)
            {
                List<TreeNode> treeNodes = new List<TreeNode>();
                foreach (int blockNumber in result.Value)
                {
                    Result readRecordResult = ReadRecord(blockNumber);
                    if (readRecordResult.IsSuccess)
                    {
                        treeNodes.Add(new TreeNode(readRecordResult.Value.Key, blockNumber));
                    }
                    else
                    {
                        return readRecordResult;
                    }
                }
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, treeNodes);
            }
            return result;
        }
        internal Result WriteTree(string TreeString)
        {
            Result result = new Result();
            try
            {
                File.WriteAllText(TreePath, TreeString);
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, null);
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return result;
        }

        internal Result ReadTree()
        {
            Result result = new Result();
            try
            {
                string treeString = File.ReadAllText(TreePath);
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, treeString);
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return result;
        }
        internal Result DeleteRecord(int BlockNumber)
        {
            Result result = new Result();
            FileStream fs = null;
            if (BlockNumber == -1)
            {
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, new int[1] { -1 });
            }
            else
            {
                result = ReadBlocks(BlockNumber);
                if (result.IsSuccess)
                {
                    List<Block> blockList = (List<Block>)result.Value;
                    List<int> blockNumberList = new List<int>();
                    int blockNumber = BlockNumber;
                    try
                    {
                        fs = new FileStream(DbPath, FileMode.Open);
                        foreach (Block block in blockList)
                        {
                            blockNumberList.Add(BlockNumber);
                            WriteBlock(fs,blockNumber, new Block(1, 0, Array.Empty<byte>(), -1).GetAsByteArray());
                            blockNumber = block.NextBlock;
                        }
                        result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, blockNumberList);
                    }
                    catch (Exception e)
                    {
                        result.Message = e.Message;
                    }
                    finally
                    {
                        fs.Close();
                    }
                }
            }
            return result;
        }
        internal Result Close()
        {
            return SetFileClosedProperlyFlag();
        }
        internal static Result IsFileClosedProperly(string DbPath)
        {
            Result result = ReadFileClosedProperlyFlag(DbPath);
            if (result.IsSuccess)
            {
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, Convert.ToBoolean(result.Value));
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
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, firstByte);
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
            }
            finally
            {
                fs.Close();
            }
            return result;
        }
    }
}
