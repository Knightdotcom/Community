// Importerar namnrymder för tjänster, DI, JWT, EF Core och Swagger
using community_api.Core.Interfaces;
using community_api.Core.Services;
using community_api.Data;
using community_api.Data.Entities;
using community_api.Data.Interfaces;
using community_api.Data.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace community_api.Extensions
{
    // Statisk klass med extension methods för IServiceCollection
    // Används i Program.cs för att hålla konfigurationen organiserad och läsbar
    public static class ServiceExtensions
    {
        // Konfigurerar Swagger/OpenAPI för API-dokumentation med JWT-stöd
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(opt =>
            {
                // Definierar API-dokumentets titel, version och beskrivning
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Community API",
                    Version = "v1",
                    Description = "API för community med inlägg, kommentarer och användare"
                });

                // Aktiverar [SwaggerOperation]-annotationer på controller-metoder
                opt.EnableAnnotations();

                // Lägger till en Bearer Token-knapp i Swagger UI för autentisering
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Klistra in din JWT-token här"
                });

                // Kräver att Bearer-token skickas med för skyddade endpoints i Swagger UI
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });
            });

            return services;
        }

        // Registrerar alla tjänster (Core/Services) och repo:s (Data/Repos) i DI-containern
        // Scoped = en ny instans skapas per HTTP-anrop
        public static IServiceCollection AddScopedServices(this IServiceCollection services)
        {
            // Affärslogik-tjänster (Applikationslagret)
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();

            // Databasnära repo:s (Datalagret)
            services.AddScoped<IPostRepo, PostRepo>();
            services.AddScoped<ICommentRepo, CommentRepo>();

            return services;
        }

        // Konfigurerar JWT Bearer-autentisering
        // Validerar token vid varje inkommande anrop till skyddade endpoints
        public static IServiceCollection AddJwtAuth(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddAuthentication(options =>
            {
                // Sätter JWT Bearer som standardschema för autentisering och utmaningar
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Definierar hur token ska valideras vid varje anrop
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,           // Kontrollerar att token kom från rätt utfärdare
                    ValidateAudience = true,          // Kontrollerar att token riktas till rätt mottagare
                    ValidateLifetime = true,          // Kontrollerar att token inte har gått ut
                    ValidateIssuerSigningKey = true,  // Kontrollerar token-signaturen med hemlig nyckel
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
                };
            });

            return services;
        }

        // Konfigurerar ASP.NET Core Identity för användarhantering
        // Identity hanterar lösenordshashning, validering och lagring av användare
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentityCore<AppUser>(options =>
            {
                // Förenklade lösenordskrav för utvecklingsmiljön
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<AppDbContext>() // Lagrar användare i SQL-databasen
            .AddSignInManager();                       // Aktiverar inloggningsfunktionalitet

            return services;
        }

        // Registrerar AppDbContext med SQL Server som databas
        // Hämtar anslutningssträngen från appsettings.Development.json
        public static IServiceCollection AddDbContextExtension(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }
    }
}
