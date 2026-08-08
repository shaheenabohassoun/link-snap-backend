# LinkSnap Backend

<p align="center">
  <strong>ASP.NET Core API</strong> for a clean, production-minded URL shortener — JWT auth, link lifecycle, analytics, and permanent redirects.
</p>

<p align="center">
  <a href="https://github.com/shaheenabohassoun/link-snap-backend"><img alt="Repo" src="https://img.shields.io/badge/GitHub-link--snap--backend-0d6e5f?style=flat-square&logo=github" /></a>
  <img alt=".NET" src="https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet" />
  <img alt="SQL Server" src="https://img.shields.io/badge/SQL%20Server-EF%20Core-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white" />
  <img alt="License" src="https://img.shields.io/badge/license-MIT-informational?style=flat-square" />
</p>

---

## Overview

**LinkSnap** turns long URLs into short, shareable codes and tracks how they perform. This repository is the **backend only**: a layered ASP.NET Core Web API that powers authentication, shortening, management, analytics, and HTTP redirects.

Companion frontend: [link-snap-frontend](https://github.com/shaheenabohassoun/link-snap-frontend)

| Capability | Details |
| --- | --- |
| Auth | Register / login with ASP.NET Identity + JWT Bearer |
| Shorten | Public shorten endpoint; optional custom alias & expiry |
| Manage | List, get, update, and delete links (authenticated) |
| Analytics | Per-link totals, daily clicks, devices, referrers |
| Redirect | Root-level `/{shortCode}` permanent redirect |
| Docs | Swagger UI in Development |

---

## Architecture

Clean architecture with clear boundaries:

```text
LinkSnap.API              → Controllers, middleware, DI, Swagger, CORS
LinkSnap.Application      → Services, DTOs, validators, mappings
LinkSnap.Domain           → Entities & domain types
LinkSnap.Infrastructure   → EF Core, Identity, repositories, migrations
```

```text
Client (Angular)
        │  JWT + JSON
        ▼
┌───────────────────┐
│   LinkSnap.API    │  Auth / Links / Redirect
└─────────┬─────────┘
          │
┌─────────▼─────────┐
│   Application     │  ShortUrl + Analytics services
└─────────┬─────────┘
          │
┌─────────▼─────────┐
│  Infrastructure   │  SQL Server via EF Core
└───────────────────┘
```

---

## Tech stack

- **.NET 9** / ASP.NET Core Web API  
- **Entity Framework Core** + **PostgreSQL** (Neon-friendly for free hosting)  
- **ASP.NET Core Identity**  
- **JWT Bearer** authentication  
- **FluentValidation** for request validation  
- **AutoMapper** for entity ↔ DTO mapping  
- **Swashbuckle** (Swagger / OpenAPI)  
- Global **exception handling** middleware  

> Want this live on the internet? See **[DEPLOY.md](./DEPLOY.md)** (Neon + Render + Cloudflare Pages).

---

## Quick start

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- A **PostgreSQL** database (local Postgres or a free [Neon](https://neon.tech) project)
- (Optional) [EF Core tools](https://learn.microsoft.com/ef/core/cli/dotnet):  
  `dotnet tool install --global dotnet-ef`

### 1. Clone

```bash
git clone https://github.com/shaheenabohassoun/link-snap-backend.git
cd link-snap-backend
```

### 2. Configure

Edit `LinkSnap.API/appsettings.json` (or use User Secrets / environment variables):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=linksnap;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "Secret": "REPLACE_WITH_A_LONG_RANDOM_SECRET",
    "Issuer": "LinkSnap",
    "Audience": "LinkSnapUsers",
    "ExpiryMinutes": 60
  },
  "Cors": {
    "FrontendOrigins": "http://localhost:4200,http://127.0.0.1:4200"
  }
}
```

> **Security:** Never commit production secrets. Prefer [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) or environment variables.

### 3. Database

Migrations run automatically on startup. To apply manually:

```bash
dotnet ef database update --project LinkSnap.Infrastructure --startup-project LinkSnap.API
```

### 4. Run

```bash
cd LinkSnap.API
dotnet run --launch-profile http
```

| Resource | URL |
| --- | --- |
| API | `http://localhost:5002` |
| Swagger UI | `http://localhost:5002/swagger` |
| Health | `http://localhost:5002/health` |
| Example redirect | `http://localhost:5002/{shortCode}` |

CORS is configured for the Angular app on `http://localhost:4200`.

---

## API reference

Base URL (local): `http://localhost:5002`

### Auth — `api/auth`

| Method | Route | Auth | Description |
| --- | --- | --- | --- |
| `POST` | `/api/auth/register` | No | Create account; returns `{ token, user }` |
| `POST` | `/api/auth/login` | No | Sign in; returns `{ token, user }` |

**Register body**

```json
{
  "firstName": "Ada",
  "lastName": "Lovelace",
  "email": "ada@example.com",
  "password": "YourStrongPassword1!"
}
```

**Login body**

```json
{
  "email": "ada@example.com",
  "password": "YourStrongPassword1!"
}
```

Send the JWT on protected routes:

```http
Authorization: Bearer <token>
```

### Links — `api/links`

| Method | Route | Auth | Description |
| --- | --- | --- | --- |
| `POST` | `/api/links/shorten` | Optional* | Create a short link |
| `GET` | `/api/links` | Yes | List current user’s links |
| `GET` | `/api/links/{id}` | Yes | Get link by id |
| `GET` | `/api/links/{id}/analytics` | Yes | Click analytics for a link |
| `PUT` | `/api/links/{id}` | Yes | Update expiry / active flag |
| `DELETE` | `/api/links/{id}` | Yes | Delete a link |

\*If a Bearer token is present, the link is associated with that user.

**Shorten body**

```json
{
  "originalUrl": "https://example.com/very/long/path",
  "customAlias": "launch",
  "expiresAt": "2026-12-31T23:59:59Z"
}
```

`customAlias` and `expiresAt` are optional.

**Update body**

```json
{
  "expiresAt": null,
  "isActive": true
}
```

### Redirect

| Method | Route | Auth | Description |
| --- | --- | --- | --- |
| `GET` | `/{shortCode}` | No | Permanent redirect to the original URL |

---

## Example flow

```bash
# 1) Register
curl -s -X POST http://localhost:5002/api/auth/register \
  -H "Content-Type: application/json" \
  -d "{\"firstName\":\"Ada\",\"lastName\":\"Lovelace\",\"email\":\"ada@example.com\",\"password\":\"YourStrongPassword1!\"}"

# 2) Shorten (with token)
curl -s -X POST http://localhost:5002/api/links/shorten \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d "{\"originalUrl\":\"https://example.com\"}"

# 3) Open short URL in a browser
# http://localhost:5002/<shortCode>
```

---

## Project layout

```text
link-snap-backend/
├── LinkSnap.API/                 # HTTP host
│   ├── Controllers/              # Auth, Links, Redirect
│   ├── Middleware/               # Exception handling
│   ├── Program.cs
│   └── appsettings*.json
├── LinkSnap.Application/         # Use cases & contracts
│   ├── DTOs/
│   ├── Services/
│   ├── Validators/
│   └── Mappings/
├── LinkSnap.Domain/              # Core entities
├── LinkSnap.Infrastructure/      # EF Core, Identity, repos
│   ├── Migrations/
│   ├── Persistence/
│   └── Repositories/
└── README.md
```

---

## Configuration & CORS

| Setting | Purpose |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection |
| `JwtSettings:Secret` | Signing key for access tokens |
| `JwtSettings:Issuer` / `Audience` | Token validation |
| `JwtSettings:ExpiryMinutes` | Token lifetime |
| `Cors:FrontendOrigins` | Comma-separated allowed frontend origins |

Swagger is enabled for interactive exploration while developing.

---

## Related

| Repo | Role |
| --- | --- |
| [link-snap-backend](https://github.com/shaheenabohassoun/link-snap-backend) | This API |
| [link-snap-frontend](https://github.com/shaheenabohassoun/link-snap-frontend) | Angular client |

---

## License

This project is available under the MIT License — feel free to use it as a learning or portfolio reference.
