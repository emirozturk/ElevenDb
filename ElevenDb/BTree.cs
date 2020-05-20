using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;

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

        private void RootFirstTraverse(Node Current, ref string treeString)
        {
            treeString += Encoding.UTF8.GetString(BitConverter.GetBytes(Current.Key.Length));
            treeString += Current.Key;
            treeString += Encoding.UTF8.GetString(BitConverter.GetBytes(Current.BlockNumber));
            RootFirstTraverse(Current.Left, ref treeString);
            RootFirstTraverse(Current.Right, ref treeString);
        }

        private void CreateFromString(string treeString)
        {
            List<Node> nodeList = StringToNodeList(treeString);
            foreach (var n in nodeList)
                AddRecord(n.Key, n.BlockNumber);
        }

        private List<Node> StringToNodeList(string treeString)
        {
            List<Node> nodeList = new List<Node>();
            int index = 0;
            int length;
            while (index < treeString.Length)
            {
                length = BitConverter.ToInt32(treeString.Substring(index, 4).Cast<byte>().ToArray());
                index += 4;

                string key = treeString.Substring(index, length);
                index += length;

                int blockNumber = BitConverter.ToInt32(treeString.Substring(index, 4).Cast<byte>().ToArray());
                index += 4;

                nodeList.Add(new Node(key, blockNumber));
            }
            return nodeList;
        }

        internal Result<int> GetBlockNumber(string Key)
        {
            int result = -1;
            Search(ref Root, Key, ref result);
            if (result == -1) return new Result<int>(-1, ResultType.KeyNotFound);
            else return new Result<int>(result, ResultType.Success);
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

        internal void UpdateRecord(string key, int data)
        {
            throw new NotImplementedException();
        }

        private void Search(ref Node Current, string Key, ref int Result)
        {
            if (Current == null) return;
            if (Key == Current.Key)
                Result = Current.BlockNumber;
            else if (Key.CompareTo(Current.Key) > 0)
            {
                Node right = Current.Right;                
                Search(ref right, Key, ref Result);
            }
            else if (Key.CompareTo(Current.Key) < 0)
            {
                Node left = Current.Left;
                Search(ref left, Key, ref Result);
            }
        }

        private void Add(ref Node Current, string Key, int BlockNumber)
        {
            if (Current == null)
                Current = new Node(Key, BlockNumber);
            else if (Key.CompareTo(Current.Key) > 0)
            {
                if (Current.Right == null)
                    Current.Right = new Node(Key, BlockNumber);
                else
                {
                    Node right = Current.Right;
                    Add(ref right, Key, BlockNumber);
                }
            }
            else if (Key.CompareTo(Current.Key) < 0)
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