
using community_api.Core.Interfaces;
using community_api.Data.DTO;
using community_api.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace community_api.Core.Services
{
    // Implementerar autentiseringslogik: inloggning och JWT-generering
    // Använder ASP.NET Core Identity via UserManager för användarhantering
    public class AuthService : IAuthService
    {
        // Privata fält för konfiguration och Identity UserManager - injiceras via DI
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;

        // Konstruktor - tar emot konfiguration och UserManager via Dependency Injection
        public AuthService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        // Loggar in en användare med användarnamn eller e-post och lösenord
        // Returnerar en JWT-token vid lyckad autentisering
        public async Task<ServiceResult<LoginResponseDto>> LoginAsync(LoginUserDto dto)
        {
            // Avgörs om inmatningen är e-post (innehåller @) eller användarnamn
            var userEntity = dto.UserNameOrEmail.Contains("@")
                ? await _userManager.FindByEmailAsync(dto.UserNameOrEmail)
                : await _userManager.FindByNameAsync(dto.UserNameOrEmail);

            // Om ingen användare hittades med angivet användarnamn/e-post
            if (userEntity is null)
                return ServiceResult<LoginResponseDto>.Fail("Anvandaren hittades inte");

            // Kontrollerar att lösenordet stämmer mot det hashade lösenordet i databasen
            var correctPassword = await _userManager.CheckPasswordAsync(userEntity, dto.Password);

            // Om lösenordet är felaktigt - returnera felmeddelande
            if (!correctPassword)
                return ServiceResult<LoginResponseDto>.Fail("Fel losenord");

            // Genererar JWT-token och returnerar den vid lyckad inloggning
            var token = await GenerateToken(userEntity);

            return ServiceResult<LoginResponseDto>.Ok(new LoginResponseDto { Token = token });
        }

        // Genererar en JWT-token med användarens claims (id, namn, e-post)
        // Token är giltig i 1 timme och signeras med hemlig nyckel från konfigurationen
        public Task<string> GenerateToken(AppUser user)
        {
            // Definierar claims som kodas in i token (identifierar användaren)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            // Hämtar JWT-inställningar från appsettings.json
            var secretKey = _config["JwtSettings:Key"];
            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];

            // Skapar signeringsnyckeln och -algoritmen (HMAC SHA-256)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Bygger JWT-token med alla inställningar - giltig i 1 timme
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            // Serialiserar token till en sträng och returnerar den
            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
