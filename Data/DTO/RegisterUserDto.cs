using System.ComponentModel.DataAnnotations;

namespace community_api.Data.DTO
{
    // DTO for registrering av ny anvandare
    // Valideras automatiskt av ASP.NET Core via DataAnnotations
    public class RegisterUserDto
    {
        // Anvandarnamn - kravs, 3-50 tecken
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        // E-postadress - kravs och maste vara giltig e-postformat
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Losenord - kravs, minst 5 tecken
        [Required, MinLength(5)]
        public string Password { get; set; } = string.Empty;

        // Valfritt fornamn - 2-50 tecken om angivet
        [StringLength(50, MinimumLength = 2)]
        public string? FirstName { get; set; }

        // Valfritt efternamn - 2-50 tecken om angivet
        [StringLength(50, MinimumLength = 2)]
        public string? LastName { get; set; }
    }
}
