# Risk Assessment — AppSec Homelab

A lightweight risk assessment of the homelab environment, structured around NIST CSF 2.0's five functions. Scope: the mini PC, the deliberately vulnerable application, the CI/CD pipeline, and the GitHub repository housing it all.

## Scope and Assets

| Asset | Description |
|---|---|
| Mini PC (Linux) | Physical host running the app in Docker, on the home network |
| Vulnerable app | ASP.NET Core 8 API + React frontend, deliberately seeded with known vulnerability classes for training purposes |
| CI/CD pipeline | GitHub Actions running Semgrep (SAST), gitleaks (secrets), and OWASP ZAP (DAST) |
| GitHub repository | Source code, documentation, and CI/CD configuration |
| Home network | Local network the mini PC and my Mac operate on |

## Threats and Risk Ratings

| Threat | Likelihood | Impact | Risk | Notes |
|---|---|---|---|---|
| Unauthorized SSH access to the mini PC | Low | High | Medium | Mitigated by key-only auth, ufw, fail2ban |
| Accidental exposure of the vulnerable app beyond the local network | Medium | High | High | App is intentionally vulnerable; exposure to the internet would be a real incident, not a training exercise |
| Secrets committed to the public GitHub repo | Low | High | Medium | Mitigated by gitleaks in CI, but relies on the scan actually catching it before merge |
| Compromise of the mini PC via an unpatched OS/package vulnerability | Medium | Medium | Medium | No formal patch cadence currently in place |
| Weak Wi-Fi security exposing the mini PC to the local network | Low | Medium | Low | Standard home Wi-Fi (WPA2/3), no additional segmentation |
| CI/CD pipeline compromise (e.g. malicious dependency, poisoned Action) | Low | Low | Low | Actions are SHA-pinned rather than tag-pinned, closing the main tag-hijack vector; Dependabot keeps the pins updated via reviewed PRs |

## Existing Controls, Mapped to NIST CSF 2.0

**Identify**
- Asset list above (this document itself is the first formal identification exercise for the environment)
- The README's "Seeded vulnerabilities" table documents every known, deliberately-seeded weakness in the app, with status and remediation evidence for each

**Protect**
- SSH key-only authentication on the mini PC, password auth disabled
- ufw firewall, default-deny with explicit allow rules (OpenSSH, and the app's port scoped to the local subnet only, not the internet)
- fail2ban blocking repeated failed login attempts
- Docker container isolation between the frontend, backend, and host
- Security headers on the frontend (CSP, X-Frame-Options, X-Content-Type-Options, Cross-Origin isolation headers) reducing browser-side attack surface
- Parameterized queries and hashed password storage in the app's actual authentication and data-access code (post-remediation)

**Detect**
- Semgrep (SAST) running on every push, flagging risky code patterns before merge
- gitleaks scanning for committed secrets
- OWASP ZAP (DAST) baseline scans identifying missing security controls at the HTTP level

**Respond**
- CI/CD pipeline configured to block the build on high-severity findings (fail-fast rather than merge-then-fix)
- Manual remediation process demonstrated: vulnerability found → understood → fixed in code → re-tested → confirmed resolved (see the SQLi/XSS/password-storage remediation writeups)

**Recover**
- Not yet formally addressed. No backup or disaster-recovery process exists for the mini PC or its data — acceptable for a training environment with no production data, but noted as a gap for completeness.

## Gaps and Recommendations

1. **No formal patch management** — the mini PC has no scheduled `apt update && apt upgrade` cadence. Recommendation: a weekly manual check, or a cron job with notification rather than silent auto-upgrade (to avoid breaking the environment unexpectedly).
2. **No Recover function in practice** — acceptable given the environment holds no production or sensitive data, but worth noting explicitly rather than silently omitting.
3. **GitHub Actions supply-chain exposure — largely mitigated.** All Actions in the pipeline are already pinned to commit SHAs (not floating tags), with Dependabot's `github-actions` ecosystem keeping those pins current via reviewed PRs rather than silent drift. Residual risk is limited to a compromise landing in an Action's source before a pin is taken, which is a much narrower window than an unpinned tag.
4. **No network segmentation** — the mini PC shares the flat home network with all other devices. Acceptable for a personal lab; would be flagged as a finding in a real environment with multiple untrusted or IoT devices present.

## Why this document exists

This assessment demonstrates applying a recognized framework (NIST CSF) to a real, hands-on technical environment rather than treating governance and technical security as separate disciplines. The controls listed above aren't hypothetical, they're the ones actually built and verified through the homelab project, mapped after the fact to a standard framework structure.
