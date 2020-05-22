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
        internal BTree index;
        internal Storage storage;
        public DB(string Path)
        {
            this.dbPath = Path;
            options = Options.GetDefault();
            Logger.LogPath = dbPath;
        }
        public Result<string> Open()
        {
            Result<string> result = Storage.DbExists(dbPath);
            if (result.Message == ResultType.DbExists)
            {
                Result<string> properlyCloseResult = Storage.IsFileClosedProperly(dbPath);
                if (properlyCloseResult.Message == ResultType.Success)
                {
                    storage = new Storage(dbPath);
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
                    storage = new Storage(dbPath);
                    index = new BTree(GetNodeList());
                    return new Result<string>(Messages.TreeReadSuccess, ResultType.Success);
                }
            }
            else
            {
                result = Storage.CreateDb(dbPath, options);
                storage = new Storage(dbPath);
                index = new BTree();
                storage.WriteRecord(new Record("ElevenTree000", index.ToString()));
                return result;
            }
        }
        public string ReadValue(string key)
        {
            return Read(key).Data;
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
            else if (recordStartResult.Message == ResultType.NotFound)
                return new Result<string>(Messages.RecordNotFound, ResultType.NotFound);
            return new Result<string>(Messages.UnknownFailure, ResultType.UnknownFailure);
        }
        public Result<List<KeyValuePair<string, string>>> ReadAll()
        {
            List<KeyValuePair<string, string>> kvpList = new List<KeyValuePair<string, string>>();
            var iterator = new Iterator(this);
            while (iterator.HasRecord)
                kvpList.Add(new KeyValuePair<string, string>(iterator.CurrentKey, iterator.GetNext().Data));
            return new Result<List<KeyValuePair<string, string>>>(kvpList, ResultType.Success);
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
            else if (blockNumberResult.Message == ResultType.NotFound)
            {
                Result<int> newBlockNumberResult = storage.WriteRecord(new Record(Key, Value));
                if (newBlockNumberResult.Message == ResultType.Success)
                    return index.AddRecord(Key, newBlockNumberResult.Data);
                else if (newBlockNumberResult.Message == ResultType.RecordWriteFailure)
                    return new Result<string>(Messages.StorageWriteError, ResultType.RecordWriteFailure);
            }
            return new Result<string>(Messages.UnknownFailure, ResultType.UnknownFailure);
        }
        public Result<string> WriteBatch(List<KeyValuePair<string, string>> kvpList)
        {
            foreach (var kvp in kvpList)
                Write(kvp.Key, kvp.Value);
            return new Result<string>(Messages.Success, ResultType.Success);
        }
        public Result<string> Delete(string key)
        {
            Result<int> blockNumberResult = index.GetBlockNumber(key);
            if (blockNumberResult.Message == ResultType.Success)
            {
                var result = storage.DeleteRecord(blockNumberResult.Data);
                if (result.Message == ResultType.Success)
                    return index.DeleteRecord(key);
                return result;
            }
            return new Result<string>(Messages.TreeKeyNotFound, ResultType.NotFound);
        }
        public Iterator GetIterator()
        {
            return new Iterator(this);
        }
        public Result<string> Close()
        {
            var result = WriteTree();
            if (result.Message == ResultType.Success)
                return storage.Close();
            else
                return result;
        }
        private Result<string> WriteTree()
        {
            Result<string> result = storage.WriteTree(index.ToString());
            return result;
        }
        private List<TreeNode> GetNodeList()
        {
            return storage.ReadNodes();
        }
        public Result<string> ReadTree()
        {
            return storage.ReadTree();
        }
    }
}
