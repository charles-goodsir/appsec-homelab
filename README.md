# AppSec Homelab

A deliberately vulnerable full-stack app, built to practice hands-on Application Security — writing, finding, and eventually catching real vulnerabilities in a CI/CD pipeline.

This is a personal training project. It never runs anywhere but locally, and every vulnerability in it is intentional and commented in the code.

## Why this exists

I'm a software engineer (.NET/C#, TypeScript/React) pivoting into Application Security. Rather than only studying vulnerabilities in isolation (PortSwigger labs, OWASP docs), this project puts them in code I wrote myself, in a stack I actually work in day-to-day — so I can practice finding, exploiting, fixing, and eventually automating detection of the same bug classes I'll see in real codebases.

Write-ups for this project also live on my [portfolio's CyberDiary](https://charles-goodsir.github.io/my-portfolio/).

## Stack

- **Backend**: ASP.NET Core 8 Web API, EF Core + SQLite
- **Frontend**: React + TypeScript, Vite
- **Pipeline**: GitHub Actions (Semgrep SAST, gitleaks secret scanning — more planned)
- **Deployment**: Docker Compose (nginx-fronted), for running OWASP ZAP against a self-hosted target

## Seeded vulnerabilities

| # | Vulnerability | OWASP 2025 Category | Location |
|---|---|---|---|
| 1 | SQL injection (login bypass) | A05:2025 - Injection | `AuthController.cs` — `Login()` |
| 2 | SQL injection (search) | A05:2025 - Injection | `ProductsController.cs` — `Search()` |
| 3 | Reflected XSS | A05:2025 - Injection | `ProductSearch.tsx` |
| 4 | Plaintext password storage | A04:2025 - Cryptographic Failures | `SeedData.cs` / `User` model |

> **Known false negative:** the Semgrep pipeline does not flag the SQL injection in `AuthController.cs`. Investigated and confirmed this is because the `csharp-sqli` rule doesn't treat `[FromBody]`-bound request objects as a tainted source, while `ProductsController.cs`'s `[FromQuery]` parameter is correctly recognised. The vulnerability itself is fully exploitable regardless - this is a gap in tool coverage, not in the code. Full investigation: [CyberDiary Entry 5](https://charles-goodsir.github.io/my-portfolio/#cyberdiary).

Each is commented in code with `// VULNERABLE: <reason>`.

### Demonstrated exploits

**SQL injection login bypass**
- Username: `administrator'--`
- Password: (anything)
- Result: logs in as administrator without knowing the real password

**Reflected XSS**
- Search query: `<img src=x onerror=alert(1)>`
- Result: JavaScript executes in the browser, popping an alert
- Note: the CSP added in `frontend/nginx.conf` (`script-src 'self'`) now blocks
  this inline-handler payload — the injection bug in `ProductSearch.tsx` is
  unchanged, CSP is defence-in-depth on top of it.

*(Screenshots to come)*

### Hardening applied

Response headers set in `frontend/nginx.conf` (remediation for the ZAP baseline
scan): `X-Frame-Options: DENY`, `X-Content-Type-Options: nosniff`, a basic
`Content-Security-Policy`, and `server_tokens off` to hide the nginx version.

## Running locally

### With Docker Compose (single entry point)

```bash
docker compose up -d --build
```

Serves the whole app on `http://localhost:8080`. nginx serves the built
frontend and reverse-proxies `/api/*` to the backend container; only the
frontend port is published. This is the layout OWASP ZAP points at.

### Without Docker (dev)

**Backend:**
```bash
cd backend/AppSecLab.Api
dotnet run
```
Note the port printed in the console (e.g. `http://localhost:5001`). If it
differs, update the `/api` proxy target in `frontend/vite.config.ts`.

**Frontend:**
```bash
cd frontend
npm install
npm run dev
```
Opens at `http://localhost:5173`. The frontend calls the API with relative
`/api/...` URLs; Vite proxies those to the backend in dev.

## Roadmap

- [x] Vulnerable app scaffolded (SQLi, XSS, plaintext credentials)
- [ ] Semgrep SAST in GitHub Actions
- [ ] gitleaks secret scanning in GitHub Actions
- [ ] OWASP Dependency-Check / Snyk (SCA)
- [x] Containerise (Docker Compose) for deployment to a self-hosted target
- [ ] Run OWASP ZAP (DAST) against the deployed target
- [ ] Additional vulnerability categories: Broken Access Control, Authentication Failures

## Disclaimer

This application is intentionally insecure and exists solely for personal security education. It is never deployed publicly and should never be used as a reference for production code.