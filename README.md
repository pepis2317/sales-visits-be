# Sales Visits (Backend)
Backend for Sales Visits, a sales rep monitoring tool i built for CV. Mitra Kaltim Motor
which is actively being used by 5 sales reps for their daily activities


### Related Repos

| Part | Repo |
|---|---|
| Frontend | [sales-visits-fe](https://github.com/pepis2317/sales-visits-fe) |

## What it does
Sales Visits streamlines planning, monitoring, and reporting of day-to-day sales rep activities. Sales reps submit their daily visit plans, then clock in on-site directly from the app. Locationis verified automatically, so visits are only recorded when reps are physically present at the customer's location. Should there be a mismatch, the system flags it, unless the rep is intentionally updating the customer's registered address.

Managers get a clean reporting dashboard showing visit performance per rep, aligned to the company's sales process.

## Features
- **Geospatial Visit Verification** - detects whether a rep is physically at the customer's location at clock-in, detects odd visits and flags them.
- **Smart Customer Proximity** - displays nearby customers. Improving UX and helps reps optimize their day.
- **Visit Performance Dashboard** - real-time performance reporting. per-rep and per period.

## Tech Stack
| Layer | Technology |
|---|---|
| Frontend | Next.js, TypeScript|
| Backend | .NET Core |
| Database | PostgreSQL, PostGIS |
| Auth | - |

### Installation
```bash
git clone https://github.com/pepis2317/sales-visits-be.git
cd sales-visits-be
dotnet restore
#Config
cp appsettings.example.json appsettings.json
dotnet run
```
## What I'd Improve
- [ ] Introduce caching
- [ ] Introduce Excel exports for visit reporting
- [ ] Keep track of sales performance with summary (WIP)
- [ ] Keep track of account receivables (WIP)
