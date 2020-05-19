using System;

namespace ElevenDb
{
    public class Record
    {
        public Record(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
