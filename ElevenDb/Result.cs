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
        RecordCreateError,
        DbNotFound,
        TreeReadSuccess,
        RecordFound,
        DbCreateError,
        TreeReadFailure,
        UnknownFailure,
        RecordReadSuccess,
        RecordReadFailure,
        RecordNotFound
    }
    public class Result<T>
    {
        public ResultType Message { get; set; }

        internal T Data;
        public Result(T Data,ResultType Message)
        {
            this.Data = Data;
            this.Message = Message;
        }
    }
}