
using community_api.Core.Services;
using community_api.Data.DTO;
using community_api.Data.Entities;

namespace community_api.Core.Interfaces
{
    // Kontrakt för autentiseringstjänsten
    // Implementeras av AuthService och möjliggör Dependency Injection och testbarhet
    public interface IAuthService
    {
        // Genererar en JWT-token för en given användare
        Task<string> GenerateToken(AppUser user);

        // Loggar in en användare med användarnamn/e-post och lösenord
        // Returnerar en ServiceResult med JWT-token vid lyckad inloggning
        Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginUserDto dto);
    }
}
