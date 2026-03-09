using System.ComponentModel.DataAnnotations;

namespace community_api.Data.Entities
{
    // Kommentarentitet - representerar en kommentar på ett inlägg
    public class Comment
    {
        // Primärnyckel - genereras automatiskt av databasen
        [Key]
        public int CommentId { get; set; }

        // Kommentarens text - krävs
        [Required]
        public string CommentText { get; set; } = string.Empty;

        // Valfri sekundärnyckel till Post - vilket inlägg kommentaren tillhör
        public int? PostId { get; set; }

        // Navigeringsegenskap till inlägget
        public Post Post { get; set; } = null!;

        // Framnyckel till AppUser - vem som skrev kommentaren
        public string UserId { get; set; } = string.Empty;

        // Navigeringsegenskap till författaren
        public AppUser User { get; set; } = null!;

        // Tidsstämpel för när kommentaren skapades - satt automatiskt till UTC-tid
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
