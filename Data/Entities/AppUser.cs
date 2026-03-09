using Microsoft.AspNetCore.Identity;

namespace community_api.Data.Entities
{
    // Användarentitet som utvidgar Identitys inbyggda IdentityUser
    // IdentityUser tillhandahåller redan: Id, UserName, Email, PasswordHash m.m.
    public class AppUser : IdentityUser
    {
        // Valfritt förnamn - ingick inte i IdentityUser
        public string? FirstName { get; set; }

        // Valfritt efternamn - ingick inte i IdentityUser
        public string? LastName { get; set; }

        // Navigeringsegenskap: användaren kan ha många inlägg
        public List<Post> Posts { get; set; } = new();

        // Navigeringsegenskap: användaren kan ha många kommentarer
        public List<Comment> Comments { get; set; } = new();
    }
}
