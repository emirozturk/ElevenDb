using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ElevenDb
{
    class Storage
    {
        internal static Result DbExists(string dbPath)
        {
            if (File.Exists(dbPath))
                return new Result(null, ResultType.DbExists);
            else
                return new Result(null, ResultType.DbNotFound);
        }

        internal static Result LoadDb(string dbPath)
        {
            throw new NotImplementedException();
        }

        internal static Result CreateDb(string dbPath, Options options)
        {
            throw new NotImplementedException();
        }

        internal static Result Write(Record value)
        {
            throw new NotImplementedException();
        }

        internal static Result ReadRecord(int RecordNumber)
        {
            throw new NotImplementedException();
        }

        internal static Result ReadTree()
        {
            throw new NotImplementedException();
        }
    }
}
