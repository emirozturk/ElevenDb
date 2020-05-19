using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ElevenDb
{
    class Storage
    {
        string DbPath;
        static int RecordSizeInKb;
        const int KB = 1024;
        FileStream fs;
        internal Storage(string DbPath)
        {
            this.DbPath = DbPath;
            fs = new FileStream(DbPath, FileMode.Open);
        }
        internal static Result<string> DbExists(string dbPath)
        {
            if (File.Exists(dbPath))
                return new Result<string>(Messages.DbExists, ResultType.DbExists);
            else
                return new Result<string>(Messages.DbNotFound, ResultType.DbNotFound);
        }

        internal static Result<string> CreateDb(string dbPath, Options options)
        {
            try
            {
                File.Create(dbPath);
                RecordSizeInKb = options.RecordSizeinKb;
                return new Result<string>(Messages.Success, ResultType.Success);
            }
            catch
            {
                return new Result<string>(Messages.Success, ResultType.DbCreateFailure);
            }
        }

        internal Result<Record> ReadRecord(int BlockNumber)
        {
            if(BlockNumber == 0)
            {
                byte[] treeBlock = new byte[1024 * KB];
                fs.Read(treeBlock, 0, 1024 * KB);
                Block b = new Block(treeBlock);
            }
            else
            {

            }
        }
        internal void WriteRecord(int BlockNumber)
        {
            if(BlockNumber == 0)
            {

            }
        }

        internal Result<int> WriteRecord(Record record)
        {
            throw new NotImplementedException();
        }

        internal Result<string> UpdateRecord(Record record)
        {
            throw new NotImplementedException();
        }

        internal void Close()
        {
            fs.Close();
        }
    }
}
