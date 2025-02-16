using Bank.Api.Data;
using Bank.Api.Entities;
using Bank.Api.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Bank.Api.Helpers
{
    public class CodeGeneratorHelper : ResponseBaseService
    {
        private readonly ApppDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public CodeGeneratorHelper(ApppDbContext context, IHttpContextAccessor httpContext) : base()
        {
            _context = context;
            _httpContext = httpContext;

        }

        public async Task<ServerResponse<string>> GenerateAccountNumberAsync()
        {
            var response = new ServerResponse<string>();
            string accountNumber;
            var random = new Random();
            try
            {
                do
                {
                    accountNumber = $"{random.Next(100000, 999999)}{random.Next(100000, 999999)}";
                }
                while (await _context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber));
                SetSuccess(response, accountNumber, ResponseCodes.SUCCESS);
            }
            catch (Exception ex)
            {
                SetError(response, ResponseCodes.FAIL);
            }

            return response;
        }

        public async Task<ServerResponse<string>> GetAccountNumberAsync()
        {
            var response = new ServerResponse<string>();

            var acc = await GenerateAccountNumberAsync();
            if (!acc.IsSuccessful)
            {
                SetError(response, ResponseCodes.FAIL);
            }

            string accountNumber = acc.Data;
            if (acc.IsSuccessful)
            {
                SetSuccess(response, accountNumber, ResponseCodes.SUCCESS);

            }

            return response;

        }

    }
}
