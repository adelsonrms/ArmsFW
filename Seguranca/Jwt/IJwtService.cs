using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

/// <summary>
/// Contem objetos para tratativa de security
/// </summary>
namespace ArmsFW.Security
{
    /// <summary>
    /// Interface para os serviços de Tokens Jwt
    /// </summary>
    public interface IJwtService
    {
        Task<JwtToken> GerarToken(ClaimsPrincipal user);
        Task<JwtToken> GerarToken(IEnumerable<Claim> userClaims);
        JwtToken Decodificar(string token);
    }
}


