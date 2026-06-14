# Case Study

**SentinelAML — Building an AI-Ready Financial Crime Investigation Platform**

*Portfolio case study for founding engineer / staff engineer evaluation.*

---

## Business Problem

A financial crime investigation platform must help analysts answer three questions quickly:

1. **Who** is involved? (customers, accounts, counterparties)
2. **What** happened? (transactions, timelines, amounts)
3. **Why** is it suspicious? (risk scores, patterns, regulatory context)

Legacy approaches fail because customer data, account states, and transaction feeds live in disconnected systems. Analysts lose hours to manual correlation before substantive investigation begins. Regulators increasingly expect faster, better-documented decisions.

The business goal: **reduce investigation cycle time and compliance risk** with a platform that starts as a reliable data foundation and evolves into an AI-assisted copilot.

---

## Technical Challenges

### 1. Financial data integrity under investigation pressure

Investigation tools must never show stale or inconsistent balances. Account close operations must enforce zero balance. Transfers must prevent self-referencing transactions.

**Challenge:** Encode these rules once, enforce everywhere — API, batch jobs, future AI agents.

### 2. Platform longevity vs. delivery speed

AML products accrue features for years (cases, rules, graphs, AI). A monolithic spaghetti codebase becomes unmaintainable.

**Challenge:** Ship a v0.1 quickly while preserving boundaries for transaction monitoring and AI modules.

### 3. Read/write workload asymmetry

Customer profile reads are lightweight. Future transaction monitoring feeds may ingest thousands of events per second while dashboards run complex filtered queries.

**Challenge:** Architecture that separates commands from queries without premature microservice complexity.

### 4. AI integration without compromising trust

Banks will not accept black-box AI decisions on suspicious activity. AI must augment analysts, not replace audit trails.

**Challenge:** Reserve a swappable AI layer that never owns financial truth.

### 5. Operational readiness from day one

Investigation platforms cannot go dark silently. Database connectivity must be observable.

**Challenge:** Health checks, structured errors, and connection verification without over-engineering observability stacks.

---

## Architecture Decisions

| Decision | Alternative considered | Rationale |
|----------|------------------------|-----------|
| Clean Architecture (5 projects) | Single Web API project | Domain longevity; AI/persistence swap |
| CQRS + MediatR | Direct service classes | Handler isolation; pipeline behaviors |
| Generic `IRepository<T>` | Direct DbContext in handlers | Testability; consistent data access |
| `Result<T>` pattern | Exceptions for all failures | Predictable API contracts |
| PostgreSQL | SQL Server / MongoDB | ACID + relational model + ecosystem |
| Domain-generated UUIDs | DB identity columns | Pre-persist ID correlation |
| snake_case columns | PascalCase in DB | PostgreSQL convention; reporting friendly |
| Modular monolith | Microservices day one | Faster iteration; extract later |
| Infrastructure stub | AI in Application layer | Keeps application testable and provider-agnostic |

---

## Implementation Approach

### Domain-first modeling

Entities were modeled with behavior, not anemic data bags:

- `Customer` — profile validation, account ownership
- `Account` — credit/debit, suspend/activate/close with invariants
- `Transaction` — transfer validation, risk score on completion, pending/failed lifecycle

Enums (`AccountStatus`, `TransactionStatus`) map to integer columns for stable storage.

### Application layer — feature folders

Each use case is a self-contained vertical slice:

```
Features/
├── Customers/
│   ├── Commands/CreateCustomer/
│   └── Queries/GetCustomerById/
└── Accounts/
    ├── Commands/CreateAccount/
    ├── Commands/CloseAccount/
    └── Queries/GetAccountsByCustomer/
```

FluentValidation rules mirror EF column constraints (max lengths, required fields). `ValidationBehavior` enforces them pipeline-wide.

### Persistence — EF Core configurations

- Table names: `customers`, `accounts`, `transactions`
- Unique indexes on `email`, `account_number`
- `numeric(18,2)` for monetary fields
- `ON DELETE RESTRICT` on all FKs
- Field-backed collections for DDD navigation properties

### API layer — thin controllers

Controllers dispatch MediatR requests and map `Result<T>` to HTTP status codes. `ExceptionHandlingMiddleware` handles validation and unexpected errors. Swagger enabled in Development.

### Testing foundation

- **Unit tests:** `Result` pattern verification
- **Integration tests:** `WebApplicationFactory` with health endpoint smoke test
- **Gap acknowledged:** Handler and domain tests planned as modules grow

---

## Results

### Delivered (measurable)

| Outcome | Detail |
|---------|--------|
| **8 REST endpoints** | Customer CRUD-read, account lifecycle, health |
| **3-table schema** | Customer → Account → Transaction model ready for monitoring |
| **Zero framework deps in Domain** | Pure C# business rules |
| **Validation pipeline** | 5 FluentValidation validators across commands/queries |
| **Operational endpoints** | `/api/health`, `/health/database` |
| **Build target** | .NET 9, EF Core 9, PostgreSQL via Npgsql |

### Architectural outcomes (qualitative)

- New AML features (transaction commands, case entities) can be added as new MediatR handlers without modifying existing handlers
- AI services can register in `SentinelAML.Infrastructure` without touching domain entities
- Database schema supports future high-volume transaction ingestion with clear FK graph
- API contracts are documented in OpenAPI and `docs/api-specification.md`

### Business outcomes (projected with roadmap)

| Milestone | Expected impact |
|-----------|-----------------|
| Transaction module | Enables monitoring demo for pilot customers |
| Case management | End-to-end investigator workflow |
| AI copilot | 40–60% reduction in report drafting time (industry benchmark target) |
| JWT + RBAC | Enterprise security gate for bank procurement |

---

## Lessons Learned

### 1. Model transactions before the API

The `Transaction` entity and EF configuration exist before handlers — this validates the schema early but risks drift if API delivery lags. **Lesson:** pair entity modeling with at least one integration test per aggregate.

### 2. Aggregate consistency matters

`Customer.OpenAccount()` exists but `CreateAccountCommandHandler` calls `Account.Create()` directly. **Lesson:** pick one account creation path to avoid DDD confusion as teams grow.

### 3. Commit migrations before investor demos

Migrations are gitignored — reproducible schema setup requires local `dotnet ef` steps. **Lesson:** for external reviewers, committed migrations or Docker Compose bootstrap are essential.

### 4. Auth cannot be a late surprise

`UseAuthorization()` is registered without JWT implementation. **Lesson:** implement authentication before exposing demos beyond localhost — financial platforms face immediate security scrutiny.

### 5. Error mapping by string matching is fragile

Controllers use `errors.Contains("not found")` for 404 mapping. **Lesson:** introduce typed error codes before API consumers depend on message text.

### 6. Test the domain, not just infrastructure

Two unit tests cover `Result`. Domain invariants (`Account.Close` with non-zero balance) are high-value, low-cost tests. **Lesson:** domain tests provide the best ROI for financial platforms.

### 7. Documentation is part of the product

For RegTech and founding engineer evaluation, architecture docs, ER diagrams, and honest roadmap status matter as much as code volume.

---

## What's Next

1. Transaction command handlers wiring `Debit`/`Credit` atomically
2. JWT authentication and role policies
3. Investigation case aggregate
4. Semantic Kernel copilot MVP in Infrastructure layer
5. CI pipeline + Docker Compose for one-command demo

---

## Related Documents

- [Startup Pitch](startup-pitch.md)
- [Technical Highlights](technical-highlights.md)
- [System Design](system-design.md)
- [API Specification](api-specification.md)
