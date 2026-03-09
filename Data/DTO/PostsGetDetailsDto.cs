namespace community_api.Data.DTO
{
    // DTO for detaljvyn av ett enskilt inlagg
    // Anvands nar ett specifikt inlagg hamtas (GET api/post/{id})
    // Inkluderar alla kommentarer pa inlagget
    public record PostsGetDetailsDto
    {
        // Inlaggets unika id
        public int Id { get; set; }

        // Titeln pa inlagget
        public string Title { get; set; } = string.Empty;

        // Textinnehallet i inlagget
        public string Text { get; set; } = string.Empty;

        // Kategorinamnet (mappas fran navigeringsegenskapen)
        public string Category { get; set; } = string.Empty;

        // Forfattarens anvandarnamn
        public string UserName { get; set; } = string.Empty;

        // Tidsstampel for nar inlagget skapades
        public DateTime CreatedAt { get; set; }

        // Lista med alla kommentarer pa inlagget
        public List<CommentGetDto> Comments { get; set; } = new();
    }
}
