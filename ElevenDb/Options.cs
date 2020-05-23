namespace ElevenDb
{
    public class Options
    {
        public byte BlockSizeinKb { get; }

        public Options(byte BlockSizeinKb)
        {
            this.BlockSizeinKb = BlockSizeinKb;
        }

        internal static Options GetDefault()
        {
            return new Options(4);
        }
    }
}
