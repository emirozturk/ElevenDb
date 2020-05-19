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
        Storage storage;
        public DB(string Path)
        {
            this.dbPath = Path;
            options = Options.GetDefault();
        }
        public DB(string Path, Options Options)
        {
            this.dbPath = Path;
            options = Options;
        }
        public Result<string> Open()
        {
            Result<string> result = Storage.DbExists(dbPath);
            if (result.Message == ResultType.DbExists)
            {
                storage = new Storage(dbPath);
                Result<Record> treeResult = storage.ReadRecord(0);
                if (treeResult.Message == ResultType.Success)
                {
                    index = new BTree(treeResult.Data.Value);
                    return new Result<string>(Messages.TreeReadSuccess, ResultType.Success);
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
                storage.WriteRecord(0);
                return result;
            }
        }

        public Result<String> Read(string Key)
        {
            Result<int> recordStartResult = index.GetBlockNumber(Key);
            if (recordStartResult.Message == ResultType.Success)
            {
                Result<Record> result = storage.ReadRecord(recordStartResult.Data);
                if (result.Message == ResultType.Success)
                    return new Result<string>(result.Data.Value, ResultType.Success);
                else if (result.Message == ResultType.RecordReadFailure)
                    return new Result<string>(Messages.RecordReadFailure, ResultType.RecordReadFailure);
            }
            else if (recordStartResult.Message == ResultType.RecordNotFound)
                return new Result<string>(Messages.RecordNotFound, ResultType.RecordNotFound);
            return new Result<string>(Messages.UnknownFailure, ResultType.UnknownFailure);
        }
        public Result<string> Write(string Key, string Value)
        {
            Result<int> blockNumberResult = index.GetBlockNumber(Key);
            if (blockNumberResult.Message == ResultType.KeyFound)
                return storage.UpdateRecord(new Record(Key, Value));
            else if (blockNumberResult.Message == ResultType.KeyNotFound)
            {
                Result<int> newBlockNumberResult = storage.WriteRecord(new Record(Key, Value));
                if (newBlockNumberResult.Message == ResultType.Success)
                {
                    Result<String> result = index.AddRecord(Key, newBlockNumberResult.Data);
                    storage.UpdateRecord(new Record("Tree",index.ToString()));
                }
                else if (newBlockNumberResult.Message == ResultType.StorageWriteFailure)
                    return new Result<string>(Messages.StorageWriteError, ResultType.StorageWriteFailure);
            }
            return new Result<string>(Messages.UnknownFailure, ResultType.UnknownFailure);
        }

        public void Close()
        {
            storage.Close();
        }

        public Iterator GetIterator()
        {
            throw new NotImplementedException();
        }
        /*
        public IEnumerable<Result> ReadAll()
        {
            throw new NotImplementedException();
        }
        */
    }
}
