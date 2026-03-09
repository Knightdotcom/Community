namespace community_api.Data.DTO
{
    // DTO for laggande till en ny kommentar
    // Skickas i request-bodyn nar en kommentar skapas
    public record CommentAddDto(
        // Kommentarens text - kravs
        string CommentText
    );
}
