# Deploy LinkSnap (Free)

This guide gets **frontend + backend + database** online for real users.

| Piece | Free service |
| --- | --- |
| Database | [Neon](https://neon.tech) (PostgreSQL) |
| Backend API | [Render](https://render.com) (Docker) |
| Frontend | [Cloudflare Pages](https://pages.cloudflare.com) |

---

## Step 1 — Neon (database)

1. Open https://neon.tech and click **Sign up** (GitHub is easiest).
2. Create a project named `linksnap`.
3. Open **Dashboard → Connection details**.
4. Copy the connection string that looks like:

```text
Host=ep-xxxx.region.aws.neon.tech;Database=neondb;Username=xxx;Password=xxx;SSL Mode=Require;Trust Server Certificate=true
```

> If Neon shows a URI (`postgresql://...`), convert it or use Neon’s “.NET / Npgsql” format if available.

**When done:** paste the connection string in chat (or keep it ready for Render env vars).  
Do **not** commit this password to GitHub.

---

## Step 2 — Render (backend API)

1. Open https://render.com → **Sign up with GitHub**.
2. **New → Web Service**.
3. Connect repo `shaheenabohassoun/link-snap-backend`.
4. Settings:
   - **Runtime:** Docker
   - **Branch:** `main`
   - **Instance type:** Free
5. **Environment variables:**

| Key | Value |
| --- | --- |
| `ConnectionStrings__DefaultConnection` | *(Neon connection string from Step 1)* |
| `JwtSettings__Secret` | long random string (32+ chars) |
| `JwtSettings__Issuer` | `LinkSnap` |
| `JwtSettings__Audience` | `LinkSnapUsers` |
| `JwtSettings__ExpiryMinutes` | `60` |
| `Cors__FrontendOrigins` | *(Cloudflare URL later — start with `http://localhost:4200`)* |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

6. Deploy → wait until status is **Live**.
7. Copy your API URL, e.g. `https://link-snap-backend.onrender.com`.
8. Check health: `https://YOUR-API.onrender.com/health`
9. Swagger: `https://YOUR-API.onrender.com/swagger`

> Free Render services **sleep** after idle time. First request after sleep can take ~30–60s.

---

## Step 3 — Frontend API URL

In `link-snap-frontend`:

1. Set `src/environments/environment.prod.ts` → `apiUrl` to your Render URL (no trailing slash).
2. Commit & push `main`.

---

## Step 4 — Cloudflare Pages (frontend)

1. Open https://dash.cloudflare.com → Sign up.
2. **Workers & Pages → Create → Pages → Connect to Git**.
3. Select `shaheenabohassoun/link-snap-frontend`.
4. Build settings:

| Setting | Value |
| --- | --- |
| Framework preset | None / Angular |
| Build command | `npm run build:pages` |
| Build output directory | `dist/link-snap-frontend/browser` |
| Root directory | `/` |
| Node version | `20` (or 22) |

5. Deploy → copy the Pages URL, e.g. `https://link-snap-frontend.pages.dev`.

---

## Step 5 — Wire CORS

Back on Render → Environment:

```text
Cors__FrontendOrigins=https://link-snap-frontend.pages.dev,http://localhost:4200
```

Redeploy the API (or Manual Deploy).

---

## Step 6 — Smoke test

1. Open the Cloudflare URL.
2. Register a user.
3. Create a short link.
4. Open `https://YOUR-API.onrender.com/{shortCode}` — should redirect.

---

## Local development notes

- Backend now uses **PostgreSQL** (Npgsql), not LocalDB/SQL Server.
- For local work, either:
  - Use the same Neon connection string in `appsettings.Development.json` / User Secrets, or
  - Run Postgres locally and update `ConnectionStrings:DefaultConnection`.
