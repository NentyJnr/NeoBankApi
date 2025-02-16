

namespace NeoBank.Dto
{
    public class TransferDto
    {
        public decimal? Amount { get; set; }

        public string? SenderAccountNumber { get; set; }

        public string? ReceiverAccountNumber { get; set; }

        public string? SenderAccountName { get; set; }  

        public string? ReceiverAccountName { get; set; }  

    }
}

