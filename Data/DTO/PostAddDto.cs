using System.ComponentModel.DataAnnotations;

namespace community_api.Data.DTO
{
    // DTO for skapande av nytt inlagg
    // Valideras automatiskt av ASP.NET Core via DataAnnotations
    public record PostAddDto
    {
        // Titel pa inlagget - kravs, 2-100 tecken
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        // Innehallet i inlagget - kravs, 2-2000 tecken
        [Required]
        [StringLength(2000, MinimumLength = 2)]
        public string Text { get; set; } = string.Empty;

        // Kategori-id - kravs, maste vara ett giltigt id fran kategorilistan
        [Required]
        public int CategoryId { get; set; }
    }
}
