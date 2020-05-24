using System;
using System.Linq;
using System.Text;

namespace ElevenDb
{
    public class Result
    {
        public string Message { get; private set; }
        public dynamic Value { get; internal set; }
        public bool IsSuccess { get; internal set; }
        internal void SetMessage(string MethodName, string Message)
        {
            this.Message = Message;
            if (Options.IsLoggingActive)
                Logger.LogLines(MethodName, this);

        }
        internal void SetDataWithSuccess(string MethodName, dynamic Value)
        {
            this.Value = Value;
            IsSuccess = true;
            Message = "Success";
            if (Options.IsLoggingActive)
                Logger.LogLines(MethodName, this);
        }
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append(IsSuccess.ToString().PadRight(5)+"|");
            output.Append(Message == null ? "" : Message+"|");
            if (Value != null)
            {
                string valueString = Value.ToString();
                int length = Math.Min(50, valueString.Length);
                output.Append(new string(valueString.Take(length).ToArray()));
                if (length < valueString.Length) 
                    output.Append("...");
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
        public Result(Result result)
        {
            this.IsSuccess = result.IsSuccess;
            this.Message = result.Message;
            this.Value = result.Value;
        }
    }
}