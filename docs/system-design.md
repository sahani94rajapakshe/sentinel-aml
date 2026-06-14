# System Design

SentinelAML system design documentation — derived from the implemented codebase in `src/` and the product roadmap described in project materials.

---

## High-Level Architecture

SentinelAML is a **modular monolith** structured using Clean Architecture. A single deployable ASP.NET Core API hosts all bounded contexts today (Customers, Accounts). Future modules (Transactions, Cases, AI) will extend the same layering without rewriting the core.

### Layer responsibilities

| Layer | Project | Responsibility |
|-------|---------|----------------|
| Presentation | `SentinelAML.API` | HTTP routing, Swagger, middleware, health endpoints |
| Application | `SentinelAML.Application` | Use cases via CQRS; validation; DTO mapping |
| Domain | `SentinelAML.Domain` | Entities, enums, invariants — framework-agnostic |
| Persistence | `SentinelAML.Persistence` | EF Core, repositories, schema configuration |
| Infrastructure | `SentinelAML.Infrastructure` | External systems (AI, email, messaging) — stub today |

Composition root: `Program.cs` registers services via `AddSentinelAMLServices()`, which chains Application, Persistence, and Infrastructure DI extensions.

---

## Design Decisions

### 1. Modular monolith over microservices (Phase 1)

**Decision:** Single API process with Clean Architecture boundaries.

**Rationale:** Early-stage AML platforms benefit from fast iteration and transactional consistency across customer/account data. Bounded contexts are separated by folders and interfaces, enabling future extraction (e.g., transaction monitoring service) without premature operational complexity.

### 2. Generic repository + Unit of Work

**Decision:** `IRepository<T>` and `IUnitOfWork` abstractions over direct DbContext usage in handlers.

**Rationale:** Handlers remain testable with mocks. `ApplicationDbContext` implements `IUnitOfWork` and exposes `SaveChangesAsync`. Reads use `AsNoTracking()` in the repository for query handlers.

**Trade-off:** Generic repositories can become leaky as query complexity grows. Planned mitigation: introduce read-optimized query services or specifications when transaction search and case dashboards require joins and pagination.

### 3. Result pattern instead of exceptions for business failures

**Decision:** Handlers return `Result` / `Result<T>` for expected failures (not found, duplicate email, zero-balance close violation).

**Rationale:** Predictable API behavior for integrations. Controllers map `Result` to HTTP status codes. Unexpected failures and validation errors still use exceptions (`ValidationException` → middleware → 400).

### 4. Domain-generated identifiers

**Decision:** `BaseEntity.Id` is `Guid.NewGuid()` with `ValueGeneratedNever()` in EF.

**Rationale:** IDs are known before persistence, simplifying correlation in distributed event flows (future transaction ingestion) and avoiding database round-trips for ID assignment.

### 5. Snake_case column naming

**Decision:** EF configurations map properties to snake_case columns (`created_at_utc`, `account_number`).

**Rationale:** Aligns with PostgreSQL conventions and simplifies DBA review, reporting queries, and future analytics pipelines.

### 6. Migrations excluded from version control

**Decision:** `.gitignore` excludes `**/Migrations/`.

**Rationale (observed):** Local migration generation during active schema iteration. **Recommendation for production:** commit migrations or maintain a shared bootstrap script before investor/production demos.

---

## Clean Architecture Explanation

Dependency rule: **source code dependencies point inward**.

```
API → Application → Domain
Persistence → Application → Domain
Infrastructure → Application
```

The Domain project (`SentinelAML.Domain.csproj`) references no NuGet packages beyond the SDK. Business rules live in entity methods:

- `Account.Close()` — requires zero balance
- `Account.Debit()` — enforces sufficient funds and active status
- `Transaction` constructor — prevents self-transfers and non-positive amounts
- `Customer.UpdateProfile()` — validates required profile fields

Application handlers orchestrate persistence but do not duplicate domain invariants when entity methods are used (e.g., `CloseAccountCommandHandler` delegates to `account.Close()`).

---

## CQRS Implementation

CQRS is implemented using **MediatR** with feature folders under `SentinelAML.Application/Features/`.

### Commands (writes)

| Command | Handler | Side effects |
|---------|---------|--------------|
| `CreateCustomerCommand` | `CreateCustomerCommandHandler` | Insert customer, unique email check |
| `CreateAccountCommand` | `CreateAccountCommandHandler` | Insert account, unique account number check |
| `CloseAccountCommand` | `CloseAccountCommandHandler` | Update account status via domain method |

### Queries (reads)

| Query | Handler | Notes |
|-------|---------|-------|
| `GetCustomersQuery` | `GetCustomersQueryHandler` | Full list, no pagination yet |
| `GetCustomerByIdQuery` | `GetCustomerByIdQueryHandler` | Single customer |
| `GetAccountByIdQuery` | `GetAccountByIdQueryHandler` | Single account |
| `GetAccountsByCustomerQuery` | `GetAccountsByCustomerQueryHandler` | Accounts by `CustomerId` |

Each request type is a record/class implementing `IRequest<Result<T>>`. Handlers implement `IRequestHandler<TRequest, TResponse>`.

---

## MediatR Usage

Registration in `DependencyInjection.cs`:

```csharp
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
services.AddValidatorsFromAssembly(assembly);
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

### Pipeline behavior

`ValidationBehavior<TRequest, TResponse>` runs all FluentValidation validators before the handler. On failure, it throws `ValidationException`, caught by `ExceptionHandlingMiddleware` and returned as:

```json
{
  "title": "Validation failed.",
  "status": 400,
  "errors": {
    "Email": ["'Email' is not a valid email address."]
  }
}
```

Controllers use `ISender` (MediatR) via `ApiControllerBase.Mediator` — keeping controllers thin.

---

## Security Approach

### Current state (implemented)

| Control | Status |
|---------|--------|
| HTTPS redirection | Enabled (`UseHttpsRedirection`) |
| Global exception handling | `ExceptionHandlingMiddleware` — no stack traces to clients on 500 |
| Input validation | FluentValidation on all commands/queries with validators |
| Database delete behavior | `Restrict` on FK relationships — prevents accidental cascade deletes |
| Health checks | `/health/database` for operational monitoring |
| Authorization middleware | Registered (`UseAuthorization`) |

### Current gap

**JWT authentication is not yet implemented.** No `[Authorize]` attributes, JWT bearer configuration, or identity packages exist in the codebase today. Endpoints are publicly accessible in Development.

### Planned security model (roadmap)

Aligned with the product direction (JWT + REST APIs):

1. **JWT Bearer authentication** — ASP.NET Core `AddAuthentication().AddJwtBearer()`
2. **Role-based authorization** — roles such as `Analyst`, `SeniorAnalyst`, `ComplianceOfficer`, `Admin`
3. **Policy-based access** — e.g., only `SeniorAnalyst+` can close accounts or export case data
4. **Audit logging** — append-only audit table for entity mutations (complementing `CreatedAtUtc` / `UpdatedAtUtc`)
5. **Secrets management** — user secrets locally; Azure Key Vault / AWS Secrets Manager in production

Implementation target: `SentinelAML.Infrastructure` for token validation services; `[Authorize]` on controllers by endpoint sensitivity.

---

## Scalability Considerations

### Horizontal scaling (API)

The API is stateless today. Multiple instances can run behind a load balancer sharing one PostgreSQL instance. Session state is not used.

### Database scaling

- **Read replicas** — CQRS enables routing heavy investigation queries to read replicas while writes hit primary
- **Partitioning** — `transactions` table (planned high volume) can be partitioned by `transaction_date`
- **Connection pooling** — Npgsql pool via EF Core; tune `Max Pool Size` under load

### Future service extraction

Natural bounded context boundaries for later microservices:

| Service | Trigger |
|---------|---------|
| Transaction Ingestion | High-throughput monitoring feed |
| Risk Scoring Engine | CPU-intensive rule/ML evaluation |
| AI Copilot | GPU-backed inference, separate scaling profile |
| Case Management | Workflow state machines with long-lived processes |

Clean Architecture and MediatR handlers make extraction a matter of moving handlers + interfaces to new hosts, not rewriting domain logic.

### Caching (planned)

- Customer profile cache for investigation dashboards
- Redis for rate limiting on public integration endpoints

---

## Performance Considerations

### Implemented optimizations

- `AsNoTracking()` on repository read methods (`GetAllAsync`, `FindAsync`)
- PostgreSQL retry policy: 3 retries, 5s max delay (`EnableRetryOnFailure`)
- EF Core query logging suppressed to Warning in production config

### Known limitations (current codebase)

| Area | Issue | Mitigation path |
|------|-------|-----------------|
| `GetCustomersQuery` | Loads entire customer table | Add pagination + filtering |
| `GetByIdAsync` | Uses `FindAsync` — no eager loading | Add includes when DTOs need related data |
| No response compression | Larger JSON payloads | Enable `AddResponseCompression` |
| Synchronous domain validation | Throws on invalid input | Acceptable for command path; map to Result where needed |

### Investigation workload expectations

Future transaction and case queries will be I/O bound. Index strategy already includes unique indexes on `customers.email` and `accounts.account_number`. Additional indexes planned on `transactions.transaction_date`, `transactions.from_account_id`, and `transactions.risk_score`.

---

## Future AI Integration Strategy

The `Transaction` entity already includes `RiskScore` — a hook for ML/rule-engine output. AI integration will follow this pattern:

### Layer placement

```
SentinelAML.Infrastructure/
├── AI/
│   ├── IInvestigationCopilotService.cs      # Application interface (future)
│   ├── SemanticKernelInvestigationService.cs
│   ├── OllamaChatCompletionAdapter.cs
│   └── RagComplianceKnowledgeService.cs
```

Application handlers depend on **interfaces** defined in `SentinelAML.Application/Common/Interfaces/`, implemented in Infrastructure.

### Integration phases

**Phase A — Risk scoring (deterministic + ML)**

- Transaction command handler calls `IRiskScoringService`
- Score persisted on `Transaction.RiskScore` before `Complete(riskScore)`

**Phase B — Investigation copilot**

- Case query aggregates customer, accounts, transactions into a context document
- Semantic Kernel orchestrates plugins: timeline builder, entity linker, narrative generator
- Output stored as investigation report artifact (new entity — planned)

**Phase C — Compliance RAG**

- Vector embeddings of regulatory corpora (FATF, FinCEN guidance, internal policy)
- Retrieval-augmented generation for analyst Q&A with citation requirements

### Design constraints for AI safety

- AI outputs are **recommendations**, not automated filing decisions
- Human analyst approval gate before SAR/STR submission workflows
- Prompt context scoped to authorized case data only (enforced post-JWT)
- Full prompt/response audit log for regulatory review

---

## Observability (planned)

| Signal | Tool (recommended) |
|--------|-------------------|
| Structured logging | Serilog → OpenTelemetry |
| Metrics | ASP.NET Core meters + Prometheus |
| Traces | OpenTelemetry → Jaeger / Azure Monitor |
| Health | Existing `/health/database` + future dependency checks |

---

## Related Documents

- [Architecture Diagram](architecture-diagram.md)
- [Database Design](database-design.md)
- [API Specification](api-specification.md)
- [Technical Highlights](technical-highlights.md)
