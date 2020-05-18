using System;
using System.Collections.Generic;
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

        public Result Write(string Key, string Value)
        {
            Result result = Record.Create(Key, Value);
            if (result.Message == ResultType.Success)
                return Storage.Write(result.Record);
            else
                return result;
        }

        public Result Open()
        {
            Result result = Storage.DbExists(dbPath);
            if (result.Message == ResultType.DbExists)
            {
                result = Storage.ReadTree();
                if (result.Message == ResultType.TreeReadSuccess)
                    index = new BTree(result.Value);
            }
            else
                result = Storage.CreateDb(dbPath, options);
            return result;
        }

        public Result Read(string Key)
        {
            Result result = BTree.Find(Key);
            if(result.Message == ResultType.RecordFound)
                return Storage.ReadRecord(Key);
            return result;
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
