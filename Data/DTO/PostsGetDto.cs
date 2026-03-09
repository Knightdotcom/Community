namespace community_api.Data.DTO
{
    // DTO for listvyn av inlagg
    // Anvands nar alla inlagg listas (GET api/post)
    public record PostsGetDto
    {
        // Inlaggets unika id
        public int Id { get; set; }

        // Titeln pa inlagget
        public string Title { get; set; } = string.Empty;

        // Textinnehallet i inlagget
        public string Text { get; set; } = string.Empty;

        // Kategorinamnet (mappas fran navigeringsegenskapen)
        public string Category { get; set; } = string.Empty;

        // Forfattarens anvandarnamn (mappas fran User.UserName)
        public string UserName { get; set; } = string.Empty;

        // Tidsstampel for nar inlagget skapades
        public DateTime CreatedAt { get; set; }
    }
}
