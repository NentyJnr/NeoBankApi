using NeoBank.Models;
using System.IdentityModel.Tokens.Jwt;

namespace NeoBank.Contracts
{
    public interface ITokenService
    {
        Task<JwtSecurityToken> GenerateToken(ApplicationUser applicationUser);
    }
}
