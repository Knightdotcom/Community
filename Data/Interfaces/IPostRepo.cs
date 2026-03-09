using community_api.Data.DTO;
using community_api.Data.Entities;

namespace community_api.Data.Interfaces
{
    // Kontrakt för inläggets repository
    // Implementeras av PostRepo och möjliggör Dependency Injection och testbarhet
    public interface IPostRepo
    {
        // Hämtar alla inlägg med valfri filtrering på titel och kategori
        Task<List<Post>> GetPostsAsync(PostSearchFilterDto filter);

        // Skapar ett nytt inlägg i databasen och returnerar den skapade entiteten
        Task<Post> AddPostAsync(Post post);

        // Tar bort ett inlägg (och dess kommentarer) från databasen
        Task<bool> DeletePostAsync(Post entity);

        // Hämtar ett inlägg via id (med kategori inkluderad)
        Task<Post?> GetPostByIdAsync(int id);

        // Hämtar ett detaljerat inlägg med kommentarer och författarinfo
        Task<Post?> GetDetailedPostAsync(int id);

        // Kontrollerar om en kategori med angivet id finns i databasen
        Task<bool> CategoryExistsAsync(int categoryId);

        // Sparar ändringar i databasen och returnerar antal påverkade rader
        Task<int> SaveChangesAsync();
    }
}
