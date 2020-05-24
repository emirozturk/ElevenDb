namespace ElevenDb
{
    public class Options
    {
        public static byte BlockSizeinKb { get; internal set; }
        public static bool IsLoggingActive { get; internal set; }
        public Options(byte blockSizeinKb, bool isLoggingActive = false)
        {
            BlockSizeinKb = blockSizeinKb;
            IsLoggingActive = isLoggingActive;
        }

        internal static Options GetDefault()
        {
            return new Options(4);
        }
    }
}
