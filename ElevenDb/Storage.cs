using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ElevenDb
{
    class Storage
    {
        static int RecordSizeInKb;
        internal static Result<string> DbExists(string dbPath)
        {
            if (File.Exists(dbPath))
                return new Result<string>(Messages.DbExists, ResultType.DbExists);
            else
                return new Result<string>(Messages.DbNotFound, ResultType.DbNotFound);
        }

        internal static Result LoadDb(string dbPath)
        {
            throw new NotImplementedException();
        }

        internal static Result CreateDb(string dbPath, Options options)
        {
            try
            {
                File.Create(dbPath);
                RecordSizeInKb = options.RecordSizeinKb;
                return new Result(null, ResultType.Success);
            }
            catch
            {
                return new Result(null, ResultType.DbCreateError);
            }
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
