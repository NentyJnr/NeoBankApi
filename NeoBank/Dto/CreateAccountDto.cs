

namespace NeoBank.Dto
{
    public class AccountCreationDto
    {
        public string? AccountName { get; set; }

        public string? Email { get; set; }

        public decimal AccountBalance { get; set; }
 
    }

    public class CreateAccountDto
    {
        public Guid Id { get; set; } 
        
        public string? AccountName { get; set; }

        public string? Email { get; set; }

        public decimal AccountBalance { get; set; }

        public string AccountNumber { get; set; }

    }
}
