 
using community_api.Data;
using community_api.Data.Entities;
using community_api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace community_api
{
    public class Program
    {
        // Applikationens startpunkt - körs när programmet startar
        public static void Main(string[] args)
        {
            // Skapar en builder som används för att konfigurera applikationen
            var builder = WebApplication.CreateBuilder(args);

            // Registrerar databaskoppling (SQL Server via AppDbContext)
            builder.Services.AddDbContextExtension(builder);

            // Registrerar JWT-autentisering med inställningar från appsettings
            builder.Services.AddJwtAuth(builder);

            // Registrerar alla tjänster och repo:s i DI-containern (Scoped livstid)
            builder.Services.AddScopedServices();

            // Registrerar AutoMapper som hanterar mappning mellan entiteter och DTO:s
            builder.Services.AddAutoMapper(typeof(Program));

            // Registrerar ASP.NET Core Identity för användarhantering och lösenordshashning
            builder.Services.AddIdentityServices();

            // Registrerar controller-stöd så att API-endpoints kan hittas
            builder.Services.AddControllers();

            // Registrerar Swagger för API-dokumentation med JWT-stöd
            builder.Services.AddSwaggerServices();

            // Bygger applikationen med alla registrerade tjänster
            var app = builder.Build();

            // Kör migrationer och seed kategorier vid uppstart
            using (var scope = app.Services.CreateScope())
            {
                // Hämtar databaskontext från DI-containern
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Kör alla EF Core-migrationer automatiskt (skapar databasen om den inte finns)
                context.Database.Migrate();

                // Lägger till standardkategorier om databasen är tom
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(
                        new Category { CategoryName = "Träning" },
                        new Category { CategoryName = "Mode" },
                        new Category { CategoryName = "Hälsa" },
                        new Category { CategoryName = "Mat" },
                        new Category { CategoryName = "Resor" }
                    );
                    context.SaveChanges();
                }
            }

            // Aktiverar Swagger middleware som genererar OpenAPI-specifikation
            app.UseSwagger();

            // Aktiverar Swagger UI (webbgränssnittet för att testa API:et)
            app.UseSwaggerUI(c =>
            {
                // Pekar Swagger UI till rätt swagger.json-fil
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Community API v1");
            });

            // Aktiverar routing så att HTTP-anrop matchas till rätt controller
            app.UseRouting();

            // Aktiverar CORS så att externa klienter (t.ex. React-appar) kan anropa API:et
            app.UseCors(b => b.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());

            // Aktiverar autentisering (kontrollerar JWT-token)
            app.UseAuthentication();

            // Aktiverar auktorisering (kontrollerar behörigheter via [Authorize])
            app.UseAuthorization();

            // Mappar alla controllers till deras respektive routes
            app.MapControllers();

            // Startar webbservern och lyssnar på inkommande anrop
            app.Run();
        }
    }
}
