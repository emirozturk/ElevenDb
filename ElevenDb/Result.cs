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
        RecordFound
    }
    public class Result
    {
        internal Record Record;
        public ResultType Message { get; set; }
        public string Value
        {
            get { return Record.Value; }
        }
        public Result(Record Record,ResultType Message)
        {
            this.Record = Record;
            this.Message = Message;
        }
    }
}