using Microsoft.AspNetCore.Mvc;
using NeoBank.Contracts;
using NeoBank.Dto;
using NeoBank.Responses;

namespace NeoBank.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _userService;

        public AccountController(IAccountService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequest request)
        {
            if (ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    ResponseCode = ResponseCodes.INVALID_REQUEST,
                    ResponseDescription = "Invalid request data"
                });
            }

            var response = await _userService.RegisterAsync(request);
            if (response.IsSuccessful)
            {
                TempData["Success"] = "Registration successful. Please log in.";
                return RedirectToAction("Login", "User");
            }
            ModelState.AddModelError(string.Empty, "Registration failed.");

            return View(request);
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto request)
        {

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var response = await _userService.LoginUserAsync(request);
            if (response.IsSuccessful)
            {
                return RedirectToAction("Index", "Account");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutUserAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
