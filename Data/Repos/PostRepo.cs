using community_api.Data.DTO;
using community_api.Data.Entities;
using community_api.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace community_api.Data.Repos
{
    // Implementerar dataåtkomst för inlägg via Entity Framework Core
    // Ansvarar for alla databasoperationer relaterade till Post-entiteten
    public class PostRepo : IPostRepo
    {
        // Privat fält för databaskontexten - injiceras via Dependency Injection
        private readonly AppDbContext _context;

        // Konstruktor - tar emot AppDbContext via Dependency Injection
        public PostRepo(AppDbContext context)
        {
            _context = context;
        }

        // Skapar ett nytt inlägg i databasen
        // Returnerar den skapade entiteten med det genererade PostId
        public async Task<Post> AddPostAsync(Post post)
        {
            var result = await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        // Kontrollerar om en kategori med angivet id finns i databasen
        // Används för validering före uppdatering av inlägg
        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
        }

        // Tar bort ett inlägg från databasen
        // Tar först bort alla kommentarer på inlägget (pga NoAction cascade)
        // Returnerar true om minst en rad påverkades i databasen
        public async Task<bool> DeletePostAsync(Post entity)
        {
            // Hämtar alla kommentarer tillhörande detta inlägg
            var comments = _context.Comments.Where(c => c.PostId == entity.PostId);

            // Tar bort kommentarerna först (måste göras manuellt pga NoAction)
            _context.Comments.RemoveRange(comments);

            // Tar sedan bort själva inlägget
            _context.Posts.Remove(entity);

            // Sparar ändringarna och returnerar om något raderades
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        // Hämtar alla inlägg med valfri filtrering på titel och kategorinamn
        // Inkluderar kategori och användare via Include (eager loading)
        public async Task<List<Post>> GetPostsAsync(PostSearchFilterDto filter)
        {
            // Bygger upp en grundfråga med eager loading av kategori och användare
            var query = _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .AsQueryable()
                .AsNoTracking();

            // Filtrera på titel om filterparametern är angiven
            if (!string.IsNullOrWhiteSpace(filter.Title))
                query = query.Where(p => p.Title.Contains(filter.Title));

            // Filtrera på kategorinamn om filterparametern är angiven
            if (!string.IsNullOrWhiteSpace(filter.Category))
                query = query.Where(p => p.Category.CategoryName.Contains(filter.Category));

            // Kör frågan och returnerar resultatlistan
            return await query.ToListAsync();
        }

        // Hämtar ett enskilt inlägg via id med kategori inkluderad
        // Används internt för validering och uppdatering
        public async Task<Post?> GetPostByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.PostId == id);
        }

        // Hämtar ett detaljerat inlägg med kategori, användare och alla kommentarer
        // Kommentarerna inkluderar också sin författarinformation via ThenInclude
        public async Task<Post?> GetDetailedPostAsync(int id)
        {
            return await _context.Posts
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .SingleOrDefaultAsync(p => p.PostId == id);
        }

        // Sparar ändringar i databaskontexten och returnerar antal påverkade rader
        // Används efter uppdatering av entiteter
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
