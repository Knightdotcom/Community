
using community_api.Core.Interfaces;
using community_api.Data.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace community_api.Controllers
{
    // Hanterar alla inläggsrelaterade operationer (CRUD)
    // Bas-URL: api/post
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        // Privat fält för inlägstjänsten - injiceras via DI
        private readonly IPostService _service;

        // Konstruktor - tar emot PostService via Dependency Injection
        public PostController(IPostService service)
        {
            _service = service;
        }

        // GET api/post
        // Hämtar alla inlägg, med valfri filtrering på titel eller kategori
        // Ingen inloggning krävs
        [HttpGet]
        [SwaggerOperation(Summary="Hämta alla inlägg",Description="Returnerar alla inlägg.")]
        [ProducesResponseType(typeof(List<PostsGetDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPosts([FromQuery] PostSearchFilterDto filter)
        {
            // Anropar tjänsten med filterparametrar och returnerar resultatet
            var result = await _service.GetPostsAsync(filter);
            return Ok(result);
        }

        // GET api/post/{id}
        // Hämtar ett specifikt inlägg med alla kommentarer och författarinformation
        [HttpGet("{id}")]
        [SwaggerOperation(Summary="Hämta ett inlägg med kommentarer",Description="Returnerar ett detaljerat inlägg inklusive alla kommentarer.")]
        [ProducesResponseType(typeof(PostsGetDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPost([FromRoute] int id)
        {
            // Anropar tjänsten och returnerar 200 OK eller 404 Not Found
            var result = await _service.GetDetailedPostAsync(id);
            return result.Success ? Ok(result) : NotFound(result.ErrorMessages);
        }

        // POST api/post - Skapar ett nytt inlägg, kräver inloggning
        [Authorize]
        [HttpPost]
        [SwaggerOperation(Summary="Skapa nytt inlägg",Description="Skapar ett nytt inlägg. Kräver inloggning (JWT-token).")]
        [ProducesResponseType(typeof(PostAddResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddPost([FromBody] PostAddDto dto)
        {
            // Hämtar den inloggade användarens id från JWT-token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var result = await _service.AddPostAsync(dto, userId);
            if (!result.Success) return BadRequest(result.ErrorMessages);
            return StatusCode(StatusCodes.Status201Created, result.Data);
        }

        // PUT api/post?postId={postId} - Uppdaterar ett befintligt inlägg
        [Authorize]
        [HttpPut]
        [SwaggerOperation(Summary="Uppdatera ett inlägg",Description="Uppdaterar ett befintligt inlägg. Kräver inloggning.")]
        [ProducesResponseType(typeof(PostUpdateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePost([FromBody] PostUpdateDto dto, int postId)
        {
            // Hämtar den inloggade användarens id från JWT-token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var result = await _service.UpdatePostAsync(dto, postId, userId);
            if (!result.Success) return BadRequest(result.ErrorMessages);
            return Ok(result);
        }

        // DELETE api/post?postId={postId} - Tar bort ett inlägg permanent
        [Authorize]
        [HttpDelete]
        [SwaggerOperation(Summary="Ta bort ett inlägg",Description="Tar bort ett inlägg permanent. Kräver inloggning.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeletePost(int postId)
        {
            // Hämtar den inloggade användarens id från JWT-token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
            var result = await _service.DeletePostAsync(postId, userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
