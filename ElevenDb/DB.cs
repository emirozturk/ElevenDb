using System.Collections.Generic;
using System.Reflection;

namespace ElevenDb
{
    public class DB
    {
        private readonly string dbPath;
        internal BTree index;
        internal Storage storage;
        /// <summary>
        /// Creates an instance of DB class
        /// </summary>
        /// <param name="Path">Path of database file</param>
        public DB(string Path)
        {
            dbPath = Path;
            Logger.LogPath = dbPath;
        }
        /// <summary>
        /// Creates an instance of DB class
        /// </summary>
        /// <param name="Path">Path of database file</param>
        /// <param name="Options">Database creation options. If db exists, this parameter will be ignored</param>
        public DB(string Path, Options Options)
        {
            dbPath = Path;
            Logger.LogPath = dbPath;
            Logger.MaxLogSizeInKb = Options.MaxLogSizeInKb;
        }
        private Result WriteTree()
        {
            return storage.WriteTree(index.ToString());
        }
        private Result GetNodeList()
        {
            return storage.ReadAllRecords();
        }
        private Result ReadTree()
        {
            return storage.ReadTree();
        }
        /// <summary>
        /// Opens or creates db file
        /// </summary>
        /// <returns></returns>
        public Result Open()
        {
            Result result;
            if (Storage.DbExists(dbPath))
            {
                result = Storage.IsFileClosedProperly(dbPath);
                if (result.IsSuccess)
                {
                    storage = new Storage(dbPath);

                    if (result.Value)
                    {
                        result = ReadTree();
                        index = new BTree(result.Value);
                    }
                    else
                    {
                        result = GetNodeList();
                        if (result.IsSuccess)
                        {
                            var nodeList = result.Value; 
                            result = storage.CreateBlockMap();
                            index = new BTree(nodeList,result.Value);
                        }
                    }

                    if (result.IsSuccess)
                    {
                        result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, null);
                    }
                }
            }
            else
            {
                result = Storage.CreateDb(dbPath);
                if (result.IsSuccess)
                {
                    storage = new Storage(dbPath);
                    index = new BTree();
                    result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, null);
                }
            }
            return result;
        }
        /// <summary>
        /// Reads a value of a corresponding key bypassing the result
        /// </summary>
        /// <param name="key">Key of value to be retrieved</param>
        /// <returns>Value of given key as string</returns>
        public string ReadValue(string key)
        {
            return Read(key).Value;
        }
        /// <summary>
        /// Reads a value of corresponding key as a result object
        /// </summary>
        /// <param name="Key">Key of value to be retrieved</param>
        /// <returns>Result object containing a string "Value" property</returns>
        public Result<string> Read(string Key)
        {
            Result result = index.GetBlockNumber(Key);
            if (result.IsSuccess)
            {
                result = storage.ReadRecord(result.Value);
                if (result.IsSuccess)
                {
                    result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, result.Value.Value);
                }
            }
            return new Result<string>(result);
        }
        /// <summary>
        /// Reads all keys and values from db
        /// </summary>
        /// <returns>A List of string key value pairs representing the keys and values</returns>
        public Result<List<KeyValuePair<string, string>>> ReadAll()
        {
            Result result = new Result();
            List<KeyValuePair<string, string>> kvpList = new List<KeyValuePair<string, string>>();
            Iterator iterator = new Iterator(this);
            while (iterator.HasRecord)
            {
                kvpList.Add(new KeyValuePair<string, string>(iterator.CurrentKey, iterator.GetNext().Value));
            }
            result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, kvpList);
            return new Result<List<KeyValuePair<string, string>>>(result);
        }
        /// <summary>
        /// Writes a record to database. If key exists value will be updated.
        /// </summary>
        /// <param name="Key">String key of record</param>
        /// <param name="Value">String value of record</param>
        /// <returns>A result object containing a message and state of operation</returns>
        public Result Write(string Key, string Value)
        {
            Result result = index.GetBlockNumber(Key);
            if (result.IsSuccess)
            {
                Record record = new Record(Key, Value);
                List<int> emptyBlocks; 
                if (result.Value != -1)
                {
                    result = storage.DeleteRecord(result.Value);
                    if (result.IsSuccess)
                    {
                        index.UnSetBlockMap(result.Value);
                        emptyBlocks = index.GetEmptyBlocks(record.CalculateBlockCount(Options.BlockSizeinKb));
                        result = storage.WriteRecord(record, emptyBlocks);
                        if (result.IsSuccess)
                        {
                            index.SetBlockMap(result.Value);
                            result = index.UpdateRecord(Key, result.Value[0]);
                        }
                    }
                }
                else
                {
                    emptyBlocks = index.GetEmptyBlocks(record.CalculateBlockCount(Options.BlockSizeinKb));
                    result = storage.WriteRecord(record, emptyBlocks);
                    if (result.IsSuccess)
                    {
                        index.SetBlockMap(result.Value);
                        result = index.AddNode(Key, result.Value[0]);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Takes a key value pair list and writes it to db as a batch
        /// </summary>
        /// <param name="kvpList">Key value pair to be written to db</param>
        /// <returns>A result object containing a message and state of operation</returns>
        public Result WriteBatch(List<KeyValuePair<string, string>> kvpList)
        {
            Result result = new Result();
            if (kvpList == null)
            {
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, null);
            }
            else
            {
                foreach (KeyValuePair<string, string> kvp in kvpList)
                {
                    result = Write(kvp.Key, kvp.Value);
                    if (!result.IsSuccess)
                    {
                        return result;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Deletes a record corresponding to given key
        /// </summary>
        /// <param name="key">Key of record to be deleted</param>
        /// <returns>A result object containing a message and state of operation</returns>
        public Result Delete(string key)
        {
            Result result = index.GetBlockNumber(key);
            if (result.IsSuccess)
            {
                result = storage.DeleteRecord(result.Value);
                if (result.IsSuccess && result.Value[0] !=-1)
                {
                    index.UnSetBlockMap(result.Value);
                    result = index.DeleteRecord(key);
                }
            }
            return result;
        }
        /// <summary>
        /// Returns an Iterator object to iterate through keys
        /// </summary>
        /// <returns>An Iterator object</returns>
        public Iterator GetIterator()
        {
            return new Iterator(this);
        }
        /// <summary>
        /// Closes db instance and saves the remaining changes
        /// </summary>
        /// <returns>A result object containing a message and state of operation</returns>
        public Result Close()
        {
            Result result = WriteTree();
            if (result.IsSuccess)
            {
                result = storage.Close();
                if (Options.IsLoggingActive)
                {
                    Logger.LogRemaining();
                }
            }

            return result;
        }
        /// <summary>
        /// Repair method will create the index and block maps from db file if db is not closed properly
        /// </summary>
        /// <returns>A result object containing a message and state of operation</returns>
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
                    if (result.IsSuccess)
                    {
                        List<int> blockMap = storage.CreateBlockMap().Value;
                        index.CreateMapFromBlockList(blockMap);
                    }
                }
                result = storage.Close();
            }
            else
            {
                result.SetDataWithSuccess(MethodBase.GetCurrentMethod().Name, null);
            }

            return result;
        }
    }
}
