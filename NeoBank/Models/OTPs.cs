namespace NeoBank.Models
{
    public class OTPs : BaseObject
    {
        public long Id { get; set; }
        public string? OTP { get; set; }
        public string? Email { get; set; }
        public string? OTPType { get; set; }
        public string? Token { get; set; }
    }
}
