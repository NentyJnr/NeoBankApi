namespace NeoBank.Models
{
    public class Account : BaseObject
    {
        public Guid Id { get; set; }

        public string? AccountName { get; set; }

        public string? Email { get; set; }

        public string? AccountNumber { get; set; }

        public decimal AccountBalance { get; set; }

    }
}
