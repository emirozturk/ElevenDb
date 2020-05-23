namespace ElevenDb
{
    internal class TreeNode
    {
        internal TreeNode Left { get; set; }
        internal TreeNode Right { get; set; }
        internal string Key { get; set; }
        internal int BlockNumber { get; set; }
        public TreeNode(string Key, int BlockNumber)
        {
            this.Key = Key;
            this.BlockNumber = BlockNumber;
        }

        public TreeNode()
        {
            Key = "";
            BlockNumber = -1;
        }
    }
}