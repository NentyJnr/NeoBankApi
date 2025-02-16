namespace NeoBank.Responses
{
    public class ServerResponse<T> : BasicResponse
    {
        public ServerResponse(bool success = false)
        {
            IsSuccessful = success;
        }
        public T Data { get; set; }
        public string SuccessMessage { get; set; }
    }
}

