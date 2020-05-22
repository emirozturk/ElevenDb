using System;
using System.Collections.Generic;
using System.Text;

namespace ElevenDb
{
    public class Iterator
    {
        DB Db;
        List<string> Keys;
        int counter;
        internal Iterator(DB Db)
        {
            this.Db = Db;
            Keys = Db.index.GetKeys();
            counter = 0;
        }
        public Result<string> GetNext()
        {
            return Db.Read(Keys[counter++]);
        }
        public bool HasRecord
        {
            get
            {
                return counter < Keys.Count;
            }
        }
        internal string CurrentKey
        {
            get
            {
                return Keys[counter];
            }
        }
    }
}
