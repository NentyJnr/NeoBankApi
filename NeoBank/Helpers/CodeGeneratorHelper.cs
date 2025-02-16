
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NeoBank.Data;
using NeoBank.Responses;

namespace NeoBank.Helpers
{
    public class CodeGeneratorHelper : ResponseBaseService
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContext;

        public CodeGeneratorHelper(AppDbContext db, IHttpContextAccessor httpContext) : base()
        {
            _db = db;
            _httpContext = httpContext;

        }

        //public async Task<ServerResponse<string>> GenerateAccountNumberAsync()
        //{
        //    var response = new ServerResponse<string>();
        //    string accountNumber;
        //    var random = new Random();

        //    try
        //    {
        //        do
        //        {
        //            accountNumber = $"{random.Next(100000, 999999)}{random.Next(100000, 999999)}";
        //        }
        //        while (await _db.Accounts.AnyAsync(a => a.AccountNumber == accountNumber)); 
        //        if (response.IsSuccessful)
        //        {
        //            SetSuccess(response, accountNumber, ResponseCodes.SUCCESS);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        if (!response.IsSuccessful)
        //        {
        //            SetError(response, ResponseCodes.FAIL);
        //        };

        //    }

        //    return response;
        //}

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
                while (await _db.Accounts.AnyAsync(a => a.AccountNumber == accountNumber));
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
