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
        RecordNotFound,
        KeyNotFound,
        StorageWriteFailure,
        TreeInsertionFailure,
        RecordUpdateFailure,
    }
    public class Result<T>
    {
        public ResultType Message { get; set; }

        public T Data { get; set; }
        public Result(T Data, ResultType Message)
        {
            this.Data = Data;
            this.Message = Message;
        }
    }
}