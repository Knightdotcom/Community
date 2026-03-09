
using community_api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace community_api.Controllers
{
    // Hanterar kategorioperationer (läsning av kategorier)
    // Bas-URL: api/category
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        // Privat fält för databaskontexten - injiceras direkt via DI
        // Kategorier behöver ingen extra servicenivå eftersom logiken är trivial
        private readonly AppDbContext _context;

        // Konstruktor - tar emot AppDbContext via Dependency Injection
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/category
        // Hämtar alla tillgängliga kategorier med id och namn
        // Ingen inloggning krävs
        [HttpGet]
        [SwaggerOperation(
            Summary = "Hämta alla kategorier",
            Description = "Returnerar en lista med alla tillgängliga kategorier och deras id-nummer. Använd kategori-id när du skapar eller uppdaterar ett inlägg."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            // Hämtar alla kategorier och väljer endast ut id och namn (anonym typ)
            var categories = await _context.Categories
                .Select(c => new { c.CategoryId, c.CategoryName })
                .ToListAsync();

            // Returnerar 200 OK med listan av kategorier
            return Ok(categories);
        }
    }
}
