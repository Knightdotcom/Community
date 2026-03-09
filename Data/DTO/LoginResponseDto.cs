namespace community_api.Data.DTO
{
    // DTO for inloggningssvar
    // Innehaller den genererade JWT-token som anvandaren sparar och skickar med framtida anrop
    public record LoginResponseDto
    {
        // JWT-token som anvands i Authorization-headern: } . q!"Bearer {Token}"! . q{
        public string Token { get; init; } = string.Empty;
    }
}
