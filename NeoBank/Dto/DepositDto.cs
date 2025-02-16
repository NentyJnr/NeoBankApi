

using System.ComponentModel.DataAnnotations;

namespace NeoBank.Dto
{
    public class DepositDto
    {
        [Required(ErrorMessage = "Account Number is required.")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Deposit amount is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Deposit amount must be greater than zero.")]
        public decimal? Amount { get; set; }

    }
}

