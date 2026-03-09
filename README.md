# Community API

Ett REST API för en community-plattform med blogginlägg, kommentarer, kategorier och användarhantering. Byggt med ASP.NET Core 9, Entity Framework Core, ASP.NET Core Identity och JWT-autentisering.

---

## Teknikstack

| Teknologi | Användning |
|-----------|------------|
| **ASP.NET Core Web API (.NET 9)** | REST API och affärslogik |
| **Entity Framework Core** | ORM, migrationer och databasåtkomst |
| **SQL Server** | Relationsdatabas |
| **ASP.NET Core Identity** | Användarhantering och lösenordshashning |
| **JWT (JSON Web Tokens)** | Stateless autentisering |
| **AutoMapper** | Mappning mellan entiteter och DTO:er |
| **Swagger / Swashbuckle** | Interaktiv API-dokumentation med JWT-stöd |

---

## Arkitektur

Projektet följer en tydlig lagerstruktur med Service-Repository-mönstret:

```
community-api/
├── Controllers/            → HTTP-endpoints, validering och statuskoder
├── Core/
│   ├── Interfaces/         → Kontrakt för services (IUserService, IPostService m.fl.)
│   └── Services/           → Affärslogik + ServiceResult<T> returnmönster
└── Data/
    ├── Entities/           → Databasmodeller (AppUser, Post, Comment, Category)
    ├── Repos/              → Databasåtkomst via Entity Framework
    ├── Interfaces/         → Kontrakt för repos (IPostRepo, ICommentRepo)
    ├── Profiles/           → AutoMapper-mappningar
    ├── DTO/                → Dataöverföringsobjekt
    └── AppDbContext.cs     → EF Core databaskonfiguration
```

Alla serviceoperationer returnerar `ServiceResult<T>` — ett generiskt svarsobjekt med antingen data vid lyckad operation, eller felmeddelanden vid misslyckad. Controllers kastar aldrig exceptions för förväntade felfall.

---

## Kom igång

### Krav
- .NET 9 SDK
- SQL Server (lokalt eller via Azure)
- Valfri REST-klient (Swagger UI, Postman, etc.)

### 1. Konfigurera databas

Öppna `appsettings.json` och ange din connection string:

```json
"ConnectionStrings": {
  "Default": "Server=localhost;Database=CommunityDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**För SQL Server Express:**
```json
"Default": "Server=.\\SQLEXPRESS;Database=CommunityDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 2. Konfigurera JWT

I `appsettings.json`, ange en hemlig nyckel för JWT-signering:

```json
"JwtSettings": {
  "SecretKey": "DIN_HEMLIGA_NYCKEL_MINST_32_TECKEN",
  "Issuer": "CommunityApi",
  "Audience": "CommunityClient"
}
```

### 3. Starta API:et

```bash
dotnet run
```

> Databasen skapas och migreras automatiskt vid uppstart.
> Kategorierna **Träning**, **Mode**, **Hälsa**, **Mat** och **Resor** seedas automatiskt om databasen är tom.

Swagger UI finns på: `http://localhost:{PORT}/swagger`

---

## API-endpoints

### Autentisering

| Metod | Endpoint | Beskrivning | Auth |
|-------|----------|-------------|------|
| POST | `/api/auth/register` | Registrera ny användare | Nej |
| POST | `/api/auth/login` | Logga in — returnerar JWT-token | Nej |

#### Registrera användare — `POST /api/auth/register`
```json
{
  "username": "anna",
  "email": "anna@mail.com",
  "password": "Hemligt123!",
  "firstName": "Anna",
  "lastName": "Svensson"
}
```

#### Logga in — `POST /api/auth/login`
```json
{
  "username": "anna",
  "password": "Hemligt123!"
}
```
**Svar:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```
> Använd token i `Authorization: Bearer <token>` headern för alla skyddade endpoints.

---

### Användare

| Metod | Endpoint | Beskrivning | Auth |
|-------|----------|-------------|------|
| PUT | `/api/user` | Uppdatera din profil | JWT |
| DELETE | `/api/user?id={id}` | Ta bort användare | Nej |

---

### Kategorier

| Metod | Endpoint | Beskrivning | Auth |
|-------|----------|-------------|------|
| GET | `/api/category` | Lista alla kategorier | Nej |

**Svar:**
```json
[
  { "categoryId": 1, "categoryName": "Träning" },
  { "categoryId": 2, "categoryName": "Mode" },
  { "categoryId": 3, "categoryName": "Hälsa" },
  { "categoryId": 4, "categoryName": "Mat" },
  { "categoryId": 5, "categoryName": "Resor" }
]
```

---

### Inlägg

| Metod | Endpoint | Beskrivning | Auth |
|-------|----------|-------------|------|
| GET | `/api/post` | Lista alla inlägg | Nej |
| GET | `/api/post?title=xxx` | Sök på titel | Nej |
| GET | `/api/post?categoryId=1` | Filtrera på kategori | Nej |
| GET | `/api/post/{id}` | Hämta inlägg med kommentarer | Nej |
| POST | `/api/post` | Skapa nytt inlägg | JWT |
| PUT | `/api/post?postId={id}` | Uppdatera inlägg | JWT |
| DELETE | `/api/post?postId={id}` | Ta bort inlägg | JWT |

#### Skapa inlägg — `POST /api/post`
```json
{
  "title": "Mitt träningspass",
  "text": "Idag sprang jag 10 km!",
  "categoryId": 1
}
```

#### Detaljerat inlägg med kommentarer — `GET /api/post/{id}`
```json
{
  "postId": 1,
  "title": "Mitt träningspass",
  "text": "Idag sprang jag 10 km!",
  "createdAt": "2024-01-15T10:30:00",
  "author": "anna",
  "category": "Träning",
  "comments": [
    {
      "commentId": 1,
      "text": "Bra jobbat!",
      "createdAt": "2024-01-15T11:00:00",
      "author": "erik"
    }
  ]
}
```

> Endast inläggets ägare kan uppdatera eller ta bort det — annars `403 Forbidden`.

---

### Kommentarer

| Metod | Endpoint | Beskrivning | Auth |
|-------|----------|-------------|------|
| POST | `/api/comment` | Lägg till kommentar | JWT |
| GET | `/api/comment/post/{postId}` | Hämta kommentarer för ett inlägg | Nej |

#### Kommentera — `POST /api/comment`
```json
{
  "text": "Bra inlägg!",
  "postId": 1
}
```
> Man kan inte kommentera sina egna inlägg — returnerar `400 Bad Request`.

---

## Typiskt testflöde

1. **Registrera** två användare via `POST /api/auth/register`
2. **Logga in** med användare 1 via `POST /api/auth/login` → kopiera JWT-token
3. **Hämta kategorier** via `GET /api/category` → notera ett `categoryId`
4. **Skapa inlägg** via `POST /api/post` med JWT-token i headern
5. **Sök inlägg** via `GET /api/post?title=träning`
6. **Logga in** med användare 2 och **kommentera** inlägget via `POST /api/comment`
7. **Försök kommentera** med användare 1 (ägaren) → förväntat `400`
8. **Uppdatera inlägg** med rätt token → ska lyckas
9. **Försök uppdatera** med fel token → förväntat `403`

---

## NuGet-paket

| Paket | Version | Användning |
|-------|---------|------------|
| `AutoMapper` | 14.0.0 | Entitet ↔ DTO-mappning |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 9.0.6 | JWT-validering |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 9.0.6 | Användarhantering |
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.6 | SQL Server-driver |
| `Microsoft.EntityFrameworkCore.Tools` | 9.0.6 | EF Core CLI |
| `Swashbuckle.AspNetCore` | 8.1.1 | Swagger UI |
| `Swashbuckle.AspNetCore.Annotations` | 8.1.1 | Swagger-annotationer |

---

## Säkerhet

- Lösenord hashas av **ASP.NET Core Identity** (bcrypt med salt)
- JWT-token valideras vid varje skyddat anrop
- Ägarskydd: bara skaparen av ett inlägg kan uppdatera eller ta bort det
- CORS är konfigurerat för att tillåta externa klienter
