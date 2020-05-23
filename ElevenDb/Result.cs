namespace ElevenDb
{
    public class Result
    {
        public string Message { get; internal set; }
        public dynamic Value { get; internal set; }
        public bool IsSuccess { get; internal set; }
        internal void SetMessage(string Message)
        {
            this.Message = Message;
        }
        internal void SetDataWithSuccess(dynamic Data)
        {
            this.Value = Data;
            IsSuccess = true;
            Message = "Success";
        }
        internal void SetSuccess(bool Value)
        {
            IsSuccess = Value;
        }
        public Result(dynamic Data, bool IsSuccess, string Message = "")
        {
            this.Value = Data;
            this.IsSuccess = IsSuccess;
            this.Message = Message;
        }

        public Result()
        {
            IsSuccess = false;
        }
    }
}