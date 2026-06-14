# Screenshots Checklist

Professional GitHub repositories for fintech/RegTech platforms rely on visual proof — not just README text. Use this checklist to capture every asset that demonstrates SentinelAML to founders, investors, and engineering reviewers.

Save all images to `docs/images/` and reference them from `README.md`.

---

## Priority 1 — Must Have (Repository Launch)

### Swagger / API Documentation

| # | Screenshot | How to capture | Filename |
|---|------------|----------------|----------|
| 1 | Swagger UI overview — all controller groups expanded | Run API in Development → `http://localhost:5077/swagger` | `swagger-overview.png` |
| 2 | Customer endpoints expanded showing schemas | Expand `Customer` tag in Swagger | `swagger-customers.png` |
| 3 | Account endpoints expanded showing schemas | Expand `Account` tag in Swagger | `swagger-accounts.png` |
| 4 | `CreateCustomerDto` schema detail | Click schema in Swagger | `swagger-create-customer-schema.png` |

### API Workflow — Customer Lifecycle

| # | Screenshot | How to capture | Filename |
|---|------------|----------------|----------|
| 5 | POST `/api/customers` — request body in Swagger | Execute with sample data | `api-create-customer-request.png` |
| 6 | POST `/api/customers` — 201 response with ID | After successful execute | `api-create-customer-response.png` |
| 7 | GET `/api/customers` — list response | Execute after creating 2+ customers | `api-list-customers.png` |
| 8 | GET `/api/customers/{id}` — single customer | Use ID from create response | `api-get-customer-by-id.png` |
| 9 | POST `/api/customers` — 400 duplicate email | Create same email twice | `api-duplicate-email-error.png` |
| 10 | POST `/api/customers` — 400 validation error | Submit empty body | `api-validation-error.png` |

### API Workflow — Account Lifecycle

| # | Screenshot | How to capture | Filename |
|---|------------|----------------|----------|
| 11 | POST `/api/accounts` — request with customerId | Create account for existing customer | `api-create-account-request.png` |
| 12 | POST `/api/accounts` — 201 response | Successful create | `api-create-account-response.png` |
| 13 | GET `/api/customers/{id}/accounts` — account list | Query customer accounts | `api-customer-accounts-list.png` |
| 14 | GET `/api/accounts/{id}` — account detail with balance | Single account view | `api-get-account.png` |
| 15 | PUT `/api/accounts/{id}/close` — 400 non-zero balance | Close account with balance > 0 | `api-close-account-blocked.png` |
| 16 | PUT `/api/accounts/{id}/close` — 200 success | Zero balance then close | `api-close-account-success.png` |

### Health & Operations

| # | Screenshot | How to capture | Filename |
|---|------------|----------------|----------|
| 17 | GET `/api/health` — JSON response | Browser or curl | `api-health-check.png` |
| 18 | GET `/health/database` — healthy status | With PostgreSQL running | `api-database-health.png` |
| 19 | Startup log — PostgreSQL connection verified | Console output in Development | `startup-db-verified.png` |

---

## Priority 2 — Architecture & Engineering Credibility

### Diagrams (render Mermaid → PNG)

| # | Screenshot | Source | Filename |
|---|------------|--------|----------|
| 20 | System architecture diagram | [architecture-diagram.md](architecture-diagram.md) | `architecture-diagram.png` |
| 21 | Request flow sequence diagram | [architecture-diagram.md](architecture-diagram.md) | `request-flow-sequence.png` |
| 22 | Database ER diagram | [database-design.md](database-design.md) | `database-er-diagram.png` |
| 23 | Clean Architecture layer diagram | [architecture-diagram.md](architecture-diagram.md) | `clean-architecture-layers.png` |

**Tools:** Mermaid Live Editor, GitHub markdown preview, or VS Code Mermaid extension → export PNG.

### IDE / Code Quality

| # | Screenshot | How to capture | Filename |
|---|------------|----------------|----------|
| 24 | Solution Explorer — project structure | Visual Studio / Rider / VS Code | `ide-solution-structure.png` |
| 25 | Feature folder — Customers CQRS slice | `Application/Features/Customers/` | `ide-customers-feature-folder.png` |
| 26 | Domain entity — Account business methods | `Account.cs` with Close/Debit/Credit visible | `ide-domain-account.png` |
| 27 | EF configuration — unique indexes | `AccountConfiguration.cs` | `ide-ef-configuration.png` |
| 28 | ValidationBehavior pipeline | `ValidationBehavior.cs` | `ide-validation-pipeline.png` |

### Database

| # | Screenshot | How to capture | Filename |
|---|------------|----------------|----------|
| 29 | pgAdmin / DBeaver — `customers` table schema | After migration applied | `db-customers-table.png` |
| 30 | pgAdmin — `accounts` table with FK to customers | Show constraints | `db-accounts-table.png` |
| 31 | pgAdmin — `transactions` table schema | Show risk_score column | `db-transactions-table.png` |
| 32 | Sample data — customer with linked accounts | JOIN query result | `db-sample-investigation-data.png` |

### Testing

| # | Screenshot | How to capture | Filename |
|---|------------|----------------|----------|
| 33 | `dotnet test` — all tests passed | Terminal output | `tests-passing.png` |
| 34 | Unit test project structure | Test explorer | `ide-test-explorer.png` |

---

## Priority 3 — Product Vision (Roadmap / Demo Story)

These screenshots require features not yet implemented — capture as mockups or after building MVP modules.

### Transaction Monitoring (Planned)

| # | Screenshot | When available | Filename |
|---|------------|----------------|----------|
| 35 | POST `/api/transactions` — transfer request | Transaction module shipped | `api-create-transaction.png` |
| 36 | Transaction with risk score in response | Risk engine integrated | `api-transaction-risk-score.png` |
| 37 | GET `/api/transactions` — filtered monitoring feed | Query module shipped | `api-transaction-feed.png` |

### Investigation Cases (Planned)

| # | Screenshot | When available | Filename |
|---|------------|----------------|----------|
| 38 | Case creation workflow | Case module shipped | `api-create-case.png` |
| 39 | Case detail — linked customer + accounts | Case module shipped | `api-case-detail.png` |
| 40 | Case timeline view | Timeline module shipped | `ui-case-timeline.png` |

### Authentication (Planned)

| # | Screenshot | When available | Filename |
|---|------------|----------------|----------|
| 41 | POST `/api/auth/login` — JWT response | JWT implemented | `api-jwt-login.png` |
| 42 | Swagger — Authorize button with Bearer token | JWT implemented | `swagger-jwt-auth.png` |
| 43 | 401 Unauthorized on protected endpoint | JWT implemented | `api-unauthorized.png` |

### AI Copilot (Planned)

| # | Screenshot | When available | Filename |
|---|------------|----------------|----------|
| 44 | AI-generated investigation summary | Copilot MVP | `ai-investigation-summary.png` |
| 45 | Compliance RAG Q&A with citations | RAG module | `ai-compliance-rag.png` |
| 46 | Relationship graph visualization | Graph module | `ui-relationship-graph.png` |

### Frontend Dashboard (Planned)

| # | Screenshot | When available | Filename |
|---|------------|----------------|----------|
| 47 | Investigator dashboard — overview | Frontend built | `ui-dashboard-overview.png` |
| 48 | Customer investigation profile page | Frontend built | `ui-customer-profile.png` |
| 49 | Alert queue / monitoring inbox | Frontend built | `ui-alert-queue.png` |

---

## Priority 4 — GitHub Repository Polish

| # | Screenshot | How to capture | Filename |
|---|------------|----------------|----------|
| 50 | GitHub repo landing page with README rendered | Push docs, view on GitHub | `github-readme-rendered.png` |
| 51 | GitHub About section with topics | Configure per [github-about.md](github-about.md) | `github-about-section.png` |
| 52 | GitHub Social Preview image | Repo Settings → Social preview | `github-social-preview.png` |
| 53 | Documentation index — `/docs` folder on GitHub | GitHub docs tree view | `github-docs-folder.png` |

---

## Capture Guidelines

### Technical quality

- **Resolution:** Minimum 1440px wide for architecture diagrams; 1280px for API screenshots
- **Theme:** Dark IDE theme for code; light theme for Swagger (better readability in README)
- **Annotations:** Optional red boxes highlighting key fields (risk score, status enum, error message)
- **PII:** Use fictional data only (`jane.investigator@example.com`, `ACC-001-987654`)

### Recommended tools

| Tool | Use |
|------|-----|
| Swagger UI | API request/response capture |
| Postman | Alternative API screenshots with collections |
| pgAdmin / DBeaver | Database schema screenshots |
| Mermaid Live Editor | Architecture diagram export |
| ShareX / CleanShot | Screen capture with annotation |
| Figma | Mockups for planned UI (Priority 3) |

### README integration template

After capturing, update README:

```markdown
## Screenshots

### API — Customer Creation
![Create Customer](docs/images/api-create-customer-response.png)

### Architecture
![System Architecture](docs/images/architecture-diagram.png)

### Database Schema
![ER Diagram](docs/images/database-er-diagram.png)
```

---

## Progress Tracker

| Priority | Total | Captured | Remaining |
|----------|-------|----------|-----------|
| P1 — Must Have | 19 | ☐ 0 | 19 |
| P2 — Engineering | 16 | ☐ 0 | 16 |
| P3 — Roadmap | 15 | ☐ 0 | 15 |
| P4 — GitHub Polish | 4 | ☐ 0 | 4 |
| **Total** | **54** | **0** | **54** |

Update this tracker as screenshots are added.

---

## Related Documents

- [README](../README.md)
- [API Specification](api-specification.md)
- [Architecture Diagram](architecture-diagram.md)
