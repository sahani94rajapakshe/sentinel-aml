# SentinelAML

**AI-Powered Financial Crime Investigation Copilot**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16+-336791)](https://www.postgresql.org/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2B%20CQRS-blue)](docs/system-design.md)
[![License](https://img.shields.io/badge/License-Proprietary-lightgrey)](#)

SentinelAML is a backend platform for financial crime investigations. It provides a production-oriented foundation for AML workflows, customer and account lifecycle management, and future AI-assisted investigation tooling.

> **Current phase:** Core domain model, REST APIs, PostgreSQL persistence, and CQRS application layer are implemented. Transaction processing, case management, fraud scoring, and AI copilot capabilities are on the roadmap.

---

## Problem Statement

Financial institutions and fraud teams face growing pressure to investigate suspicious activity faster, with fewer false positives, and under strict regulatory scrutiny. Today, many teams rely on:

- Fragmented tools across CRM, core banking, case management, and spreadsheets
- Manual correlation of customers, accounts, and transactions
- Slow handoffs between monitoring alerts and investigator workflows
- Limited ability to generate consistent, audit-ready investigation narratives

These gaps increase investigation cycle time, operational cost, and compliance risk.

---

## Business Value

SentinelAML is designed to reduce time-to-investigation and improve analyst productivity by:

| Capability | Business impact |
|------------|-----------------|
| Unified customer & account data model | Single source of truth for investigator context |
| Domain-enforced financial invariants | Fewer data integrity issues during investigations |
| CQRS + Clean Architecture | Faster feature delivery as AML modules grow |
| PostgreSQL + EF Core | Reliable transactional storage with audit-friendly timestamps |
| API-first design | Enables analyst dashboards, integrations, and future AI services |
| Planned AI copilot layer | Accelerates report drafting and compliance research |

---

## Key Features

### Implemented (v0.1)

- **Customer management** — Create and retrieve customers with unique email enforcement
- **Account management** — Open accounts, query by customer, close accounts with zero-balance rule
- **Rich domain model** — `Customer`, `Account`, and `Transaction` entities with business invariants
- **CQRS application layer** — MediatR commands/queries with FluentValidation pipeline
- **PostgreSQL persistence** — EF Core with snake_case columns, unique indexes, and health checks
- **REST API + Swagger** — OpenAPI documentation in Development
- **Global exception handling** — Structured validation error responses

### Planned (Roadmap)

- Transaction processing and monitoring feeds
- Fraud risk scoring engine
- Investigation case management
- Relationship graph analysis
- Timeline reconstruction
- JWT authentication and role-based access control
- AI investigation copilot (Semantic Kernel + RAG)
- Compliance knowledge assistant

---

## Architecture Overview

SentinelAML follows **Clean Architecture** with strict dependency direction: API → Application → Domain. Infrastructure and Persistence implement application abstractions without leaking into the domain.

```
┌─────────────────────────────────────────────────────────────┐
│  API Layer (SentinelAML.API)                                │
│  Controllers · Middleware · Swagger · Health Checks         │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│  Application Layer (SentinelAML.Application)                │
│  CQRS · MediatR · Validators · DTOs · Result<T>             │
└──────────────────────────┬──────────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────────┐
│  Domain Layer (SentinelAML.Domain)                          │
│  Entities · Enums · Business Rules                          │
└──────────────────────────▲──────────────────────────────────┘
                           │
┌──────────────────────────┴──────────────────────────────────┐
│  Persistence (SentinelAML.Persistence)                      │
│  EF Core · Repositories · Entity Configurations              │
│  Infrastructure (SentinelAML.Infrastructure) — external svcs  │
└──────────────────────────┬──────────────────────────────────┘
                           │
                    ┌──────▼──────┐
                    │ PostgreSQL  │
                    └─────────────┘
```

See [System Design](docs/system-design.md) and [Architecture Diagram](docs/architecture-diagram.md) for full details.

---

## Technology Stack

| Category | Technology |
|----------|------------|
| Runtime | .NET 9 / C# 13 |
| API | ASP.NET Core Web API |
| Application patterns | CQRS, MediatR, FluentValidation |
| ORM | Entity Framework Core 9 |
| Database | PostgreSQL (Npgsql provider) |
| API docs | Swashbuckle (Swagger UI) |
| Testing | xUnit, WebApplicationFactory |
| Planned auth | JWT Bearer (roadmap) |
| Planned AI | Semantic Kernel, Ollama, Llama 3, RAG |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 14+](https://www.postgresql.org/download/) running locally or remotely
- Optional: [dotnet-ef](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) global tool

### 1. Clone the repository

```bash
git clone https://github.com/your-org/SentinelAML.git
cd SentinelAML
```

### 2. Create the database

```sql
CREATE DATABASE "SentinelAMLDb";
```

### 3. Configure the connection string

The default connection string in `appsettings.json` omits the password. Set credentials via user secrets (recommended for local dev):

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5432;Database=SentinelAMLDb;Username=postgres;Password=YOUR_PASSWORD" \
  --project src/SentinelAML.API
```

### 4. Apply the database schema

Migrations are generated locally (excluded from version control). From the repository root:

```bash
dotnet ef migrations add InitialCreate \
  --project src/SentinelAML.Persistence \
  --startup-project src/SentinelAML.API

dotnet ef database update \
  --project src/SentinelAML.Persistence \
  --startup-project src/SentinelAML.API
```

### 5. Run the API

```bash
dotnet run --project src/SentinelAML.API
```

| Endpoint | URL |
|----------|-----|
| HTTP | http://localhost:5077 |
| HTTPS | https://localhost:7287 |
| Swagger UI | http://localhost:5077/swagger |
| Health | http://localhost:5077/api/health |
| DB Health | http://localhost:5077/health/database |

### 6. Run tests

```bash
dotnet test
```

> Integration tests expect PostgreSQL at `localhost:5432` with database `SentinelAMLDb_Test`.

---

## Folder Structure

```
SentinelAML/
├── docs/                              # Architecture, API, and product documentation
├── src/
│   ├── SentinelAML.API/               # REST controllers, middleware, Program.cs
│   │   ├── Controllers/
│   │   ├── Extensions/
│   │   └── Middleware/
│   ├── SentinelAML.Application/       # CQRS features, validators, behaviors
│   │   ├── Common/
│   │   └── Features/
│   │       ├── Accounts/
│   │       └── Customers/
│   ├── SentinelAML.Domain/            # Entities, enums, domain rules
│   │   ├── Common/
│   │   ├── Entities/
│   │   └── Enums/
│   ├── SentinelAML.Infrastructure/    # External services (AI, messaging — planned)
│   └── SentinelAML.Persistence/       # EF Core DbContext, repositories, configs
│       └── Data/
│           ├── Configurations/
│           └── Repositories/
├── tests/
│   ├── SentinelAML.UnitTests/
│   └── SentinelAML.IntegrationTests/
├── Directory.Build.props
├── SentinelAML.sln
└── README.md
```

---

## Sample API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/health` | Service liveness |
| `GET` | `/health/database` | PostgreSQL connectivity |
| `POST` | `/api/customers` | Create customer |
| `GET` | `/api/customers` | List all customers |
| `GET` | `/api/customers/{id}` | Get customer by ID |
| `POST` | `/api/accounts` | Create account for customer |
| `GET` | `/api/accounts/{id}` | Get account by ID |
| `GET` | `/api/customers/{customerId}/accounts` | List customer accounts |
| `PUT` | `/api/accounts/{id}/close` | Close account (zero balance required) |

Full specification: [docs/api-specification.md](docs/api-specification.md)

**Example — create customer:**

```bash
curl -X POST http://localhost:5077/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Jane",
    "lastName": "Investigator",
    "email": "jane.investigator@example.com",
    "phoneNumber": "+15551234567"
  }'
```

---

## Future AI Roadmap

| Phase | Capability | Integration point |
|-------|------------|-------------------|
| 1 | Transaction ingestion + risk scoring | Application commands + Infrastructure services |
| 2 | Investigation case workflows | New domain aggregates + API modules |
| 3 | Relationship graph analysis | Read models + graph store (planned) |
| 4 | AI investigation copilot | Semantic Kernel plugins over case/transaction context |
| 5 | Compliance RAG assistant | Vector store in Infrastructure layer |

The `SentinelAML.Infrastructure` project is reserved for AI providers, background jobs, and external integrations so the Application layer remains testable.

Details: [docs/system-design.md#future-ai-integration-strategy](docs/system-design.md)

---

## Screenshots

> Placeholder section — add assets to `docs/images/` before publishing.

| Screenshot | Description | Status |
|------------|-------------|--------|
| `docs/images/swagger-overview.png` | Swagger UI with all endpoints | ☐ Capture |
| `docs/images/create-customer-response.png` | POST `/api/customers` 201 response | ☐ Capture |
| `docs/images/customer-accounts-list.png` | GET customer accounts workflow | ☐ Capture |
| `docs/images/architecture-diagram.png` | Rendered architecture diagram | ☐ Capture |
| `docs/images/database-er-diagram.png` | Entity relationship diagram | ☐ Capture |

Full checklist: [docs/screenshots-needed.md](docs/screenshots-needed.md)

---

## Documentation Index

| Document | Purpose |
|----------|---------|
| [System Design](docs/system-design.md) | Architecture decisions and scalability |
| [Architecture Diagram](docs/architecture-diagram.md) | Mermaid system diagram |
| [Database Design](docs/database-design.md) | Schema, entities, ER diagram |
| [API Specification](docs/api-specification.md) | Endpoint reference |
| [Technical Highlights](docs/technical-highlights.md) | Engineering rationale |
| [Startup Pitch](docs/startup-pitch.md) | Investor-facing narrative |
| [Case Study](docs/case-study.md) | Portfolio / founding engineer narrative |
| [GitHub About](docs/github-about.md) | Repository description and topics |
| [Screenshots Checklist](docs/screenshots-needed.md) | Visual asset capture list |

---

## Technical Highlights

SentinelAML prioritizes **investigation-grade data integrity** and **modular growth**:

- **CQRS** separates read and write paths so transaction monitoring and case queries can scale independently
- **Clean Architecture** keeps fraud rules in the Domain layer, independent of EF Core or API concerns
- **PostgreSQL** provides ACID guarantees, JSON support for future event payloads, and mature operational tooling
- **Result-based error handling** returns predictable API responses for analyst tooling integrations

Read more: [docs/technical-highlights.md](docs/technical-highlights.md)

---

## Contributing

This repository is under active development. For architecture changes, please align with the patterns in `SentinelAML.Application/Features/` and extend domain invariants before adding persistence mappings.

---

## License

Proprietary — FinGuard AI. All rights reserved.
