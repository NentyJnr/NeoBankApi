using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NeoBank.Contracts;
using NeoBank.Data;
using NeoBank.Dto;
using NeoBank.Migrations;
using NeoBank.Models;
using NeoBank.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NeoBank.Services
{
    public class AccountService : ResponseBaseService, IAccountService
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly IEmailRequests _emailRequests;
        //private readonly IEmailSender _emailSender;
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration;

     

        public AccountService(AppDbContext db, IHttpContextAccessor httpContext, IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<JwtSettings> jwtSettings) : base()
        {
            _db = db;
            _httpContext = httpContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwtSettings = jwtSettings.Value;
            //_emailRequests = emailRequests;
        }

        

        public async Task<ServerResponse<RegistrationResponse>> RegisterAsync(RegistrationRequest request)
        {
            var response = new ServerResponse<RegistrationResponse>();

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser == null)
            {
                var existingUsername = await _userManager.FindByNameAsync(request.UserName);
                if (existingUsername != null)
                {
                    return SetError(response, ResponseCodes.USERNAME_EXISTS);
                }

                ApplicationUser user = request.Adapt<ApplicationUser>();
                user.DateCreated = DateTime.Now;
                user.IsActive = true;
                user.IsDeleted = false;

                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    var baseUrl = _configuration["SystemSettings:ApiBaseUrl"];
                    var accountActivationDto = new AccountActivationDTO
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        ActivationLink = $"{baseUrl}/Account/AccountActivate?userId={user.Id}"
                    };

                    //var emailBody = await _emailRequests.AccountActivation(accountActivationDto);
                    //var emailResponse = await _emailSender.SendEmail(email);
                    if (response != null)
                    {
                        response.Data = new RegistrationResponse { UserId = user.Id };

                        return SetSuccess(response, response.Data, ResponseCodes.SUCCESS);
                    }
                    else
                    {
                        return SetError(response, ResponseCodes.EMAIL_NOT_SENT);
                    }
                }
                else
                {
                    return SetError(response, ResponseCodes.REGITRATION_FAILED);
                }
            }
            else
            {
                return SetError(response, ResponseCodes.USER_ALREADY_EXISTS);
            }
        }

        public async Task<ServerResponse<bool>> LoginUserAsync(UserLoginDto request)
        {
            var response = new ServerResponse<bool>();
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, request.RememberMe, lockoutOnFailure: false);
            SetSuccess(response, true, ResponseCodes.ACCOUNT_CREATED_SUCCESSFULLY);
            return response;
        }

        public async Task<ServerResponse<bool>> LogoutUserAsync()
        {
            var response = new ServerResponse<bool>();
            await _signInManager.SignOutAsync();
            SetSuccess(response, true, ResponseCodes.ACCOUNT_CREATED_SUCCESSFULLY);
            return response;
        }

        public async Task<ServerResponse<RegistrationResponse>> GetUserByIdAsync(string userId)
        {
            var response = new ServerResponse<RegistrationResponse>();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return SetError(response, ResponseCodes.USER_NOT_FOUND);
            }

            var userDto = new RegistrationResponse
            {
                               
            };

            return SetSuccess(response, userDto, ResponseCodes.SUCCESS);
            
        }

        //public async Task<ServerResponse<IEnumerable<UserDto>>> GetAllUsersAsync()
        //{
        //    var response = new ServerResponse<IEnumerable<UserDto>>();

        //    var user =  _db.ApplicationUsers.ToList();
        //    var records = user.Select(u => new UserDto
        //    {
        //        Id = u.Id,
        //        FirstName = u.FirstName,
        //        LastName = u.LastName,
        //        Email = u.Email,
        //        UserName = u.UserName,
        //        DateOfBirth = u.DateOfBirth,
        //    });

        //    return SetSuccess(response, records, ResponseCodes.SUCCESS);

        //}

        
    }
}
