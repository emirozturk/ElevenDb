namespace ElevenDb
{
    public enum ResultType
    {
        Success,
        NotFound,
        Overwritten,
        DbExists,
        RecordCreateFailure,
        DbNotFound,
        DbCreateFailure,
        TreeReadFailure,
        UnknownFailure,
        RecordReadFailure,
        StorageWriteFailure,
        TreeInsertionFailure,
        RecordUpdateFailure,
        BlockReadFailure,
        BlockWriteFailure,
        RecordWriteFailure,
        FlagWriteError,
        FlagReadError,
        TreeWriteFailure,
        FileCheckFailure,
    }
    internal class Messages
    {
        public static string FileClosedProperly { get; internal set; }
        public static string FileNotClosedProperly { get; internal set; }
        internal static string DbNotFound { get; } = "Cannot access db file";
        internal static string DbExists { get; } = "";
        internal static string TreeReadFailure { get; } = "";
        internal static string UnknownFailure { get; } = "";
        internal static string RecordReadFailure { get; } = "";
        internal static string RecordNotFound { get; } = "";
        internal static string StorageWriteError { get; } = "";
        internal static string TreeInsertionFailure { get; } = "";
        internal static string DbCreateFailure { get; } = "";
        internal static string Success { get; } = "";
        internal static string TreeKeyNotFound { get; } = "";
        internal static string TreeReadSuccess { get; } = "";
        internal static string TreeInsertionSuccess { get; } = "";
    }

}