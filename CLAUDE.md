# Vital Photography — CLAUDE.md

This file documents the project structure, how to run each service, environment variables, API conventions, and deployment notes for Claude Code and contributors.

---

## Project Overview

Personal photography portfolio ("Vital Photography") — a dark, moody, masonry-grid gallery site with a JWT-protected admin panel. The owner is a semi-professional hobbyist photographer.

**Genres / filter categories:** Landscape / Nature · Street / Urban

---

## Monorepo Structure

```
/frontend     Nuxt 3 + Vue 3 + TypeScript + Tailwind CSS
/backend      ASP.NET Core 8 Web API (C#)
/infra        Azure config / deployment scripts
CLAUDE.md     This file
.gitignore
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Nuxt 3, Vue 3, TypeScript, Tailwind CSS |
| Backend | ASP.NET Core 8 Web API, C# |
| Database | Azure SQL (prod) · SQLite (local dev) |
| Image storage | Azure Blob Storage |
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
NUXT_PUBLIC_API_BASE_URL=http://localhost:5000   # backend API base
NUXT_PUBLIC_BLOB_BASE_URL=                       # Azure Blob CDN URL (prod)
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
| `ConnectionStrings__DefaultConnection` | SQLite path for local dev; Azure SQL connection string in prod |
| `Jwt__Secret` | Secret key for signing JWTs (min 32 chars) |
| `Jwt__Issuer` | JWT issuer (e.g. `vital-photography`) |
| `Jwt__Audience` | JWT audience (e.g. `vital-photography-admin`) |
| `Jwt__ExpiryMinutes` | Token lifetime in minutes (default 60) |
| `AzureBlob__ConnectionString` | Azure Storage connection string (prod only) |
| `AzureBlob__ContainerName` | Blob container name |
| `Admin__PasswordHash` | Bcrypt hash of the single admin password |

Local dev uses SQLite; no Azure credentials needed until Phase 5.

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
| 2 | Public gallery — masonry, filtering, lightbox, lazy loading | Pending |
| 3 | Admin panel — upload, metadata, resize, reorder, delete | Pending |
| 4 | Contact & About pages, site-wide nav/footer | Pending |
| 5 | WebP on upload, CDN headers, SEO, Azure deployment | Pending |

---

## Deployment Notes (Phase 5)

- Frontend: Azure Static Web Apps (or Vercel/Netlify) pointing at `/frontend/.output/public`
- Backend: Azure App Service (Linux container) or Azure Container Apps
- Database: Azure SQL Basic tier
- Images: Azure Blob Storage, served via CDN with `Cache-Control: public, max-age=31536000`
- CI/CD: GitHub Actions workflows live in `.github/workflows/`

---

## Design Tokens (Dark Theme)

Defined in `frontend/assets/css/tokens.css` and consumed via Tailwind's `theme.extend`:

```
--color-bg:        #0a0a0a   near-black canvas
--color-surface:   #141414   card / panel backgrounds
--color-border:    #232323   subtle dividers
--color-text:      #e8e8e8   primary text
--color-muted:     #6b6b6b   secondary / metadata text
--color-accent:    TBD       chosen in Phase 2
```

---

## Image Categories (Slug → Display)

```
landscape-nature  →  Landscape / Nature
street-urban      →  Street / Urban
```
