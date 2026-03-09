namespace community_api.Data.DTO
{
    // DTO for svar efter skapande av ett nytt inlagg
    // Returneras med HTTP 201 Created
    public record PostAddResponseDto
    {
        // Det genererade inlaggs-id:t
        public int PostId { get; set; }

        // Titeln pa det skapade inlagget
        public string? Title { get; set; }

        // Texten i det skapade inlagget
        public string? Text { get; set; }

        // Kategorinamnet (mappas fran Category.CategoryName)
        public string? Category { get; set; }

        // Id:t pa anvandaren som skapade inlagget
        public string? UserId { get; set; }
    }
}
