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
        public int Length { get; set; }

        internal static Result Create(string key, string value)
        {
            try
            {
                return new Result(new Record(key, value), ResultType.Success);
            }
            catch
            {
                return new Result(null, ResultType.RecordCreateError);
            }
        }
    }
}
