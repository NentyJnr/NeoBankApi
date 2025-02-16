

using NeoBank.Dto;
using NeoBank.Models;
using NeoBank.Responses;

namespace NeoBank.Contracts
{
    public interface IBankService
    {
        Task<ServerResponse<bool>> CreateAccountAsync(AccountCreationDto request);
        Task<ServerResponse<bool>> DepositAsync(DepositDto request);
        Task<ServerResponse<BalanceDto>> CheckAccountBalanceAsync(string accountNumber);
        Task<ServerResponse<bool>> TransferAsync(TransferDto request);
        Task<ServerResponse<bool>> WithdrawAsync(WithdrawalDto request);
        Task<ServerResponse<bool>> DeleteAccountAsync(Guid id);
        Task<ServerResponse<string>> GetAccountNameAsync(string accountNumber);
        Task<ServerResponse<CreateAccountDto>> GetAccountByIdAsync(Guid id);

    }
}
