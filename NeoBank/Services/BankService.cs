
using Microsoft.EntityFrameworkCore;
using NeoBank.Responses;
using NeoBank.Contracts;
using NeoBank.Data;
using NeoBank.Dto;
using NeoBank.Models;
using NeoBank.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace NeoBank.Services
{
    public class BankService : ResponseBaseService, IBankService
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContext;
        private readonly CodeGeneratorHelper _codeGenerator;

        public BankService(AppDbContext db, IHttpContextAccessor httpContext, CodeGeneratorHelper codeGenerator) : base()
        {
            _db = db;
            _httpContext = httpContext;
            _codeGenerator = codeGenerator;
        }



            public async Task<ServerResponse<bool>> CreateAccountAsync(AccountCreationDto request)
            {
                var response = new ServerResponse<bool>();

                // Check if email already exists
                var exAccount = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == request.Email);
                if (exAccount != null)
                {
                    response.IsSuccessful = false;
                    return SetError(response, ResponseCodes.EMAIL_ALREADY_EXIST);
                }

                // Generate account number
                var accountNo = await _codeGenerator.GenerateAccountNumberAsync();
                if (!accountNo.IsSuccessful)
                {
                    return SetError(response, ResponseCodes.FAIL);
                }
                var accountNumber = accountNo.Data;
                var account = new Account
                {
                    Email = request.Email,
                    AccountName = request.AccountName,
                    AccountNumber = accountNo.Data,
                    Id = Guid.NewGuid(),
                    DateCreated = DateTime.UtcNow,
                    IsActive = true,
                    AccountBalance = 0,
                };
                
                var save = await _db.Accounts.AddAsync(account);
                var saveResult = await _db.SaveChangesAsync();
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

            if (request.Amount == null || request.Amount <= 0)
            {
                return SetError(response, ResponseCodes.INVALID_DEPOSIT_AMOUNT);
            }

            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == request.AccountNumber);
            if (account == null)
            {
                return SetError(response, ResponseCodes.ACCOUNT_NOT_FOUND);
            }


            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    account.AccountBalance += request.Amount.Value;
                    _db.Accounts.Update(account);
                    var result = await _db.SaveChangesAsync();

                    if (result > 0)
                    {
                        await transaction.CommitAsync();
                        return SetSuccess(response, true, ResponseCodes.ACCOUNT_CREDITED_SUCCESSFULLY);
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL);
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return SetError(response, ResponseCodes.FAIL);
                }
            }
        }

        public async Task<ServerResponse<BalanceDto>> CheckAccountBalanceAsync(string accountNumber)
        {
            var response = new ServerResponse<BalanceDto>();

            var account = await _db.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                return SetError(response, ResponseCodes.ACCOUNT_NOT_FOUND);
            }

            var balDto = new BalanceDto
            {
                AccountBalance = account.AccountBalance
            };

            return SetSuccess(response, balDto, ResponseCodes.SUCCESS);
        }

        public async Task<ServerResponse<string>> GetAccountNameAsync(string accountNumber)
        {
            var response = new ServerResponse<string>();
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                return SetError(response, ResponseCodes.INVALID_REQUEST); //"Account number is required.");
            }
            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            if (account == null)
            {
                return SetError(response, ResponseCodes.ACCOUNT_NOT_FOUND); //"Account not found.");
            }
            return SetSuccess(response, account.AccountName, ResponseCodes.SUCCESS);
        }


        //public async Task<ServerResponse<bool>> TransferAsync(TransferDto request)
        //{
        //    var response = new ServerResponse<bool>();

        //    var senderAccount = await _db.Accounts
        //        .FirstOrDefaultAsync(a => a.AccountNumber == request.SenderAccountNumber);
        //    var receiverAccount = await _db.Accounts
        //        .FirstOrDefaultAsync(a => a.AccountNumber == request.ReceiverAccountNumber);

        //    if (senderAccount == null || receiverAccount == null)
        //    {
        //        return SetError(response, ResponseCodes.ACCOUNT_NOT_FOUND);
        //    }

        //    if (senderAccount.AccountBalance < request.Amount)
        //    {
        //        return SetError(response, ResponseCodes.INSUFFICIENT_BALANCE);
        //    }

        //    senderAccount.AccountBalance -= request.Amount.Value;
        //    receiverAccount.AccountBalance += request.Amount.Value;

        //    _db.Accounts.Update(senderAccount);
        //    _db.Accounts.Update(receiverAccount);
        //    var result = await _db.SaveChangesAsync();
        //    if (result > 0)
        //    {
        //        return SetSuccess(response, true, ResponseCodes.TRANSFER_SUCCESSFUL);
        //    }
        //    else
        //    {
        //        return SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL);
        //    }


        //}

        public async Task<ServerResponse<bool>> TransferAsync(TransferDto request)
        {
            var response = new ServerResponse<bool>();

            // Validate the transfer amount to ensure it's provided and positive
            if (!request.Amount.HasValue || request.Amount.Value <= 0)
            {
                return SetError(response, ResponseCodes.INVALID_AMOUNT); //"Transfer amount must be greater than zero.";
            }

            // Optionally, prevent transferring to the same account
            if (request.SenderAccountNumber == request.ReceiverAccountNumber)
            {
                return SetError(response, ResponseCodes.INVALID_TRANSFER); //"Sender and receiver accounts cannot be the same.");
            }
            // Retrieve both sender and receiver accounts from the database
            var senderAccount = await _db.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == request.SenderAccountNumber);
            var receiverAccount = await _db.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == request.ReceiverAccountNumber);
            
            // Check if either account is not found
            if (senderAccount == null || receiverAccount == null)
            {
                return SetError(response, ResponseCodes.ACCOUNT_NOT_FOUND);
            }
            // Ensure the sender has sufficient funds
            if (senderAccount.AccountBalance < request.Amount.Value)
            {
                return SetError(response, ResponseCodes.INSUFFICIENT_BALANCE);
            }

            request.SenderAccountName = senderAccount.AccountName;
            request.ReceiverAccountName = receiverAccount.AccountName;
            // Use a transaction to ensure atomicity of the transfer operation
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    // Deduct the amount from the sender and add it to the receiver
                    senderAccount.AccountBalance -= request.Amount.Value;
                    receiverAccount.AccountBalance += request.Amount.Value;

                    // Update both accounts
                    _db.Accounts.Update(senderAccount);
                    _db.Accounts.Update(receiverAccount);
                    var result = await _db.SaveChangesAsync();
                    if (result > 0)
                    {
                        await transaction.CommitAsync();
                        return SetSuccess(response, true, ResponseCodes.TRANSFER_SUCCESSFUL);
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return SetError(response, ResponseCodes.REQUEST_NOT_SUCCESSFUL);
                    }
                }
                catch (Exception ex)
                {
                    // Roll back the transaction in case of an exception
                    await transaction.RollbackAsync();
                    return SetError(response, ResponseCodes.FAIL);
                }
            }
        }


        public async Task<ServerResponse<bool>> WithdrawAsync(WithdrawalDto request)
        {
            var response = new ServerResponse<bool>();

            var account =  _db.Accounts.FirstOrDefault(a => a.AccountNumber == request.AccountNumber);

            if (account == null)
            {
                return SetError(response, ResponseCodes.ACCOUNT_NOT_FOUND);
            }

            //if (request.Amount <= 0)
            //{
            //    return SetError(response, ResponseCodes.WITHDRAWAL_AMOUNT_MUST_BE_GREATER_THAN_ZERO);
            //}
            if (!request.Amount.HasValue || request.Amount.Value <= 0)
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

            _db.Accounts.Update(account);
            var result = await _db.SaveChangesAsync();
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
            if (id == Guid.Empty)
            {
                return SetError(response, ResponseCodes.INVALID_ID);
            }

            var acc = await _db.Accounts.FindAsync(id);
            if (acc == null)
            {
                return SetError(response, ResponseCodes.ACCOUNT_NOT_FOUND);
            }

            try
            {
                 _db.Accounts.Remove(acc);
                await _db.SaveChangesAsync();
                return SetSuccess(response, true, ResponseCodes.SUCCESS);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                // Example: _logger.LogError(ex, "Error deleting account with ID {AccountId}", id);
                return SetError(response, ResponseCodes.FAIL);
            }
        }

        public async Task<ServerResponse<CreateAccountDto>> GetAccountByIdAsync(Guid id)
        {
            var response = new ServerResponse<CreateAccountDto>();

            if (id == Guid.Empty)
            {
                return SetError(response, ResponseCodes.INVALID_ID);
            }

            var account = await _db.Accounts.FindAsync(id);
            if (account == null)
            {
                return SetError(response, ResponseCodes.ACCOUNT_NOT_FOUND);
            }

            // Map the account entity to an AccountDto (using AutoMapper or manually)
            var accountDto = new CreateAccountDto
            {
                Id = account.Id,
                AccountName = account.AccountName,
                Email = account.Email,
                AccountNumber = account.AccountNumber,
                AccountBalance = account.AccountBalance
            };

            return SetSuccess(response, accountDto, ResponseCodes.SUCCESS);
        }
    }
}
