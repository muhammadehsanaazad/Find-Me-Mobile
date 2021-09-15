namespace Find_Me_Mobile.Models
{
    public class ApplicationResult
    {
        public ApplicationResult()
        {
            IsSuccess = false;
            Message = string.Empty;
            Data = null;
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
