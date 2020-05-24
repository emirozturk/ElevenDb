using System;

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
                Logger.LogLine(MethodName, this);

        }
        internal void SetDataWithSuccess(string MethodName, dynamic Value)
        {
            this.Value = Value;
            IsSuccess = true;
            Message = "Success";
            if (Options.IsLoggingActive)
                Logger.LogLine(MethodName, this);
        }
        public override string ToString()
        {
            return IsSuccess.ToString().PadRight(5) + " - " + Message;
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