using community_api.Data.Entities;
using community_api.Data.Interfaces;

namespace community_api.Data.Repos
{
    // Implementerar dataåtkomst för kommentarer via Entity Framework Core
    // Ansvarar for alla databasoperationer relaterade till Comment-entiteten
    public class CommentRepo : ICommentRepo
    {
        // Privat fält för databaskontexten - injiceras via Dependency Injection
        private readonly AppDbContext _context;

        // Konstruktor - tar emot AppDbContext via Dependency Injection
        public CommentRepo(AppDbContext context)
        {
            _context = context;
        }

        // Skapar en ny kommentar i databasen
        // Returnerar den skapade entiteten med det genererade CommentId
        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            var result = await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
    }
}
