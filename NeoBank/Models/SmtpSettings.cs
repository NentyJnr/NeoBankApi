namespace NeoBank.Models
{
    public class SmtpSettings
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
    }
}
