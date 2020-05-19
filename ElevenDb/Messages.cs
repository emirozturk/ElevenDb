namespace ElevenDb
{
    internal class Messages
    {
        internal static readonly string DbNotFound = "Cannot access db file";

        public static string DbExists { get; internal set; }
        public static string TreeReadFailure { get; internal set; }
        public static string UnknownFailure { get; internal set; }
        public static string TreeReadSuccess { get; internal set; }
        public static string RecordReadFailure { get; internal set; }
        public static string RecordNotFound { get; internal set; }
        public static string StorageWriteError { get; internal set; }
        public static string Success { get; internal set; }
        public static string TreeInsertionFailure { get; internal set; }
        public static string TreeInsertionSuccess { get; internal set; }
    }
}