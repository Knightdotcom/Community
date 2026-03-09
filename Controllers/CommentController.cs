
using community_api.Core.Interfaces;
using community_api.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace community_api.Controllers
{
    // Hanterar kommentarsrelaterade operationer
    // Hela controllern kräver inloggning ([Authorize] på klassnivå)
    // Bas-URL: api/comment
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        // Privat fält för kommentarstjänsten - injiceras via Dependency Injection
        private readonly ICommentService _service;

        // Konstruktor - tar emot CommentService via Dependency Injection
        public CommentController(ICommentService service)
        {
            _service = service;
        }

        // POST api/comment?postId={postId}
        // Lägger till en ny kommentar på ett inlägg
        // Kräver inloggning - man kan inte kommentera sitt eget inlägg
        [HttpPost]
        [SwaggerOperation(
            Summary = "Lägg till kommentar på ett inlägg",
            Description = "Lägger till en kommentar på ett specifikt inlägg. Kräver inloggning. Du kan inte kommentera ditt eget inlägg. Ange inläggets id som query-parameter och kommentartexten i bodyn."
        )]
        [ProducesResponseType(typeof(CommentAddResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddComment(int postId, [FromBody] CommentAddDto dto)
        {
            // Hämtar den inloggade användarens id från JWT-token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Om id saknas är token ogiltig - returnera 401
            if (userId is null)
                return Unauthorized();

            // Skapar kommentaren - tjänsten kontrollerar att användaren inte äger inlägget
            var result = await _service.AddCommentAsync(postId, dto, userId);

            // Om skapandet misslyckades, returnera 400 med felmeddelanden
            if (!result.Success)
                return BadRequest(result);

            // Returnera 200 OK med den skapade kommentaren
            return Ok(result);
        }
    }
}
