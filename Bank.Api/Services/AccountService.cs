using Azure.Core;
using Azure;
using Bank.Api.Contracts;
using Bank.Api.Data;
using Bank.Api.Dto;
using Bank.Api.Responses;
using Microsoft.EntityFrameworkCore;
using Bank.Api.Helpers;
using Bank.Api.Entities;

namespace Bank.Api.Services
{
    public class AccountService : ResponseBaseService, IAccountService
    {
        private readonly ApppDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly CodeGeneratorHelper _codeGenerator;

        public AccountService(ApppDbContext context, IHttpContextAccessor httpContext, CodeGeneratorHelper codeGenerator) : base()
        {
            _context = context;
            _httpContext = httpContext;
            _codeGenerator = codeGenerator;
        }


        public async Task<ServerResponse<bool>> CreateAccountAsync(AccountCreationDto request)
        {
            var response = new ServerResponse<bool>();

            var exUser = await _context.Accounts.FirstOrDefaultAsync(p => p.Email == request.Email);
            if (exUser != null)
            {
                return SetError(response, ResponseCodes.EMAIL_ALREADY_EXIST);
            }

            var acc = await _codeGenerator.GetAccountNumberAsync();
            if (!acc.IsSuccessful)
            {
                return SetError(response, ResponseCodes.FAIL); 
            }

            var accountNumber = acc.Data;

            var account = new Account
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                AccountBalance = 0,
                AccountName = $"{request.FirstName} {request.LastName}",
                AccountNumber = accountNumber,
                IsActive = true,
                DateCreated = DateTime.UtcNow,

            };

            var save = await _context.Accounts.AddAsync(account);
            var saveResult = await _context.SaveChangesAsync();
            if (saveResult > 0)
            {
                SetSuccess(response, true, ResponseCodes.ACCOUNT_CREATED_SUCCESSFULLY);
            }
            else
            {
                SetError(response, ResponseCodes.FAIL);
            }

            return response;

        }



        public async Task<ServerResponse<bool>> DepositAsync(DepositDto request)
        {
            var response = new ServerResponse<bool>();

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber);
            if (account == null)
            {
                return SetError(response, ResponseCodes.ACCOUNT_NOT_FOUND);
            }

            account.AccountBalance += request.Amount.Value;
            _context.Accounts.Update(account);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return SetSuccess(response, true, ResponseCodes.ACCOUNT_CREDITED_SUCCESSFULLY);
            }
            else
            {
                return SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL);
            }

        }




        public async Task<ServerResponse<BalanceDto>> CheckAccountBalanceAsync(string accountNumber)
        {
            var response = new ServerResponse<BalanceDto>();

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                return SetError(response, ResponseCodes.ACCOUNT_NOT_FOUND);
            }

            var balDto = new BalanceDto
            {
                AccountBalance = $"₦{account.AccountBalance:N2}"
            };

            return SetSuccess(response, balDto, ResponseCodes.SUCCESS);
        }




        public async Task<ServerResponse<bool>> TransferAsync(TransferDto request)
        {
            var response = new ServerResponse<bool>();

            var senderAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == request.SenderAccountNumber);
            var receiverAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == request.ReceiverAccountNumber);

            if (senderAccount == null || receiverAccount == null)
            {
                return SetError(response, ResponseCodes.ACCOUNT_NOT_FOUND);
            }

            if (senderAccount.AccountBalance < request.Amount)
            {
                return SetError(response, ResponseCodes.INSUFFICIENT_BALANCE);
            }

            senderAccount.AccountBalance -= request.Amount.Value;
            receiverAccount.AccountBalance += request.Amount.Value;

            _context.Accounts.Update(senderAccount);
            _context.Accounts.Update(receiverAccount);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return SetSuccess(response, true, ResponseCodes.TRANSFER_SUCCESSFUL);
            }
            else
            {
                return SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL);
            }


        }



        public async Task<ServerResponse<bool>> WithdrawAsync(WithdrawalDto request)
        {
            var response = new ServerResponse<bool>();

            var account =  _context.Accounts.FirstOrDefault(a => a.AccountNumber == request.AccountNumber);

            if (account == null)
            {
                return SetError(response, ResponseCodes.ACCOUNT_NOT_FOUND);
            }

            if (request.Amount <= 0)
            {
                return SetError(response, ResponseCodes.WITHDRAWAL_AMOUNT_MUST_BE_GREATER_THAN_ZERO);
            }

            var currentBalance = account.AccountBalance; 
            if(currentBalance - request.Amount < 100)
            {
                return SetError(response, ResponseCodes.INSUFFICIENT_BALANCE_MINIMUM_BALANCE_OF_100_IS_REQUIRED);
            }

            if(currentBalance < request.Amount)
            {
                return SetError(response, ResponseCodes.INSUFFICIENT_BALANCE);
            }
            account.AccountBalance -= request.Amount.Value;

            _context.Accounts.Update(account);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return SetSuccess(response, true, ResponseCodes.REQUEST_SUCCESSFUL);
            }
            else
            {
                return SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL);
            }

        }



        public async Task<ServerResponse<bool>> DeleteAccountAsync(Guid id)
        {
            var response = new ServerResponse<bool>();

            var acc = await _context.Accounts.FindAsync(id);
            if (acc == null)
            {
                return SetError(response, ResponseCodes.ACCOUNT_NOT_FOUND);
            }

            _context.Accounts.Remove(acc);
            await _context.SaveChangesAsync();

            return SetSuccess(response, true, ResponseCodes.SUCCESS);
        }

    }
}
