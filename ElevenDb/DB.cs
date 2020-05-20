using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;

namespace ElevenDb
{
    public class DB
    {
        readonly private string dbPath;
        readonly Options options;
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
                storage = new Storage(dbPath, options);
                Result<string> treeResult = ReadTree();
                if (treeResult.Message == ResultType.Success)
                {
                    index = new BTree(treeResult.Data);
                    return new Result<string>(Messages.TreeReadSuccess, ResultType.Success);
                }
                else if (treeResult.Message == ResultType.TreeReadFailure)
                    return new Result<string>(Messages.TreeReadFailure, ResultType.TreeReadFailure);
                else
                    return new Result<string>(Messages.UnknownFailure, ResultType.UnknownFailure);
            }
            else
            {
                result = Storage.CreateDb(dbPath);
                storage = new Storage(dbPath, options);
                index = new BTree();
                storage.WriteRecord(new Record("ElevenTree000", index.ToString()));
                return result;
            }
        }
        public Result<string> ReadTree()
        {
            Result<Record> result = storage.ReadRecord(0);
            if (result.Message == ResultType.Success)
                return new Result<string>(result.Data.Value, ResultType.Success);
            else if (result.Message == ResultType.RecordReadFailure)
                return new Result<string>(Messages.RecordReadFailure, ResultType.RecordReadFailure);
            else
                return new Result<string>(Messages.UnknownFailure, ResultType.UnknownFailure);
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
            if (blockNumberResult.Message == ResultType.Success)
            {
                Result<int> updateResult = storage.UpdateRecord(new Record(Key, Value), blockNumberResult.Data);
                if (updateResult.Message == ResultType.Success)
                {
                    index.UpdateRecord(Key, updateResult.Data);
                    return new Result<string>(Messages.Success, ResultType.Overwritten);
                }
            }
            else if (blockNumberResult.Message == ResultType.KeyNotFound)
            {
                Result<int> newBlockNumberResult = storage.WriteRecord(new Record(Key, Value));
                if (newBlockNumberResult.Message == ResultType.Success)
                {
                    Result<string> result = index.AddRecord(Key, newBlockNumberResult.Data);
                    if (result.Message == ResultType.Success)
                    {
                        Result<int> updateResult = storage.UpdateRecord(new Record("ElevenTree000", index.ToString()), 0);
                        if (updateResult.Message == ResultType.Success)
                            return new Result<string>(Messages.Success, ResultType.Success);
                    }
                    return
                        new Result<string>(Messages.TreeInsertionFailure, ResultType.TreeInsertionFailure);
                }
                else if (newBlockNumberResult.Message == ResultType.RecordWriteFailure)
                    return new Result<string>(Messages.StorageWriteError, ResultType.RecordWriteFailure);
            }
            return new Result<string>(Messages.UnknownFailure, ResultType.UnknownFailure);
        }
        public Result<string> Delete(string key)
        {
            Result<int> blockNumberResult = index.GetBlockNumber(key);
            if (blockNumberResult.Message == ResultType.Success)
                return storage.DeleteRecord(blockNumberResult.Data);
            else
                return new Result<string>(Messages.TreeKeyNotFound, ResultType.KeyNotFound);
        }
        public void Close()
        {
            storage.Close();
        }
        /*
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
