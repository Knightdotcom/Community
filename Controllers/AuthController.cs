
using community_api.Core.Interfaces;
using community_api.Core.Services;
using community_api.Data.DTO;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace community_api.Controllers
{
    // Hanterar autentisering (inloggning)
    // Bas-URL: api/auth
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Privat fält för autentiseringstjänsten - injiceras via DI
        private readonly IAuthService _authService;

        // Konstruktor - tar emot AuthService via Dependency Injection
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST api/auth/login
        // Loggar in användaren med användarnamn/e-post och lösenord
        // Returnerar en JWT-token vid lyckad inloggning
        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Logga in",
            Description = "Logga in med användarnamn eller e-post och lösenord. Returnerar JWT-token."
        )]
        [ProducesResponseType(typeof(ServiceResult<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginUserDto dto)
        {
            // Anropar tjänsten och returnerar 200 OK eller 401 Unauthorized
            var result = await _authService.LoginAsync(dto);
            return result.Success ? Ok(result) : Unauthorized(result.ErrorMessages);
        }
    }
}
