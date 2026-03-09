namespace community_api.Data.DTO
{
    // DTO for visning av en kommentar i inlaggslistan
    // Ingaar i PostsGetDetailsDto.Comments-listan
    public record CommentGetDto
    {
        // Kommentarens unika id
        public int Id { get; set; }

        // Anvandarnamnet pa den som skrev kommentaren
        public string? UserName { get; set; }

        // Kommentarens text
        public string? CommentText { get; set; }

        // Tidsstampel for nar kommentaren skapades
        public DateTime CreatedAt { get; set; }
    }
}
