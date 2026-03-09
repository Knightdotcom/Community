using community_api.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace community_api.Data
{
    // Databaskontexten - kopplar applikationen till SQL Server via Entity Framework Core
    // Ärver från IdentityDbContext för att få med Identity-tabellerna (AspNetUsers, AspNetRoles, etc.)
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        // DbSet-egenskaper representerar tabeller i databasen
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }

        // Konstruktor - tar emot konfigurationsalternativ via Dependency Injection
        public AppDbContext(DbContextOptions options) : base(options) { }

        // Konfigurerar relationer och begränsningar i databasen
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Anropar bas-implementationen för att konfigurera Identity-tabellerna
            base.OnModelCreating(modelBuilder);

            // Konfigurerar relationen Comment -> Post
            // NoAction: kommentarer måste tas bort manuellt innan inlägget tas bort
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            // Konfigurerar relationen Comment -> AppUser
            // NoAction: undviker cascade-cykelproblem i SQL Server
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
