# Architecture Diagram

High-level system architecture for SentinelAML, derived from the current solution structure and planned components.

## System Context

```mermaid
flowchart TB
    subgraph Clients["Client Layer (Planned)"]
        WebUI["Investigator Dashboard<br/>(Planned)"]
        ExtSys["Core Banking / TMS<br/>(Planned Integrations)"]
    end

    subgraph API["API Layer — SentinelAML.API"]
        Controllers["REST Controllers<br/>Customer · Account · Health"]
        Middleware["ExceptionHandlingMiddleware"]
        Swagger["Swagger / OpenAPI"]
        HealthChecks["Health Checks<br/>/api/health · /health/database"]
    end

    subgraph Auth["Authentication (Roadmap)"]
        JWT["JWT Bearer Auth<br/>Role-based access<br/>(Not yet implemented)"]
    end

    subgraph Application["Application Layer — SentinelAML.Application"]
        MediatR["MediatR Pipeline"]
        CQRS["CQRS Handlers<br/>Commands · Queries"]
        Validation["FluentValidation<br/>ValidationBehavior"]
        Result["Result&lt;T&gt; Pattern"]
    end

    subgraph Domain["Domain Layer — SentinelAML.Domain"]
        Entities["Customer · Account · Transaction"]
        Rules["Business Invariants<br/>Balance · Status · RiskScore"]
        Enums["AccountStatus · TransactionStatus"]
    end

    subgraph Persistence["Persistence — SentinelAML.Persistence"]
        DbContext["ApplicationDbContext"]
        Repo["Generic Repository&lt;T&gt;"]
        EFConfig["EF Entity Configurations"]
    end

    subgraph Infrastructure["Infrastructure — SentinelAML.Infrastructure"]
        Placeholder["External Service Adapters<br/>(Placeholder today)"]
        FutureAI["Future: Semantic Kernel<br/>Ollama · RAG · Vector Store"]
    end

    subgraph Data["Data Layer"]
        PostgreSQL[("PostgreSQL<br/>SentinelAMLDb")]
    end

    WebUI --> JWT
    ExtSys --> JWT
    JWT --> Controllers
    Controllers --> Middleware
    Controllers --> MediatR
    Swagger --> Controllers
    HealthChecks --> DbContext

    MediatR --> Validation
    Validation --> CQRS
    CQRS --> Result
    CQRS --> Repo
    CQRS --> Entities

    Repo --> DbContext
    DbContext --> EFConfig
    EFConfig --> Entities
    DbContext --> PostgreSQL

    CQRS -.-> Placeholder
    Placeholder -.-> FutureAI
    FutureAI -.-> PostgreSQL

    Entities --> Rules
    Entities --> Enums
```

## Request Flow (Command Example)

```mermaid
sequenceDiagram
    participant Client
    participant Controller as AccountController
    participant MediatR
    participant Validator as FluentValidation
    participant Handler as CreateAccountCommandHandler
    participant Repo as IRepository
    participant UoW as IUnitOfWork
    participant DB as PostgreSQL

    Client->>Controller: POST /api/accounts
    Controller->>MediatR: CreateAccountCommand
    MediatR->>Validator: ValidationBehavior
    alt Validation fails
        Validator-->>Controller: ValidationException → 400
    end
    Validator->>Handler: Handle command
    Handler->>Repo: GetByIdAsync(Customer)
    Handler->>Repo: FindAsync(AccountNumber)
    Handler->>Repo: AddAsync(Account)
    Handler->>UoW: SaveChangesAsync()
    UoW->>DB: INSERT accounts
    Handler-->>Controller: Result&lt;Guid&gt;
    Controller-->>Client: 201 Created
```

## Layer Dependency Rules

```mermaid
flowchart LR
    API["SentinelAML.API"] --> App["SentinelAML.Application"]
    API --> Pers["SentinelAML.Persistence"]
    API --> Infra["SentinelAML.Infrastructure"]
    App --> Domain["SentinelAML.Domain"]
    Pers --> App
    Pers --> Domain
    Infra --> App

    style Domain fill:#e8f5e9
    style App fill:#e3f2fd
    style API fill:#fff3e0
```

The Domain layer has **zero dependencies** on frameworks. All EF Core and HTTP concerns remain in outer layers.
