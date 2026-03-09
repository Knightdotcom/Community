
using community_api.Core.Interfaces;
using community_api.Core.Services;
using community_api.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace community_api.Controllers
{
    // Hanterar användarrelaterade operationer (registrering, uppdatering, borttagning)
    // Bas-URL: api/user
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // Privat fält för användartjänsten - injiceras via Dependency Injection
        private readonly IUserService _userService;

        // Konstruktor - tar emot UserService via Dependency Injection
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // POST api/user
        // Registrerar en ny användare i systemet
        // Returnerar 201 Created med den skapade användaren
        [HttpPost]
        [SwaggerOperation(
            Summary = "Registrera ny användare",
            Description = "Skapa ett nytt konto. Användarnamn, e-post och lösenord är obligatoriska. Förnamn och efternamn är valfria."
        )]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            // Anropar registreringstjänsten med inkommande DTO
            var result = await _userService.RegisterUserAsync(dto);

            // Om registreringen misslyckades, returnera 400 med felmeddelanden
            if (!result.Success || result.Data is null)
                return BadRequest(result.ErrorMessages);

            // Returnera 201 Created med den skapade användaren och ett bekräftelsemeddelande
            return StatusCode(201, new { User = result.Data, Message = "Anvandaren skapades" });
        }

        // PUT api/user
        // Uppdaterar den inloggade användarens profil
        // Kräver JWT-token i Authorization-headern
        [Authorize]
        [HttpPut]
        [SwaggerOperation(
            Summary = "Uppdatera min användarprofil",
            Description = "Uppdatera dina egna uppgifter (förnamn, efternamn, e-post, användarnamn, lösenord). Alla fält är valfria. Kräver inloggning."
        )]
        [ProducesResponseType(typeof(ServiceResult<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(UpdateUserDto dto)
        {
            // Hämtar den inloggade användarens id från JWT-token claims
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Om id saknas är token ogiltig - returnera 401 Unauthorized
            if (id is null)
                return Unauthorized("Logga in for att uppdatera");

            // Anropar uppdateringstjänsten med användarens id och ny data
            var result = await _userService.UpdateUserAsync(id, dto);

            // Om uppdateringen misslyckades, returnera 400 med felmeddelanden
            if (!result.Success)
                return BadRequest(result.ErrorMessages);

            // Returnera 200 OK med den uppdaterade användardatan
            return Ok(result);
        }

        // DELETE api/user?id={id}
        // Tar bort en användare med angivet id
        [HttpDelete]
        [SwaggerOperation(
            Summary = "Ta bort användare",
            Description = "Tar bort en användare med angivet id. Skicka med användarens id-sträng som query-parameter."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            // Anropar borttagningstjänsten med det angivna id:t
            var result = await _userService.DeleteUserAsync(id);

            // Om användaren inte hittades, returnera 404 Not Found
            if (!result.Success)
                return NotFound(result.ErrorMessages);

            // Returnera 200 OK med bekräftelsemeddelande
            return Ok(result.Data);
        }
    }
}
