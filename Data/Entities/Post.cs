using System.ComponentModel.DataAnnotations;

namespace community_api.Data.Entities
{
    // Inläggsentitet - representerar ett blogginlägg i databasen
    public class Post
    {
        // Primärnyckel - genereras automatiskt av databasen
        [Key]
        public int PostId { get; set; }

        // Titel på inlägget - krävs, 2-100 tecken
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;

        // Innehållet i inlägget - krävs, ingen maxgräns
        [Required]
        public string Text { get; set; } = string.Empty;

        // Navigeringsegenskap: ett inlägg kan ha många kommentarer
        public List<Comment> Comments { get; set; } = new();

        // Sekundärnyckel till AppUser - vem som skapade inlägget
        public string UserId { get; set; } = string.Empty;

        // Navigeringsegenskap till författaren
        public AppUser User { get; set; } = null!;

        // Valfri sekundärnyckel till Category - ett inlägg tillhör en kategori
        public int? CategoryId { get; set; }

        // Navigeringsegenskap till kategorin
        public Category Category { get; set; } = null!;

        // Tidsstämpel för när inlägget skapades - satt automatiskt till UTC-tid
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
