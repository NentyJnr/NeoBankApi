using NeoBank.Dto;
using NeoBank.Responses;

namespace NeoBank.Contracts
{
    public interface IAccountService
    {
        Task<ServerResponse<RegistrationResponse>> RegisterAsync(RegistrationRequest request);
        Task<ServerResponse<bool>> LoginUserAsync(UserLoginDto request);
        Task <ServerResponse<bool>> LogoutUserAsync();
        Task<ServerResponse<RegistrationResponse>> GetUserByIdAsync(string userId);
        //Task<ServerResponse<IEnumerable<RegistrationResponse>>> GetAllUsersAsync();
    }
}
