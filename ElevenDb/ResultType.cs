using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
