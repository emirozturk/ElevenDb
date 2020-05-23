using System.Collections.Generic;

namespace ElevenDb
{
    public class Iterator
    {
        private readonly DB db;
        private readonly List<string> keys;
        private int counter;
        internal Iterator(DB Db)
        {
            db = Db;
            keys = Db.index.GetKeys().Value;
            counter = 0;
        }
        public Result GetNext()
        {
            return db.Read(keys[counter++]);
        }
        public bool HasRecord => counter < keys.Count;
        internal string CurrentKey => keys[counter];
    }
}
