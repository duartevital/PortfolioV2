# Vital Photography — CLAUDE.md

This file documents the project structure, how to run each service, environment variables, API conventions, and deployment notes for Claude Code and contributors.

---

## Project Overview

Personal photography portfolio ("Vital Photography") — a dark, moody, masonry-grid gallery site with a JWT-protected admin panel. The owner is a semi-professional hobbyist photographer.

**Genres / filter categories:** Landscape / Nature · Street / Urban

---

## Monorepo Structure

```
/frontend          Nuxt 4 + Vue 3 + TypeScript + Tailwind CSS
/backend           ASP.NET Core 8 Web API (C#)
docker-compose.yml Runs both services with one command
CLAUDE.md          This file
.env.example       Secret template (copy to .env, never commit)
.gitignore
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Nuxt 4, Vue 3, TypeScript, Tailwind CSS |
| Backend | ASP.NET Core 8 Web API, C# |
| Database | SQLite (dev + prod, file on Render persistent disk) |
| Image storage | Local filesystem (Render persistent disk in prod) |
| Auth | JWT, single-user, no third-party auth service |

---

## Running the Frontend

```bash
cd frontend
npm install
npm run dev        # http://localhost:3000
npm run build      # production build
npm run preview    # preview production build
```

### Frontend Env Vars

Create `frontend/.env` (never commit):

```
NUXT_PUBLIC_API_BASE_URL=http://localhost:5000   # backend API base (browser-side)
NUXT_PUBLIC_SITE_URL=http://localhost:3000       # canonical site URL (for SEO/sitemap)
```

When running in Docker, also set:
```
NUXT_API_BASE_URL=http://backend:8080   # backend API base (SSR, internal Docker network)
```

---

## Running the Backend

```bash
cd backend
dotnet restore
dotnet run --project VitalPhotography.Api        # http://localhost:5000
dotnet watch --project VitalPhotography.Api      # hot-reload dev server
```

### Backend Env Vars

Set via `backend/appsettings.Development.json` or environment variables (env vars take precedence in production):

| Variable | Description |
|---|---|
| `ConnectionStrings__DefaultConnection` | SQLite file path (`Data Source=vital-photography.db` in dev) |
| `Storage__Root` | Directory for uploaded photos (defaults to `wwwroot/uploads` in dev) |
| `Jwt__Secret` | Secret key for signing JWTs (min 32 chars) |
| `Jwt__Issuer` | JWT issuer (e.g. `vital-photography`) |
| `Jwt__Audience` | JWT audience (e.g. `vital-photography-admin`) |
| `Jwt__ExpiryMinutes` | Token lifetime in minutes (default 60) |
| `Admin__PasswordHash` | Bcrypt hash of the single admin password |
| `Cors__AllowedOrigins` | Comma-separated allowed origins (e.g. `https://your-app.vercel.app`) |

Local dev uses SQLite with no extra credentials needed.

---

## API Conventions

- Base path: `/api/v1`
- All endpoints return JSON
- Auth endpoints: `POST /api/v1/auth/login` → `{ token: string }`
- Protected routes require `Authorization: Bearer <token>` header
- Image metadata shape:

```json
{
  "id": "uuid",
  "title": "string",
  "category": "landscape-nature | street-urban",
  "description": "string",
  "shootDate": "ISO 8601 date",
  "visible": true,
  "order": 0,
  "thumbnailUrl": "string",
  "displayUrl": "string",
  "createdAt": "ISO 8601 datetime"
}
```

- Upload endpoint: `POST /api/v1/admin/photos` (multipart/form-data)
  - Backend resizes on upload: stores `thumbnail` (400 px wide) + `display` (1800 px wide) separately in Blob.
- Error responses: `{ "error": "string", "details": "string | null" }`

---

## Pages & Routes (Frontend)

| Route | Description |
|---|---|
| `/` | Public gallery (masonry, filterable, lightbox) |
| `/about` | Bio, gear/philosophy, portrait |
| `/contact` | Email address display only |
| `/admin` | JWT-protected admin panel |
| `/admin/login` | Login form |

---

## Phased Roadmap

| Phase | Scope | Status |
|---|---|---|
| 1 | Project skeleton, scaffolding, CLAUDE.md, .gitignore | ✅ Done |
| 2 | Public gallery — masonry, filtering, lightbox, lazy loading | ✅ Done |
| 3 | Admin panel — upload, metadata, resize, reorder, delete | ✅ Done |
| 4 | Contact & About pages, site-wide nav/footer | ✅ Done |
| 5 | WebP on upload, cache headers, SEO, Render + Vercel deployment | ✅ Done |

---

## Deployment

One command:

```bash
docker compose up --build
```

- Frontend: http://localhost:3000
- Backend API: http://localhost:5000
- Data (SQLite + photos) persists in `./data/` on the host

### First-time setup

1. Copy `.env.example` to `.env` and fill in the two secrets:
   - `Jwt__Secret` — any random string ≥ 32 chars
   - `Admin__PasswordHash` — bcrypt hash (cost 11) of your admin password; generate at bcrypt-generator.com

2. Run `docker compose up --build`

### Self-hosting on a server

Same steps, but on the server set `Cors__AllowedOrigins` in the backend environment to your domain, and update `NUXT_PUBLIC_API_BASE_URL` in the frontend environment to the backend's public URL.

### Image pipeline

- Upload → `ImageService` → WebP 85% quality → 400 px thumbnail + 1800 px display
- Both dev and prod: saved to `./data/uploads/` (or `/data/uploads/` inside the container), served by the static files middleware

### Caching strategy

| Layer | Cache-Control |
|---|---|
| Image files (`/uploads/*`) | `public, max-age=31536000, immutable` |
| `GET /api/v1/photos` | `public, max-age=60, stale-while-revalidate=300` |
| Admin routes | No cache (auth-gated) |

---

## Design Tokens (Dark Theme)

Defined in `frontend/assets/css/tokens.css` and consumed via Tailwind's `theme.extend`:

```
--color-bg:        #0a0a0a   near-black canvas
--color-surface:   #141414   card / panel backgrounds
--color-border:    #232323   subtle dividers
--color-text:      #e8e8e8   primary text
--color-muted:     #6b6b6b   secondary / metadata text
--color-accent:    #6B8F71   muted sage green (chosen Phase 2)
--color-accent-dim: #4a6450  darker accent for hover states
```

---

## Image Categories (Slug → Display)

```
landscape-nature  →  Landscape / Nature
street-urban      →  Street / Urban
```
