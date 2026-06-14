# Startup Pitch

Investor and founder-facing narrative for SentinelAML — grounded in the implemented platform and documented roadmap.

---

## The Problem

Financial crime investigations are **manual, slow, and fragmented**.

Fraud and AML teams at banks, fintechs, and payment processors face a compounding challenge:

- **Alert volume is rising** — transaction monitoring systems generate more suspicious activity reports than teams can investigate thoroughly
- **Context is scattered** — customer profiles, account histories, and transaction trails live across core banking, TMS, CRM, and spreadsheets
- **Investigations are repetitive** — analysts spend hours reconstructing timelines and drafting similar narrative reports
- **Regulatory pressure is increasing** — fines for AML failures reach billions annually; auditors demand consistent, traceable decision records
- **Tooling lag** — legacy case management systems were not built for AI-assisted workflows or real-time risk scoring

The result: long investigation cycle times, analyst burnout, missed true positives, and expensive compliance risk.

---

## The Solution

**SentinelAML unifies investigation data and workflows on a modern, API-first platform built for scale and AI augmentation.**

### What exists today

SentinelAML delivers a **production-grade backend foundation**:

| Capability | Investigator benefit |
|------------|---------------------|
| Unified customer & account model | One profile view for every investigation subject |
| Domain-enforced financial rules | Trustworthy balances and account states in case files |
| REST APIs + Swagger | Integrates with analyst dashboards and existing bank systems |
| CQRS + Clean Architecture | New AML modules ship fast without destabilizing core data |
| PostgreSQL + audit timestamps | Durable, queryable records for regulatory review |

### How it improves productivity (today)

1. **Faster case setup** — Create customers and link accounts via API instead of manual data entry across systems
2. **Reliable account lifecycle** — Close accounts only when balance is zero — domain rules prevent inconsistent case records
3. **Integration-ready** — API-first design allows TMS alerts to push context into SentinelAML as the investigation hub
4. **Operational confidence** — Database health checks and structured validation errors reduce downtime during live investigations

### What ships next (near-term roadmap)

| Module | Productivity gain |
|--------|-------------------|
| Transaction processing | Real-time monitoring feed with risk scores on every transfer |
| Investigation cases | Assign, track, and escalate cases in one system |
| JWT + RBAC | Secure multi-analyst access with role-appropriate permissions |
| Relationship analysis | Surface hidden connections between subjects and accounts |
| Timeline reconstruction | Automatic chronological view of suspicious activity |

---

## Future Vision

### AI Copilot for Investigators

SentinelAML is architected to become an **AI-powered investigation copilot**, not a black-box decision engine.

**Vision:** An analyst opens a case and asks:

> "Summarize suspicious activity for Customer X over the last 90 days, highlight high-risk transactions, and draft a preliminary investigation narrative with regulatory citations."

The platform responds with:

- A **timeline** of flagged transactions ranked by `RiskScore`
- A **relationship graph** of linked accounts and counterparties
- A **draft report** generated via Semantic Kernel + RAG over case data and compliance knowledge bases
- **Human-in-the-loop approval** before any filing or escalation

### Why this architecture supports the vision

- **`SentinelAML.Infrastructure`** is reserved for AI providers (Ollama, Semantic Kernel) — swappable without rewriting business logic
- **`Transaction.RiskScore`** is already modeled — ML and rule engines plug in at completion time
- **CQRS read models** will power dashboards and graph queries without overloading write paths
- **Clean Architecture** ensures AI is an advisor layer, not the source of financial truth

### Market positioning

| Segment | Value proposition |
|---------|-------------------|
| Mid-size fintechs | Enterprise-grade AML tooling without enterprise implementation timelines |
| Regional banks | Modernize investigation workflows without replacing core banking |
| RegTech integrators | API platform to embed AI-assisted investigations into existing products |

---

## Traction & Technical Proof Points

| Metric | Status |
|--------|--------|
| Core domain model | ✅ Customer, Account, Transaction with invariants |
| REST API surface | ✅ 8 live endpoints + health checks |
| Architecture | ✅ Clean Architecture + CQRS + MediatR |
| Database | ✅ PostgreSQL with EF Core, unique constraints, RESTRICT deletes |
| Test foundation | ✅ Unit + integration test projects |
| AI integration point | ✅ Infrastructure layer stub + RiskScore field |

---

## Ask / Next Milestones

Typical seed-stage engineering milestones this codebase de-risks:

1. **Transaction module** — unlock monitoring demo for pilot banks
2. **JWT auth + audit log** — enterprise security baseline
3. **Case management MVP** — end-to-end investigator workflow demo
4. **AI copilot alpha** — differentiated demo for investor and customer conversations

---

## Related Documents

- [Case Study](case-study.md) — founding engineer portfolio narrative
- [Technical Highlights](technical-highlights.md) — engineering decisions
- [System Design](system-design.md) — architecture depth
