using System.ComponentModel.DataAnnotations;

namespace community_api.Data.Entities
{
    // Kategorientitet - representerar en kategori som inlägg kan tillhöra
    // Kategorier skapas vid uppstart via seedning i Program.cs
    public class Category
    {
        // Primärnyckel - genereras automatiskt av databasen
        [Key]
        public int CategoryId { get; set; }

        // Kategorinamnet - krävs (t.ex. "Träning", "Mode", "Hälsa")
        [Required]
        public string CategoryName { get; set; } = string.Empty;
    }
}
