using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;

namespace ElevenDb
{
    public class DB
    {
        private string dbPath;
        Options options;
        BTree index;
        public DB(string Path)
        {
            this.dbPath = Path;
            options = Options.GetDefault();
        }
        public DB(string Path,Options Options)
        {
            this.dbPath = Path;
            options = Options;
        }
        public Result<string> Open()
        {
            Result<string> result = Storage.DbExists(dbPath);
            if (result.Message == ResultType.DbExists)
            {
                Result<Record> treeResult = Storage.ReadTree();
                if (treeResult.Message == ResultType.TreeReadSuccess)
                {
                    index = new BTree(treeResult.Data.Value);
                    return new Result<string>(Messages.TreeReadSuccess, ResultType.TreeReadSuccess);
                }
                else if (treeResult.Message == ResultType.TreeReadFailure)
                    return new Result<string>(Messages.TreeReadFailure, ResultType.TreeReadFailure);
                else
                    return new Result<string>(Messages.UnknownFailure, ResultType.UnknownFailure);
            }
            else
            {
                result = Storage.CreateDb(dbPath, options);
                index = new BTree();
                return result;
            }
        }

        public Result<String> Read(string Key)
        {
            Result<int> recordStartResult = BTree.Find(Key);
            if (recordStartResult.Message == ResultType.RecordFound)
            {
                Result<Record> result = Storage.ReadRecord(recordStartResult.Data);
                if(result.Message == ResultType.RecordReadSuccess)
                {
                    return new Result<string>(result.Data.Value, ResultType.RecordReadSuccess);
                }
                else if(result.Message == ResultType.RecordReadFailure)
                {
                    return new Result<string>(Messages.RecordReadFailure, ResultType.RecordReadFailure);
                }
                else
                    return new Result<string>(Messages.UnknownFailure, ResultType.UnknownFailure);
            }
            else if(recordStartResult.Message == ResultType.RecordNotFound)
            {
                return new Result<string>(Messages.RecordNotFound, ResultType.RecordNotFound);
            }
            else
                return new Result<string> (Messages.UnknownFailure, ResultType.UnknownFailure);
        }
        public Result Write(string Key, string Value)
        {
            Result result = Record.Create(Key, Value);
            if (result.Message == ResultType.Success)
            {
                if (index.KeyExists())
                {

                }
                else
                {
                    index.InsertKey();
                }
                return Storage.Write(result.Record);
            else
                    return result;

            }
        }



        public Iterator GetIterator()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Result> ReadAll()
        {
            throw new NotImplementedException();
        }
    }
}
