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
            Key = BitConverter.ToString(data.Skip(2 * sizeof(int)).Take(keyLength).ToArray());
            Value = BitConverter.ToString(data.Skip(2 * sizeof(int) + keyLength).Take(valueLength).ToArray());
        }

        public Record(string key, string value)
        {
            Key = key;
            Value = value;
        }
        internal byte[] ToByteArray()
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(Key);
            byte[] valueBytes = Encoding.ASCII.GetBytes(Value);
            byte[] result = new byte[0];
            result.Concat(BitConverter.GetBytes(keyBytes.Length));
            result.Concat(BitConverter.GetBytes(valueBytes.Length));
            result.Concat(keyBytes);
            result.Concat(valueBytes);
            return result;
        }
    }
}
