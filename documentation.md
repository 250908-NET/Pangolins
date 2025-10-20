here’s a compact, copy-pasteable **cheat sheet** of everything we did to get API + Next.js running locally, in Docker, and wired together. it’s written so a teammate can reproduce from a clean clone, and so you can show the flow to your trainer.

---

# Pangolivia – dev & docker cheat sheet

## 0) prerequisites

* .NET 9 SDK
* Node 20+ (for Next.js)
* Docker Desktop
* (optional) Azure CLI if you’ll try cloud later

## 1) clone & branch (don’t touch `main`)

```bash
git clone https://github.com/250908-NET/Pangolins.git
cd Pangolins
git checkout -b experiment2
```

## 2) API: configure connection string securely

We use **User Secrets** in dev (no secrets in repo).

```bash
cd Pangolivia.API

# ensure user-secrets is initialized
dotnet user-secrets init

# set a dev connection string (pick one that works for you)
# Example – local SQL Server in Docker:
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
"Server=localhost,1433;Database=Pangolivia;User Id=sa;Password=Password123;TrustServerCertificate=True;"
```

> can swap in the team’s Azure SQL connection later; keep it in Secrets, not in files.

## 3) database: create & update

```bash
# inside Pangolivia.API/
dotnet build
dotnet ef migrations list              # should show migrations, or “None” on first run
# if none exist (already added by repo), you can create once:
# dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 4) run the API locally

```bash
dotnet run
# API listens on http://localhost:5156 (per launchSettings.json)
# Swagger: http://localhost:5156/swagger
```

## 5) Next.js → point to API

In **Pangolivia.Next**, we use `NEXT_PUBLIC_API_BASE` so FE knows where to call:

* During local dev without Docker, set:

```bash
cd ../Pangolivia.Next
echo "NEXT_PUBLIC_API_BASE=http://localhost:5156" > .env.local
```

* Start Next.js:

```bash
npm install
npm run dev
# http://localhost:3000
```

## 6) Dockerize both (API + Web) and run together

### 6.1 API Dockerfile (already in `Pangolivia.API/Dockerfile`)

* Multi-stage build with `dotnet publish`
* App listens on **8080** in container.

### 6.2 Web (Next.js) Dockerfile (already in `Pangolivia.Next/Dockerfile`)

* Builds in **standalone** mode.
* Uses `NEXT_PUBLIC_API_BASE` (defaults to `http://api:8080` when running with compose).

### 6.3 root `docker-compose.yml` (at repo root)

* Defines two services:

  * `api` → builds `./Pangolivia.API`, exposes **8080**
  * `web` → builds `./Pangolivia.Next`, exposes **3000**, **depends_on: api**
* FE talks to API via internal name `http://api:8080`.

Run:

```bash
cd ..
docker compose up --build
# web: http://localhost:3000  | api (swagger): http://localhost:8080/swagger
```

Quick checks:

```bash
curl -i http://localhost:8080/swagger
curl -i http://localhost:3000/api/quiz            # FE endpoint that proxies/calls the API
curl -i "http://localhost:3000/api/quiz/external?amount=3"
```

## 7) logging & exception handling

* We added **Serilog** console/file logging and a **global exception middleware** in API (development friendly).
* When running in Docker/compose, logs appear in container logs:

```bash
docker compose logs api -f
docker compose logs web -f
```

## 8) unit test project (optional)

We created `Pangolivia.Tests` at repo root and referenced the API project.
Common operations:

```bash
dotnet clean
dotnet build
dotnet test --collect:"XPlat Code Coverage"
```

> If you ever see duplicate assembly attribute errors, delete `bin/obj` and rebuild.

## 9) git hygiene (work only in your feature branch)

When you reach a stable point:

```bash
git add -A
git commit -m "Docker/FE integration: Next.js standalone Dockerfile + root docker-compose; FE proxied to api:8080 via NEXT_PUBLIC_API_BASE; local compose runs web+api"
git push origin experiment2
```

---

## (optional) Azure notes – what we tried

* We built and pushed `polyglotarist/pangolivia-api:latest` and configured an **Azure Web App for Containers**.
* App settings needed for .NET API in a container:

  * `WEBSITES_PORT=8080`
  * `ASPNETCORE_URLS=http://+:8080`
  * `ASPNETCORE_ENVIRONMENT=Development`
  * `ConnectionStrings__DefaultConnection=...` (your Azure SQL)
* On student subscriptions, we hit region/provider policy limits and an **image arch mismatch** once (Linux expects `linux/amd64`; rebuild with `--platform linux/amd64` if needed).
* If the container starts but returns **Application Error**:

  1. enable log stream in the Web App portal (or `az webapp log tail`),
  2. confirm the app is listening on **8080**,
  3. ensure the DB connection string is present as an app setting (not in code),
  4. add a simple `/healthz` endpoint in the API to satisfy platform health checks.

> Cloud deploy is optional for dev. Our **local compose** flow is the supported dev path.

---

## handy snippets

**Switch API to container port 8080 locally (one-off run):**

```bash
docker run -it -p 8080:8080 \
  -e ASPNETCORE_URLS=http://+:8080 \
  -e "ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=Pangolivia;User Id=sa;Password=Password123;TrustServerCertificate=True;" \
  pangolivia-api:latest
```

**Reset EF migration lock (rare):**

```bash
dotnet ef database update
```

**Clear build artifacts if VS Code shows weird duplicate attribute errors:**

```bash
find . -name bin -o -name obj | xargs rm -rf
dotnet clean && dotnet build
```

---

### what’s next (tiny, safe improvements)

* Add `GET /healthz` in API (returns 200) for health checks.
* Add CORS policy in API to allow the web origin when not on the same docker network.
* Add `.env.example` in Next.js with `NEXT_PUBLIC_API_BASE=`.

