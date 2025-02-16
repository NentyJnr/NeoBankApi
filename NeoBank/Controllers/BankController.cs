using Microsoft.AspNetCore.Mvc;
using NeoBank.Contracts;
using NeoBank.Data;
using NeoBank.Dto;
using NeoBank.Models;
using NeoBank.Responses;

namespace NeoBank.Controllers
{
    public class BankController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IBankService _accountService;

        public BankController(AppDbContext db, IBankService accountService)
        {
            _db = db;
            _accountService = accountService;
        }

        public IActionResult Index()
        {
            List<Account> objAccountList = _db.Accounts.ToList();
            return View(objAccountList);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(AccountCreationDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    ResponseCode = ResponseCodes.INVALID_REQUEST,
                    ResponseDescription = "Invalid request data"
                });
            }

            var response = await _accountService.CreateAccountAsync(request);

            if (response == null)
            {
                return BadRequest(new ErrorResponse
                {
                    ResponseCode = ResponseCodes.EXCEPTION,
                    ResponseDescription = "An unexpected error occurred while creating the account."
                });
            }

            if (response.IsSuccessful)
            {
                TempData["success"] = "Account created successfully";
                return RedirectToAction("Index");
            }

            return BadRequest(new ErrorResponse
            {
                ResponseCode = ResponseCodes.FAIL,
                ResponseDescription = "Failed to create account"
            });
        }

        public IActionResult Deposit()
        {
            return View(new DepositDto());
        }

        // POST: Deposit/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(DepositDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    ResponseCode = ResponseCodes.INVALID_REQUEST,
                    ResponseDescription = "Invalid request data"
                });

                //TempData["Error"] = "Invalid input. Please check your details.";
                //return View(request);
            }
            var response = await _accountService.DepositAsync(request);

            if (response == null)
            {
                return BadRequest(new ErrorResponse
                {
                    ResponseCode = ResponseCodes.EXCEPTION,
                    ResponseDescription = "An unexpected error occurred."
                });
            }

            if (response.IsSuccessful)
            {
                TempData["success"] = "Deposit successful!";
                return RedirectToAction("Index");
            }

            return BadRequest(new ErrorResponse
            {
                ResponseCode = ResponseCodes.FAIL,
                ResponseDescription = "Failed to fund account"
            });
        }

        [HttpGet]
        public IActionResult Balance()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Balance(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                TempData["Error"] = "Account number is required.";
                return View();
            }


            var response = await _accountService.CheckAccountBalanceAsync(accountNumber);

            if (response.IsSuccessful)
            {
                return View("Display", response.Data);
            }
            else
            {
                TempData["Error"] = "Failed to retrieve balance";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Transfer()
        {
            return View(new TransferDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(TransferDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide valid transfer details.";
                return View(model);
            }

            var response = await _accountService.TransferAsync(model);

            if (response.IsSuccessful)
            {
                TempData["Success"] = "Transfer successful!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Transfer not successful";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Withdraw()
        {
            return View(new WithdrawalDto());
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(WithdrawalDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid input data.";
                return View(model);
            }

            var response = await _accountService.WithdrawAsync(model);
            if (response.IsSuccessful)
            {
                TempData["Success"] = "Withdrawal successful.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Request Unsuccessful";
                return View(model);
            }
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAccountName(string accountNumber)
        {
            var serviceResponse = await _accountService.GetAccountNameAsync(accountNumber);
            return Json(new
            {
                success = serviceResponse.IsSuccessful,
                accountName = serviceResponse.Data,
                message = serviceResponse.IsSuccessful
            });
        }


        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["Error"] = "Invalid account ID.";
                return RedirectToAction("Index");
            }

            var response = await _accountService.GetAccountByIdAsync(id);
            if (!response.IsSuccessful || response.Data == null)
            {
                TempData["Error"] = "Account not found.";
                return RedirectToAction("Index");
            }

            // Return the confirmation view with the account details.
            return View(response.Data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["Error"] = "Invalid account ID.";
                return RedirectToAction("Index");
            }

            var response = await _accountService.DeleteAccountAsync(id);

            if (response.IsSuccessful)
            {
                TempData["Success"] = "Account deleted successfully!";
            }
            else
            {
                TempData["Error"] = "An error occurred while deleting the account.";
            }

            return RedirectToAction("Index");
        }



    }

}




