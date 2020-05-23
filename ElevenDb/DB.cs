﻿using System;
using System.Collections.Generic;

namespace ElevenDb
{
    public class DB
    {
        private readonly string dbPath;
        private readonly Options options;
        internal BTree index;
        internal Storage storage;
        public DB(string Path)
        {
            dbPath = Path;
            options = Options.GetDefault();
            Logger.LogPath = dbPath;
        }
        private Result WriteTree()
        {
            return storage.WriteTree(index.ToString());
        }
        private Result GetNodeList()
        {
            return storage.ReadAllRecords();
        }
        public Result Open()
        {
            Result result;
            if (Storage.DbExists(dbPath))
            {
                result = Storage.IsFileClosedProperly(dbPath);
                if (result.IsSuccess)
                {
                    storage = new Storage(dbPath);

                    if (result.Value) result = ReadTree();
                    else result = GetNodeList();

                    if (result.IsSuccess)
                    {
                        index = new BTree(result.Value);
                        result.SetDataWithSuccess(null);
                    }
                }
            }
            else
            {
                result = Storage.CreateDb(dbPath, options);
                if (result.IsSuccess)
                {
                    storage = new Storage(dbPath);
                    index = new BTree();
                    result.SetDataWithSuccess(null);
                }
            }
            return result;
        }
        public string ReadValue(string key)
        {
            return Read(key).Value.Value;
        }
        public Result Read(string Key)
        {
            Result result = index.GetBlockNumber(Key);
            if (result.IsSuccess)
            {
                result = storage.ReadRecord(result.Value);
                if (result.IsSuccess)
                {
                    result.SetDataWithSuccess(result.Value.Value);
                }
            }
            return result;
        }
        public Result ReadAll()
        {
            Result result = new Result();
            List<KeyValuePair<string, string>> kvpList = new List<KeyValuePair<string, string>>();
            Iterator iterator = new Iterator(this);
            while (iterator.HasRecord)
            {
                kvpList.Add(new KeyValuePair<string, string>(iterator.CurrentKey, iterator.GetNext().Value));
            }

            result.SetDataWithSuccess(kvpList);
            return result;
        }
        public Result Write(string Key, string Value)
        {
            Result result = index.GetBlockNumber(Key);
            if (result.IsSuccess)
            {
                if (result.Value != -1)
                {
                    result = storage.UpdateRecord(new Record(Key, Value), result.Value);
                    if (result.IsSuccess)
                    {
                        result = index.UpdateRecord(Key, result.Value);
                    }
                }
                else
                {
                    result = storage.WriteRecord(new Record(Key, Value));
                    if (result.IsSuccess)
                    {
                        result = index.AddRecord(Key, result.Value);
                    }
                }
            }
            return result;
        }
        public Result WriteBatch(List<KeyValuePair<string, string>> kvpList)
        {
            Result result = new Result();
            foreach (KeyValuePair<string, string> kvp in kvpList)
            {
                result = Write(kvp.Key, kvp.Value);
                if (!result.IsSuccess)
                {
                    return result;
                }
            }
            return result;
        }
        public Result Delete(string key)
        {
            Result result = index.GetBlockNumber(key);
            if (result.IsSuccess)
            {
                result = storage.DeleteRecord(result.Value);
                if (result.IsSuccess)
                {
                    result = index.DeleteRecord(key);
                }
            }
            return result;
        }
        public Iterator GetIterator()
        {
            return new Iterator(this);
        }
        public Result Close()
        {
            Result result = WriteTree();
            if (result.IsSuccess)
            {
                result = storage.Close();
            }

            return result;
        }
        public Result ReadTree()
        {
            return storage.ReadTree();
        }

        public Result RepairDb()
        {
            Result result = new Result();
            if (Storage.DbExists(dbPath))
            {
                storage = new Storage(dbPath);
                result = GetNodeList();
                if (result.IsSuccess)
                {
                    index = new BTree(result.Value);
                }
                result = storage.Close();
            }
            else
                result.SetDataWithSuccess(null);
            return result;
        }
    }
}
