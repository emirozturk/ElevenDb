namespace ElevenDb
{
    public class Options
    {
        internal static byte BlockSizeinKb { get; set; } = 4;
        internal static bool IsLoggingActive { get; set; } = false;
        internal static int MaxLogSizeInKb { get; set; } = 1024;

        public Options(byte BlockSizeinKb = 4, bool IsLoggingActive = false, int MaxLogSizeInKb = 1024)
        {
            Options.BlockSizeinKb = BlockSizeinKb;
            Options.IsLoggingActive = IsLoggingActive;
            Options.MaxLogSizeInKb = MaxLogSizeInKb;
        }
    }
}
