using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ElevenDb
{
    internal class BTree
    {
        private TreeNode Root;
        private BitArray blockMap;

        public BTree()
        {
            blockMap = new BitArray(1024);
        }
        public BTree(string TreeString)
        {
            CreateFromString(TreeString);
        }

        public BTree(List<TreeNode> NodeList, List<int> BlockMap)
        {
            foreach (TreeNode n in NodeList)
            {
                AddNode(n.Key, n.BlockNumber);
            }
            CreateMapFromBlockList(BlockMap);
        }
        private void CreateFromString(string treeString)
        {
            int blockMaplength = Converter.StringToInteger(treeString.Substring(0, sizeof(int)));
            blockMap = Converter.StringToBitArray(treeString.Substring(sizeof(int), blockMaplength));
            List<TreeNode> nodeList = StringToNodeList(treeString.Substring(blockMaplength + sizeof(int)));
            foreach (TreeNode n in nodeList)
            {
                AddNode(n.Key, n.BlockNumber);
            }
        }

        public override string ToString()
        {
            string blockMapString = Converter.BitArrayToString(blockMap);
            blockMapString = Converter.IntegerToString(blockMapString.Length) + blockMapString;

            StringBuilder resultString = new StringBuilder();
            if (Root != null)
            {
                RootFirstTraverse(Root, ref resultString);
            }
            return blockMapString + resultString.ToString();
        }
        private void ResizeBlockMap(int number)
        {
            while (blockMap.Length <= number)
            {
                blockMap.Length += 1024;
            }
        }
        internal void SetBlockMap(List<int> BlockList)
        {
            foreach (int number in BlockList)
            {
                if (number >= blockMap.Length)
                {
                    ResizeBlockMap(number);
                }

                blockMap[number] = true;
            }
        }

        internal void UnSetBlockMap(List<int> BlockList)
        {
            foreach (int number in BlockList)
            {
                blockMap[number] = false;
            }
        }
        private static List<TreeNode> StringToNodeList(string treeString)
        {
            List<TreeNode> nodeList = new List<TreeNode>();
            int index = 0;
            int length;
            while (index < treeString.Length)
            {
                length = Converter.StringToInteger(treeString.Substring(index, sizeof(int)));
                index += sizeof(int);

                string key = treeString.Substring(index, length);
                index += length;

                int blockNumber = Converter.StringToInteger(treeString.Substring(index, sizeof(int)));
                index += sizeof(int);

                nodeList.Add(new TreeNode(key, blockNumber));
            }
            return nodeList;
        }
        private static KeyValuePair<string, int> MinValue(TreeNode Current)
        {
            KeyValuePair<string, int> min = new KeyValuePair<string, int>(Current.Key, Current.BlockNumber);
            while (Current.Left != null)
            {
                min = new KeyValuePair<string, int>(Current.Left.Key, Current.Left.BlockNumber);
                Current = Current.Left;
            }
            return min;
        }
        private void RootFirstTraverse(TreeNode Current, ref StringBuilder treeString)
        {
            if (Current == null)
            {
                return;
            }

            treeString.Append(Converter.IntegerToString(Current.Key.Length));
            treeString.Append(Current.Key);
            treeString.Append(Converter.IntegerToString(Current.BlockNumber));

            RootFirstTraverse(Current.Left, ref treeString);
            RootFirstTraverse(Current.Right, ref treeString);
        }

        internal List<int> GetEmptyBlocks(int BlockCount)
        {
            List<int> emptyBlocks = new List<int>();
            int blockNumber = 0;
            int length = blockMap.Length;
            while (emptyBlocks.Count < BlockCount && blockNumber < length)
            {
                if (!blockMap[blockNumber])
                {
                    emptyBlocks.Add(blockNumber);
                }

                blockNumber++;
            }
            while (emptyBlocks.Count < BlockCount)
            {
                emptyBlocks.Add(blockNumber++);
            }

            return emptyBlocks;
        }

        private void GetKeys(TreeNode Current, ref List<string> keys)
        {
            if (Current == null)
            {
                return;
            }

            keys.Add(Current.Key);
            GetKeys(Current.Left, ref keys);
            GetKeys(Current.Right, ref keys);
        }

        private void Search(ref TreeNode Current, string Key, ref TreeNode Result)
        {
            if (Current == null)
            {
                return;
            }

            if (Key == Current.Key)
            {
                Result = Current;
            }
            else if (string.Compare(Key, Current.Key, StringComparison.Ordinal) > 0)
            {
                TreeNode right = Current.Right;
                Search(ref right, Key, ref Result);
            }
            else if (string.Compare(Key, Current.Key, StringComparison.Ordinal) < 0)
            {
                TreeNode left = Current.Left;
                Search(ref left, Key, ref Result);
            }
        }

        private void Add(ref TreeNode Current, string Key, int BlockNumber)
        {
            if (Current == null)
            {
                Current = new TreeNode(Key, BlockNumber);
            }
            else if (string.Compare(Key, Current.Key, StringComparison.Ordinal) > 0)
            {
                if (Current.Right == null)
                {
                    Current.Right = new TreeNode(Key, BlockNumber);
                }
                else
                {
                    TreeNode right = Current.Right;
                    Add(ref right, Key, BlockNumber);
                }
            }
            else if (string.Compare(Key, Current.Key, StringComparison.Ordinal) < 0)
            {
                if (Current.Left == null)
                {
                    Current.Left = new TreeNode(Key, BlockNumber);
                }
                else
                {
                    TreeNode left = Current.Left;
                    Add(ref left, Key, BlockNumber);
                }
            }
        }

        internal void CreateMapFromBlockList(List<int> BlockList)
        {
            int count = BlockList.Count;
            int limit = 1024;
            while (limit < count)
            {
                limit += 1024;
            }

            blockMap = new BitArray(limit);
            for (int i = 0; i < BlockList.Count; i++)
            {
                blockMap[i] = Convert.ToBoolean(BlockList[i]);
            }
        }

        private TreeNode Delete(TreeNode Current, string Key)
        {
            if (Current == null)
            {
                return Current;
            }

            if (string.Compare(Key, Current.Key, StringComparison.Ordinal) > 0)
            {
                Current.Right = Delete(Current.Right, Key);
            }
            else if (string.Compare(Key, Current.Key, StringComparison.Ordinal) < 0)
            {
                Current.Left = Delete(Current.Left, Key);
            }
            else
            {
                if (Current.Left == null)
                {
                    return Current.Right;
                }
                else if (Current.Right == null)
                {
                    return Current.Left;
                }

                KeyValuePair<string, int> kv = MinValue(Current.Right);
                Current.Key = kv.Key;
                Current.BlockNumber = kv.Value;
                Current.Right = Delete(Current.Right, Current.Key);
            }
            return Current;
        }

        internal Result GetBlockNumber(string Key)
        {
            Result result = new Result();
            try
            {
                TreeNode node = new TreeNode();
                Search(ref Root, Key, ref node);
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, node.BlockNumber);
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return result;
        }
        internal Result AddNode(string Key, int BlockNumber)
        {
            Result result = new Result();
            try
            {
                Add(ref Root, Key, BlockNumber);
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, $"TreeNode: Key={Key} Value={BlockNumber}");
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return result;
        }
        internal Result UpdateRecord(string Key, int BlockNumber)
        {
            Result result = new Result();
            try
            {
                TreeNode n = new TreeNode();
                Search(ref Root, Key, ref n);
                n.BlockNumber = BlockNumber;
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, $"TreeNode: Key={Key} Value={BlockNumber}");
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return result;
        }
        internal Result DeleteRecord(string Key)
        {
            Result result = new Result();
            try
            {
                Root = Delete(Root, Key);
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, $"TreeNode: Key={Key}");
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return result;
        }
        internal Result GetKeys()
        {
            Result result = new Result();
            try
            {
                List<string> keyList = new List<string>();
                GetKeys(Root, ref keyList);
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, keyList);
            }
            catch (Exception e)
            {
                result.SetMessage(MethodBase.GetCurrentMethod().Name, e.Message);
            }
            return result;
        }
    }
}