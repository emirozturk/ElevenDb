using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevenDb
{
    internal class BTree
    {
        private TreeNode Root;
        public BTree() { }
        public BTree(string TreeString)
        {
            CreateFromString(TreeString);
        }

        public BTree(List<TreeNode> NodeList)
        {
            foreach (TreeNode n in NodeList)
            {
                AddRecord(n.Key, n.BlockNumber);
            }
        }
        public override string ToString()
        {
            if (Root == null)
            {
                return string.Empty;
            }

            string treeString = "";
            RootFirstTraverse(Root, ref treeString);
            return treeString;
        }
        private string IntegerToString(int Value)
        {
            byte[] buffer = BitConverter.GetBytes(Value);
            return new string(buffer.Select(x => Convert.ToChar(x)).ToArray());
        }
        private int StringToInteger(string Value)
        {
            return BitConverter.ToInt32(Value.ToCharArray().Select(x => Convert.ToByte(x)).ToArray());
        }
        private void RootFirstTraverse(TreeNode Current, ref string treeString)
        {
            if (Current == null)
            {
                return;
            }

            treeString += IntegerToString(Current.Key.Length);
            treeString += Current.Key;
            treeString += IntegerToString(Current.BlockNumber);

            RootFirstTraverse(Current.Left, ref treeString);
            RootFirstTraverse(Current.Right, ref treeString);
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
        private void CreateFromString(string treeString)
        {
            List<TreeNode> nodeList = StringToNodeList(treeString);
            foreach (TreeNode n in nodeList)
            {
                AddRecord(n.Key, n.BlockNumber);
            }
        }
        private List<TreeNode> StringToNodeList(string treeString)
        {
            List<TreeNode> nodeList = new List<TreeNode>();
            int index = 0;
            int length;
            while (index < treeString.Length)
            {
                length = StringToInteger(treeString.Substring(index, sizeof(int)));
                index += sizeof(int);

                string key = treeString.Substring(index, length);
                index += length;

                int blockNumber = StringToInteger(treeString.Substring(index, sizeof(int)));
                index += sizeof(int);

                nodeList.Add(new TreeNode(key, blockNumber));
            }
            return nodeList;
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
            else if (string.Compare(Key, Current.Key) > 0)
            {
                TreeNode right = Current.Right;
                Search(ref right, Key, ref Result);
            }
            else if (string.Compare(Key, Current.Key) < 0)
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
            else if (string.Compare(Key, Current.Key) > 0)
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
            else if (string.Compare(Key, Current.Key) < 0)
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
        private TreeNode Delete(TreeNode Current, string Key)
        {
            if (Current == null)
            {
                return Current;
            }

            if (string.Compare(Key, Current.Key) > 0)
            {
                Current.Right = Delete(Current.Right, Key);
            }
            else if (string.Compare(Key, Current.Key) < 0)
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

                Current = MinValue(Current.Right);
                Current.Right = Delete(Current.Right, Current.Key);
            }
            return Current;
        }
        private TreeNode MinValue(TreeNode Current)
        {
            TreeNode min = Current;
            while (Current.Left != null)
            {
                min = Current.Left;
                Current = Current.Left;
            }
            return min;
        }

        internal Result GetBlockNumber(string Key)
        {
            Result result = new Result();
            try
            {
                TreeNode node = new TreeNode();
                Search(ref Root, Key, ref node);
                result.SetDataWithSuccess(node.BlockNumber);
            }
            catch (Exception e)
            {
                result.SetMessage(e.Message);
            }
            return result;
        }
        internal Result AddRecord(string Key, int BlockNumber)
        {
            Result result = new Result();
            try
            {
                Add(ref Root, Key, BlockNumber);
                result.SetDataWithSuccess(null);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
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
                result.SetDataWithSuccess(null);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }
            return result;
        }
        internal Result DeleteRecord(string Key)
        {
            Result result = new Result();
            try
            {
                Delete(Root, Key);
                result.SetDataWithSuccess(null);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
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
                result.SetDataWithSuccess(keyList);
            }
            catch (Exception e)
            {
                result.Message = e.Message;
            }
            return result;
        }
    }
}