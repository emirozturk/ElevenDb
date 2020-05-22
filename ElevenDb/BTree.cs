using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Transactions;

namespace ElevenDb
{
    internal class BTree
    {
        TreeNode Root;
        public BTree()
        {

        }

        public BTree(string TreeString)
        {
            CreateFromString(TreeString);
        }
        public BTree(List<TreeNode> NodeList)
        {
            foreach (var n in NodeList)
                AddRecord(n.Key, n.BlockNumber);
        }
        public override string ToString()
        {
            if (Root == null) return String.Empty;
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
            if (Current == null) return;
            treeString += IntegerToString(Current.Key.Length);            
            treeString += Current.Key;
            treeString += IntegerToString(Current.BlockNumber);

            RootFirstTraverse(Current.Left, ref treeString);
            RootFirstTraverse(Current.Right, ref treeString);
        }

        private void CreateFromString(string treeString)
        {
            List<TreeNode> nodeList = StringToNodeList(treeString);
            foreach (var n in nodeList)
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

        internal Result<int> GetBlockNumber(string Key)
        {
            TreeNode result = new TreeNode(Key, -1);
            Search(ref Root, Key, ref result);
            if (result.BlockNumber == -1)
                return new Result<int>(-1, ResultType.NotFound);
            else
                return new Result<int>(result.BlockNumber, ResultType.Success);
        }

        internal Result<string> AddRecord(string Key, int BlockNumber)
        {
            try
            {
                Add(ref Root, Key, BlockNumber);
                return new Result<string>(Messages.TreeInsertionSuccess, ResultType.Success);
            }
            catch
            {
                return new Result<string>(Messages.TreeInsertionFailure, ResultType.TreeInsertionFailure);
            }
        }

        internal void UpdateRecord(string Key, int BlockNumber)
        {
            TreeNode n = new TreeNode(Key, -1);
            Search(ref Root, Key, ref n);
            n.BlockNumber = BlockNumber;
        }

        private void Search(ref TreeNode Current, string Key, ref TreeNode Result)
        {
            if (Current == null) return;
            if (Key == Current.Key)
                Result = Current;
            else if (String.Compare(Key,Current.Key) > 0)
            {
                TreeNode right = Current.Right;
                Search(ref right, Key, ref Result);
            }
            else if (String.Compare(Key,Current.Key) < 0)
            {
                TreeNode left = Current.Left;
                Search(ref left, Key, ref Result);
            }
        }

        private void Add(ref TreeNode Current, string Key, int BlockNumber)
        {
            if (Current == null)
                Current = new TreeNode(Key, BlockNumber);
            else if (String.Compare(Key, Current.Key) > 0)
            {
                if (Current.Right == null)
                    Current.Right = new TreeNode(Key, BlockNumber);
                else
                {
                    TreeNode right = Current.Right;
                    Add(ref right, Key, BlockNumber);
                }
            }
            else if (String.Compare(Key, Current.Key) < 0)
            {
                if (Current.Left == null)
                    Current.Left = new TreeNode(Key, BlockNumber);
                else
                {
                    TreeNode left = Current.Left;
                    Add(ref left, Key, BlockNumber);
                }
            }
        }

        internal Result<string> DeleteRecord(string Key)
        {
            TreeNode deleteNode = new TreeNode("",-1); 
            Search(ref Root,Key,ref deleteNode);
            //TBC
        }

        internal List<string> GetKeys()
        {
            throw new NotImplementedException();
        }
    }
}