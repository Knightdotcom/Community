using community_api.Data.Entities;

namespace community_api.Data.Interfaces
{
    // Kontrakt för kommentarens repository
    // Implementeras av CommentRepo och möjliggör Dependency Injection och testbarhet
    public interface ICommentRepo
    {
        // Skapar en ny kommentar i databasen och returnerar den skapade entiteten
        Task<Comment> AddCommentAsync(Comment comment);
    }
}
