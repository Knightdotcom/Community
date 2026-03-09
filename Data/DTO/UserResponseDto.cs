namespace community_api.Data.DTO
{
    // DTO for anvandarsvar - returneras nar en anvandare registreras eller uppdateras
    // Innehaller bara publikt tillganglig information (inget losenord eller hash)
    public record UserResponseDto
    {
        // Anvandarens unika id-strang (GUID fran Identity)
        public string Id { get; init; } = string.Empty;

        // Anvandarnamn
        public string UserName { get; init; } = string.Empty;

        // E-postadress
        public string Email { get; init; } = string.Empty;

        // Kombinerat for- och efternamn (mappas av AutoMapper)
        public string FullName { get; init; } = string.Empty;
    }
}
