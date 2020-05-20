using System;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ElevenDb
{
    public class Record
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public Record(string Key)
        {
            this.Key = Key;
            Value = "";
        }

        public Record(byte[] data)
        {
            int keyLength = BitConverter.ToInt32(data.Take(sizeof(int)).ToArray());
            int valueLength = BitConverter.ToInt32(data.Skip(sizeof(int)).Take(sizeof(int)).ToArray());
            Key = Encoding.UTF8.GetString(data.Skip(2 * sizeof(int)).Take(keyLength).ToArray());
            Value = Encoding.UTF8.GetString(data.Skip(2 * sizeof(int) + keyLength).Take(valueLength).ToArray());
        }

        public Record(string key, string value)
        {
            Key = key;
            Value = value;
        }
        internal byte[] ToByteArray()
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(Key);
            byte[] valueBytes = Encoding.UTF8.GetBytes(Value);
            byte[] result = BitConverter.GetBytes(keyBytes.Length)
            .Concat(BitConverter.GetBytes(valueBytes.Length))
            .Concat(keyBytes)
            .Concat(valueBytes).ToArray();
            return result;
        }
    }
}
