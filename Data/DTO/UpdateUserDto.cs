using System.ComponentModel.DataAnnotations;

namespace community_api.Data.DTO
{
    // DTO for uppdatering av anvandarprofil
    // Alla falt ar valfria - bara angivna falt uppdateras (partial update via AutoMapper)
    public class UpdateUserDto
    {
        // Valfritt nytt fornamn - 2-50 tecken om angivet
        [StringLength(50, MinimumLength = 2)]
        public string? FirstName { get; set; }

        // Valfritt nytt efternamn - 2-50 tecken om angivet
        [StringLength(50, MinimumLength = 2)]
        public string? LastName { get; set; }

        // Valfritt nytt anvandarnamn - 3-30 tecken om angivet
        [StringLength(30, MinimumLength = 3)]
        public string? UserName { get; set; }

        // Valfri ny e-postadress - maste vara giltig e-postformat om angiven
        [EmailAddress]
        public string? Email { get; set; }

        // Valfritt nytt losenord - 5-50 tecken om angivet
        [StringLength(50, MinimumLength = 5)]
        public string? Password { get; set; }
    }
}
