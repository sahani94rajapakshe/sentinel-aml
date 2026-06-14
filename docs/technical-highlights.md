# Technical Highlights

Engineering rationale for SentinelAML — written for senior engineers, architects, and technical investors evaluating platform decisions.

---

## Why CQRS Was Selected

**Command Query Responsibility Segregation** separates operations that change state from operations that read state.

### Fit for AML workloads

| Workload type | CQRS benefit |
|---------------|--------------|
| Transaction ingestion (write-heavy) | Commands validate and persist without read query complexity |
| Investigation dashboards (read-heavy) | Queries optimized independently — pagination, projections, read replicas |
| Risk scoring pipeline | Async command handlers can invoke scoring services without blocking reads |
| Regulatory audit | Clear handler boundaries — each use case is isolated and testable |

### Implementation in SentinelAML

- **MediatR** dispatches `CreateCustomerCommand`, `GetAccountsByCustomerQuery`, etc.
- **Feature folders** (`Features/Customers`, `Features/Accounts`) co-locate handlers, validators, and DTOs
- **`ValidationBehavior`** runs FluentValidation before every handler — cross-cutting validation without controller bloat
- **`Result<T>`** returns expected failures without exceptions — predictable for API consumers and future event publishers

### What we avoided

Full event sourcing is **not** used — operational complexity is unnecessary at current scale. CQRS here means **handler separation**, not an event store. Event sourcing can be added for transaction audit trails when regulatory requirements demand it.

---

## Why PostgreSQL Was Selected

### Requirements from financial crime platforms

- **ACID transactions** — account debits/credits and transaction completion must be atomic
- **Relational integrity** — customers, accounts, and transactions have strict FK relationships
- **Audit-friendly storage** — timestamptz columns, immutable-style RESTRICT deletes
- **Operational maturity** — DBAs, backup tools, replication, and monitoring are industry-standard
- **Extensibility** — JSONB for future rule payloads; pgvector for compliance RAG embeddings

### How SentinelAML uses PostgreSQL

| Feature | Usage |
|---------|-------|
| `numeric(18,2)` | Financial precision for balances and amounts |
| `uuid` PKs | Domain-generated identifiers for correlation |
| Unique indexes | Email and account number deduplication |
| `ON DELETE RESTRICT` | Prevents destructive cascades on financial records |
| Retry policy | `EnableRetryOnFailure` (3 retries, 5s delay) for transient failures |
| Health checks | `/health/database` via `AddDbContextCheck` |

### Alternatives considered

| Database | Why not (for this platform) |
|----------|----------------------------|
| MongoDB | Weak relational integrity for multi-entity financial graphs |
| SQL Server | Strong option for .NET shops; PostgreSQL chosen for cost and OSS ecosystem at scale |
| DynamoDB | Better for extreme scale key-value; investigation queries need joins and ad-hoc filters |

PostgreSQL balances **transactional correctness** with **analytical flexibility** as the platform grows into graph and vector workloads.

---

## Why Clean Architecture Was Selected

### The constraint

AML platforms live for **years**. Rules change. Integrations change. AI providers will change. The domain must outlive every framework version.

### Layer enforcement in SentinelAML

```
SentinelAML.Domain          → zero external dependencies
SentinelAML.Application     → depends on Domain only
SentinelAML.Persistence     → implements Application interfaces
SentinelAML.Infrastructure  → implements Application interfaces (AI, messaging)
SentinelAML.API             → composition root, HTTP adapter
```

### Concrete benefits observed in code

1. **`Account.Close()`** enforces zero balance in the domain — not in a controller or SQL trigger
2. **EF Core configurations** map to domain entities without polluting entities with data annotations
3. **`IRepository<T>` / `IUnitOfWork`** allow handler unit tests without a database
4. **`SentinelAML.Infrastructure`** is an empty stub today — AI services slot in without touching Domain or Application contracts

### Cost

More projects and boilerplate than a single-project API. For a platform targeting investigation cases, fraud engines, and AI copilots, this cost pays back within the first major feature module.

---

## How the System Can Scale

### Phase 1 — Modular monolith (current)

Single deployable API. Stateless HTTP. Shared PostgreSQL. Scale horizontally by adding API instances behind a load balancer.

### Phase 2 — Read/write separation

| Path | Scaling lever |
|------|---------------|
| Commands | Primary DB, optimized write handlers |
| Queries | Read replicas, materialized views, cached DTOs |
| Heavy search | Elasticsearch or PostgreSQL full-text for transaction feeds |

CQRS handlers are already separated — routing reads to replicas requires infrastructure change, not application rewrites.

### Phase 3 — Service extraction

Natural split points based on current bounded contexts:

| Service | Signal to extract |
|---------|-------------------|
| Transaction Ingestion | >10K TPS monitoring feed |
| Risk Engine | CPU-bound ML inference blocking API threads |
| AI Copilot | GPU workloads, different deployment cadence |
| Case Workflow | Long-running state machines |

Domain entities and MediatR handlers move with their interfaces — Clean Architecture boundaries become service boundaries.

### Phase 4 — Data platform

- **Event streaming** (Kafka) for transaction alerts to multiple consumers
- **Graph database** (Neo4j) for relationship analysis — fed from PostgreSQL CDC
- **Vector store** (pgvector or dedicated) for compliance RAG

The current schema and CQRS structure are compatible with this evolution without a ground-up rewrite.

---

## How AI Can Be Integrated

### Principle: AI advises; domain decides

Financial crime platforms cannot delegate regulatory decisions to LLMs. SentinelAML architecturally separates:

| Layer | Role |
|-------|------|
| Domain | Source of truth — balances, statuses, risk scores |
| Application | Orchestrates use cases, calls AI via interfaces |
| Infrastructure | Implements AI providers — Semantic Kernel, Ollama, OpenAI |

### Integration points already in the codebase

| Hook | AI use |
|------|--------|
| `Transaction.RiskScore` | ML/rule engine output at `Complete(riskScore)` |
| `TransactionStatus` lifecycle | AI can flag for review before completion |
| `SentinelAML.Infrastructure/DependencyInjection.cs` | Register `IInvestigationCopilotService` implementations |
| CQRS query handlers | Aggregate case context for prompt construction |

### Planned AI stack (roadmap)

| Component | Purpose |
|-----------|---------|
| Semantic Kernel | Plugin orchestration — timeline, entity link, narrative |
| Ollama + Llama 3 | Local/on-prem inference for data-sensitive banks |
| RAG | Compliance Q&A with citation to policy documents |
| Vector embeddings | Stored in Infrastructure; retrieved at query time |

### Safety controls (planned)

- Role-scoped prompt context (post-JWT)
- Analyst approval gate on AI-generated reports
- Immutable audit log of prompts and responses
- No automated SAR/STR submission without human sign-off

---

## Summary

| Decision | One-line rationale |
|----------|-------------------|
| CQRS | Separates investigation reads from monitoring writes as volume grows |
| PostgreSQL | ACID + relational integrity + extensibility for analytics and vectors |
| Clean Architecture | Domain outlives frameworks; AI and persistence are swappable adapters |
| Modular monolith | Fast iteration now, clean extraction path later |
| Infrastructure AI layer | Copilot is a feature, not a foundation — business rules stay in Domain |

---

## Related Documents

- [System Design](system-design.md)
- [Architecture Diagram](architecture-diagram.md)
- [Startup Pitch](startup-pitch.md)
