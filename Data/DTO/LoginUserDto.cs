namespace community_api.Data.DTO
{
    // DTO for inloggningsbegaran
    // Tar emot anvandarnamn eller e-post samt losenord
    public record LoginUserDto(string UserNameOrEmail, string Password);
}
