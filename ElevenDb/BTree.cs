using System;

namespace ElevenDb
{
    internal class BTree
    {
        private string value;

        public BTree()
        {
        }

        public BTree(string value)
        {
            this.value = value;
        }

        internal static Result<int> Find(string key)
        {
            throw new NotImplementedException();
        }
    }
}