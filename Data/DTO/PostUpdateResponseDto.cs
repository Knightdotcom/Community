namespace community_api.Data.DTO
{
    // DTO for svar efter uppdatering av ett inlagg
    // Returneras med HTTP 200 OK
    public class PostUpdateResponseDto
    {
        // Inlaggets unika id
        public int Id { get; set; }

        // Titeln pa det uppdaterade inlagget
        public string Title { get; set; } = string.Empty;

        // Textinnehallet i det uppdaterade inlagget
        public string Text { get; set; } = string.Empty;

        // Kategorinamnet pa det uppdaterade inlagget
        public string CategoryName { get; set; } = string.Empty;

        // Id:t pa anvandaren som ager inlagget
        public string UserId { get; set; } = string.Empty;
    }
}
