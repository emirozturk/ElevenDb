using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Transactions;

namespace ElevenDb
{
    class Node
    {

        internal Node Left { get; set; }
        internal Node Right { get; set; }
        internal string Key { get; set; }
        internal int BlockNumber { get; set; }
        public Node(string Key, int BlockNumber)
        {
            this.Key = Key;
            this.BlockNumber = BlockNumber;
        }
    }
    internal class BTree
    {
        Node Root;
        public BTree()
        {

        }

        public BTree(string TreeString)
        {
            CreateFromString(TreeString);
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
        private void RootFirstTraverse(Node Current, ref string treeString)
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
            List<Node> nodeList = StringToNodeList(treeString);
            foreach (var n in nodeList)
            {
                AddRecord(n.Key, n.BlockNumber);
            }
        }
        private List<Node> StringToNodeList(string treeString)
        {
            List<Node> nodeList = new List<Node>();
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

                nodeList.Add(new Node(key, blockNumber));
            }
            return nodeList;
        }

        internal Result<int> GetBlockNumber(string Key)
        {
            Node result = new Node(Key, -1);
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
            Node n = new Node(Key, -1);
            Search(ref Root, Key, ref n);
            n.BlockNumber = BlockNumber;
        }

        private void Search(ref Node Current, string Key, ref Node Result)
        {
            if (Current == null) return;
            if (Key == Current.Key)
                Result = Current;
            else if (String.Compare(Key,Current.Key) > 0)
            {
                Node right = Current.Right;
                Search(ref right, Key, ref Result);
            }
            else if (String.Compare(Key,Current.Key) < 0)
            {
                Node left = Current.Left;
                Search(ref left, Key, ref Result);
            }
        }

        private void Add(ref Node Current, string Key, int BlockNumber)
        {
            if (Current == null)
                Current = new Node(Key, BlockNumber);
            else if (String.Compare(Key, Current.Key) > 0)
            {
                if (Current.Right == null)
                    Current.Right = new Node(Key, BlockNumber);
                else
                {
                    Node right = Current.Right;
                    Add(ref right, Key, BlockNumber);
                }
            }
            else if (String.Compare(Key, Current.Key) < 0)
            {
                if (Current.Left == null)
                    Current.Left = new Node(Key, BlockNumber);
                else
                {
                    Node left = Current.Left;
                    Add(ref left, Key, BlockNumber);
                }
            }
        }
    }
}