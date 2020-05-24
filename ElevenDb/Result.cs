using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElevenDb
{
    public class Result
    {
        public string Message { get; internal set; }
        public dynamic Value { get; internal set; }
        public bool IsSuccess { get; internal set; }
        internal void SetMessage(string MethodName, string Message)
        {
            this.Message = Message;
            if (Options.IsLoggingActive)
            {
                Logger.LogLines(MethodName, this);
            }
        }
        internal void SetDataWithSuccess(string MethodName, dynamic Value)
        {
            this.Value = Value;
            IsSuccess = true;
            Message = "Success";
            if (Options.IsLoggingActive)
            {
                Logger.LogLines(MethodName, this);
            }
        }
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append($"{IsSuccess,5}|");
            output.Append(Message == null ? "" : Message + "|");
            if (Value != null)
            {
                string valueString;
                if (Value.GetType().Equals(typeof(List<int>)))
                {
                    valueString = string.Join(", ", Value);
                }
                else
                {
                    valueString = Value.ToString();
                }
                int length = Math.Min(50, valueString.Length);
                output.Append(new string(valueString.Take(length).ToArray()));
                if (length < valueString.Length)
                {
                    output.Append("...");
                }
            }
            return output.ToString();
        }
        public Result()
        {
            IsSuccess = false;
        }
    }
    public class Result<T>
    {
        public string Message { get; private set; }
        public T Value { get; internal set; }
        public bool IsSuccess { get; internal set; }
        internal Result(Result result)
        {
            IsSuccess = result.IsSuccess;
            Message = result.Message;
            Value = result.Value;
        }
    }
}