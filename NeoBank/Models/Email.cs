namespace NeoBank.Models
{
    public class Email
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class EmailServerResponse
    {
        public bool hasError { get; set; }
        public string message { get; set; }
        public string statusCode { get; set; }
        public string token { get; set; }
        public Result result { get; set; }
    }
    public class Result
    {
        public string message { get; set; }
        public bool isSuccessful { get; set; }
        public int retId { get; set; }
        public string errors { get; set; }
    }
    public class EmailBody
    {
        public long Id { get; set; }
        public string Receiver { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public bool HasAttachment { get; set; }
        public byte[] attachmentContent { get; set; }
    }
}
