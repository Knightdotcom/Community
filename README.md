# Community API

ASP.NET Web API för ett community med blogginlägg, användare, kategorier och kommentarer.
Byggd med .NET 10, Entity Framework Core och SQL Server. Dokumenterad med Swagger.

---

## Kom igång

### 1. Krav
- .NET 10 SDK
- SQL Server (lokalt eller via Azure)
- Postman eller webbläsare för Swagger

### 2. Konfigurera databas

Öppna `appsettings.json` och ersätt connection string:

```json
"Default": "Server=YOUR_SERVER;Database=CommunityDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
```

**Exempel för lokal SQL Server:**
```json
"Default": "Server=localhost;Database=CommunityDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

**Exempel för SQL Server Express:**
```json
"Default": "Server=.\\SQLEXPRESS;Database=CommunityDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

> Databasen och tabellerna skapas automatiskt när appen startas första gången.
> Kategorierna (Träning, Mode, Hälsa, Mat, Resor) seedas automatiskt.

### 3. Starta API:et

```bash
dotnet run
```

Swagger UI öppnas på: `http://localhost:{PORT}/swagger`

---

## Endpoints

### Användare

| Metod | URL | Beskrivning | Auth |
|---|---|---|---|
| POST | `/api/users` | Registrera ny användare | Nej |
| POST | `/api/users/login` | Logga in, returnerar userId | Nej |
| PUT | `/api/users/{id}` | Uppdatera användare | Nej |
| DELETE | `/api/users/{id}` | Ta bort användare | Nej |

#### POST `/api/users` — Registrera
```json
{
  "username": "anna",
  "password": "hemligt123",
  "email": "anna@mail.com"
}
```
**Svar:**
```json
{
  "userId": 1,
  "username": "anna",
  "email": "anna@mail.com"
}
```

#### POST `/api/users/login` — Logga in
```json
{
  "username": "anna",
  "password": "hemligt123"
}
```
**Svar:**
```json
{
  "userId": 1,
  "username": "anna"
}
```
> Spara `userId` — det används i alla anrop som kräver inloggning.

#### PUT `/api/users/{id}` — Uppdatera
```json
{
  "username": "anna2",
  "email": "ny@mail.com",
  "password": "nyttlösenord"
}
```
> Alla fält är valfria. Skicka bara de du vill uppdatera.

#### DELETE `/api/users/{id}` — Ta bort
Ingen body. Returnerar `204 No Content`.

---

### Kategorier

| Metod | URL | Beskrivning |
|---|---|---|
| GET | `/api/categories` | Lista alla kategorier |

#### GET `/api/categories`
```json
[
  { "categoryId": 1, "name": "Träning" },
  { "categoryId": 2, "name": "Mode" },
  { "categoryId": 3, "name": "Hälsa" },
  { "categoryId": 4, "name": "Mat" },
  { "categoryId": 5, "name": "Resor" }
]
```

---

### Inlägg

| Metod | URL | Beskrivning | Auth |
|---|---|---|---|
| GET | `/api/posts` | Lista alla inlägg | Nej |
| GET | `/api/posts?title=xxx` | Sök på titel (delträff) | Nej |
| GET | `/api/posts?categoryId=1` | Filtrera på kategori | Nej |
| GET | `/api/posts/{id}` | Hämta enskilt inlägg med kommentarer | Nej |
| POST | `/api/posts` | Skapa nytt inlägg | Ja (userId i body) |
| PUT | `/api/posts/{id}` | Uppdatera inlägg (bara ägaren) | Ja (userId i body) |
| DELETE | `/api/posts/{id}?userId={userId}` | Ta bort inlägg (bara ägaren) | Ja (userId i query) |

#### POST `/api/posts` — Skapa inlägg
```json
{
  "title": "Mitt träningspass",
  "text": "Idag sprang jag 10 km!",
  "categoryId": 1,
  "userId": 1
}
```

#### PUT `/api/posts/{id}` — Uppdatera inlägg
```json
{
  "title": "Uppdaterad titel",
  "text": "Uppdaterad text",
  "categoryId": 2,
  "userId": 1
}
```
> `userId` måste matcha inläggets ägare, annars `403 Forbidden`.
> `title`, `text` och `categoryId` är valfria.

#### DELETE `/api/posts/{id}?userId=1`
Ingen body. Returnerar `204 No Content`.
> `userId` måste matcha inläggets ägare, annars `403 Forbidden`.

#### GET `/api/posts/{id}` — Svar med kommentarer
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

---

### Kommentarer

| Metod | URL | Beskrivning | Auth |
|---|---|---|---|
| POST | `/api/comments` | Lägg till kommentar | Ja (userId i body) |
| GET | `/api/comments/post/{postId}` | Hämta kommentarer för ett inlägg | Nej |

#### POST `/api/comments` — Kommentera
```json
{
  "text": "Bra inlägg!",
  "postId": 1,
  "userId": 2
}
```
> Man kan inte kommentera sina egna inlägg — returnerar `400 Bad Request`.

---

## Typiskt testflöde i Postman

1. **Registrera** användare 1 via `POST /api/users`
2. **Registrera** användare 2 via `POST /api/users`
3. **Logga in** med användare 1 via `POST /api/users/login` → spara `userId`
4. **Hämta kategorier** via `GET /api/categories` → notera `categoryId`
5. **Skapa inlägg** via `POST /api/posts` med userId från steg 3
6. **Sök inlägg** via `GET /api/posts?title=xxx` eller `GET /api/posts?categoryId=1`
7. **Kommentera** med användare 2 via `POST /api/comments`
8. **Försök kommentera** med användare 1 (ägaren) → ska ge `400`
9. **Uppdatera inlägg** med rätt userId → ska lyckas
10. **Försök uppdatera** med fel userId → ska ge `403`

---

## Projektstruktur

```
community-api/
├── Controllers/
│   ├── UserController.cs       # Registrering, login, uppdatering, borttagning
│   ├── PostController.cs       # CRUD för inlägg + sökning
│   ├── CategoryController.cs   # Lista kategorier
│   └── CommentController.cs    # Skapa och läsa kommentarer
├── Data/
│   └── CommunityContext.cs     # EF Core DbContext
├── Models/
│   ├── User.cs
│   ├── Post.cs
│   ├── Category.cs
│   └── Comment.cs
├── appsettings.json            # Connection string (ändra här)
├── Program.cs                  # App-konfiguration, CORS, Swagger, seed
└── community-api.csproj        # Paketberoenden
```

## Paket

| Paket | Användning |
|---|---|
| `Microsoft.EntityFrameworkCore.SqlServer` | EF Core med SQL Server |
| `Microsoft.EntityFrameworkCore.Tools` | EF Core CLI-verktyg |
| `Swashbuckle.AspNetCore` | Swagger-dokumentation |

---

## Säkerhet

- Lösenord hashas med **SHA-256** innan de sparas i databasen
- Klartext-lösenord lagras aldrig
- Ägarskydd: bara skaparen av ett inlägg kan uppdatera eller ta bort det
