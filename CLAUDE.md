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
| 2 | Public gallery — masonry, filtering, lightbox, lazy loading | ✅ Done |
| 3 | Admin panel — upload, metadata, resize, reorder, delete | ✅ Done |
| 4 | Contact & About pages, site-wide nav/footer | ✅ Done |
| 5 | WebP on upload, CDN headers, SEO, Azure deployment | ✅ Done |

---

## Deployment (free stack)

| Layer | Service | Cost |
|---|---|---|
| Frontend | Vercel | Free |
| Backend API | Fly.io | Free tier (shared VM, 256 MB) |
| Database | SQLite on Fly volume | Free (1 GB volume) |
| Image storage + CDN | Cloudflare R2 | Free (10 GB, no egress fees) |

### 1 — Cloudflare R2 (image storage)

1. Create a free account at cloudflare.com
2. R2 → Create bucket → name it `photos` → enable public access
3. R2 → Manage API tokens → Create token (Object Read & Write on `photos` bucket)
4. Note: **Account ID**, **Access Key ID**, **Secret Access Key**
5. R2 → `photos` bucket → Settings → copy the **Public bucket URL** (looks like `https://pub-xxx.r2.dev`)

### 2 — Fly.io (backend + database)

```bash
# Install CLI (Windows)
winget install flyio.flyctl

# Login
fly auth login

# Launch app (run once from /backend — accepts fly.toml, skip deploy for now)
cd backend
fly launch --no-deploy

# Create persistent volume for SQLite
fly volumes create vital_photography_data --size 1 --region mad

# Set all secrets (replace placeholder values)
fly secrets set \
  Jwt__Secret="a-random-string-at-least-32-chars-long" \
  Admin__PasswordHash="<bcrypt hash of your chosen admin password>" \
  CloudflareR2__AccountId="<from step 1>" \
  CloudflareR2__AccessKeyId="<from step 1>" \
  CloudflareR2__SecretAccessKey="<from step 1>" \
  CloudflareR2__BucketName="photos" \
  CloudflareR2__PublicUrl="<public bucket URL from step 1>" \
  Cors__AllowedOrigins="https://your-app.vercel.app"

# First deploy
fly deploy
```

To generate the `Admin__PasswordHash`, paste your chosen password into any online bcrypt generator (e.g. bcrypt-generator.com) with cost 11.

### 3 — Vercel (frontend)

1. Push this repo to GitHub
2. vercel.com → New Project → Import your repo → set **Root Directory** to `frontend`
3. Add environment variables in Vercel dashboard:
   - `NUXT_PUBLIC_API_BASE_URL` → your Fly.io API URL (e.g. `https://vital-photography-api.fly.dev`)
   - `NUXT_PUBLIC_BLOB_BASE_URL` → your R2 public bucket URL
   - `NUXT_PUBLIC_SITE_URL` → your Vercel deployment URL
4. Deploy — Vercel auto-deploys on every push to `main` from here on

### 4 — GitHub Secret for CI

Add one secret to your GitHub repo (Settings → Secrets → Actions):

| Secret | Value |
|---|---|
| `FLY_API_TOKEN` | Run `fly tokens create deploy` and paste the output |

After this, every push to `main` that touches `backend/` triggers an automatic Fly.io redeploy via `.github/workflows/backend.yml`.

### Image pipeline

- Upload → `ImageService` → WebP 85% quality → 400 px thumbnail + 1800 px display
- Dev: saved to `wwwroot/uploads/` served by static files middleware with immutable headers
- Prod: uploaded to Cloudflare R2 → served via R2 public URL with `Cache-Control: immutable`

### Caching strategy

| Layer | Cache-Control |
|---|---|
| Image files (R2/CDN) | `public, max-age=31536000, immutable` |
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
