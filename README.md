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

## Seeded vulnerabilities

| # | Vulnerability | OWASP 2025 Category | Location |
|---|---|---|---|
| 1 | SQL injection (login bypass) | A05:2025 - Injection | `AuthController.cs` — `Login()` |
| 2 | SQL injection (search) | A05:2025 - Injection | `ProductsController.cs` — `Search()` |
| 3 | Reflected XSS | A05:2025 - Injection | `ProductSearch.tsx` |
| 4 | Plaintext password storage | A04:2025 - Cryptographic Failures | `SeedData.cs` / `User` model |

Each is commented in code with `// VULNERABLE: <reason>`.

### Demonstrated exploits

**SQL injection login bypass**
- Username: `administrator'--`
- Password: (anything)
- Result: logs in as administrator without knowing the real password

**Reflected XSS**
- Search query: `<img src=x onerror=alert(1)>`
- Result: JavaScript executes in the browser, popping an alert

*(Screenshots to come)*

## Running locally

**Backend:**
```bash
cd backend/AppSecLab.Api
dotnet run
```
Note the port printed in the console (e.g. `http://localhost:5001`).

**Frontend:**
```bash
cd frontend
npm install
npm run dev
```
Opens at `http://localhost:5173`. Update the fetch URLs in `LoginForm.tsx`/`ProductSearch.tsx` if your backend port differs.

## Roadmap

- [x] Vulnerable app scaffolded (SQLi, XSS, plaintext credentials)
- [ ] Semgrep SAST in GitHub Actions
- [ ] gitleaks secret scanning in GitHub Actions
- [ ] OWASP Dependency-Check / Snyk (SCA)
- [ ] Deploy to a self-hosted target so OWASP ZAP (DAST) can run against it
- [ ] Additional vulnerability categories: Broken Access Control, Authentication Failures

## Disclaimer

This application is intentionally insecure and exists solely for personal security education. It is never deployed publicly and should never be used as a reference for production code.