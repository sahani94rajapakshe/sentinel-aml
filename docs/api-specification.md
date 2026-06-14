# API Specification

REST API reference for **SentinelAML API v1**, derived from controllers in `src/SentinelAML.API/Controllers/`.

**Base URL (Development):** `http://localhost:5077`  
**OpenAPI / Swagger:** `http://localhost:5077/swagger` (Development only)  
**Content-Type:** `application/json`

---

## Authentication

| Status | Detail |
|--------|--------|
| **Current** | No authentication required — all endpoints are public |
| **Planned** | JWT Bearer token in `Authorization: Bearer <token>` header |

When JWT is implemented, sensitive endpoints (account mutations, future case data) will require authenticated analyst roles. Health endpoints will remain anonymous for load balancer probes.

---

## Common Response Patterns

### Success

Standard JSON body per endpoint (see below).

### Validation error (400)

Thrown by `ValidationBehavior` → `ExceptionHandlingMiddleware`:

```json
{
  "title": "Validation failed.",
  "status": 400,
  "errors": {
    "Email": ["'Email' is not a valid email address."]
  }
}
```

### Business rule failure (400)

Handler returns `Result.Failure` — controller responds with:

```json
{
  "errors": ["An account with this account number already exists."]
}
```

### Not found (404)

When error message contains `"not found"` (case-insensitive):

```json
{
  "errors": ["Customer not found."]
}
```

### Server error (500)

```json
{
  "title": "An unexpected error occurred.",
  "status": 500,
  "errors": null
}
```

---

## Health Controller

**Route prefix:** `/api/health` (inherits `api/[controller]` from `ApiControllerBase`)

---

### GET /api/health

Service liveness check.

| Property | Value |
|----------|-------|
| **Method** | `GET` |
| **Authentication** | None |
| **Purpose** | Confirm API process is running |

**Request example:**

```http
GET /api/health HTTP/1.1
Host: localhost:5077
```

**Response `200 OK`:**

```json
{
  "status": "healthy",
  "service": "SentinelAML.API"
}
```

---

## Database Health Check

Registered in `Program.cs` — not a controller endpoint.

---

### GET /health/database

PostgreSQL connectivity check via EF Core health check.

| Property | Value |
|----------|-------|
| **Method** | `GET` |
| **Authentication** | None |
| **Purpose** | Operational monitoring of database availability |

**Request example:**

```http
GET /health/database HTTP/1.1
Host: localhost:5077
```

**Response:** ASP.NET Core Health Checks format (JSON when configured; may return `Healthy`, `Degraded`, or `Unhealthy` status).

---

## Customer Controller

**Route prefix:** `/api/customers`

---

### POST /api/customers

Create a new customer.

| Property | Value |
|----------|-------|
| **Method** | `POST` |
| **Authentication** | None (planned: Analyst+) |
| **Purpose** | Register investigation subject with unique email |

**Request body:**

```json
{
  "firstName": "Jane",
  "lastName": "Investigator",
  "email": "jane.investigator@example.com",
  "phoneNumber": "+15551234567"
}
```

**Validation rules** (`CreateCustomerCommandValidator`):

| Field | Rules |
|-------|-------|
| `firstName` | Required, max 100 |
| `lastName` | Required, max 100 |
| `email` | Required, valid email, max 255 |
| `phoneNumber` | Required, max 20 |

**Response `201 Created`:**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Location header:** `GET /api/customers/{id}`

**Response `400 Bad Request` — duplicate email:**

```json
{
  "errors": ["A customer with this email already exists."]
}
```

---

### GET /api/customers

List all customers.

| Property | Value |
|----------|-------|
| **Method** | `GET` |
| **Authentication** | None (planned: Analyst+) |
| **Purpose** | Retrieve customer directory for investigation workflows |

**Request example:**

```http
GET /api/customers HTTP/1.1
Host: localhost:5077
```

**Response `200 OK`:**

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "Jane",
    "lastName": "Investigator",
    "email": "jane.investigator@example.com",
    "phoneNumber": "+15551234567"
  }
]
```

---

### GET /api/customers/{id}

Retrieve a single customer by ID.

| Property | Value |
|----------|-------|
| **Method** | `GET` |
| **Authentication** | None (planned: Analyst+) |
| **Purpose** | Load customer profile for case context |

**Path parameters:**

| Name | Type | Required |
|------|------|----------|
| `id` | `uuid` | Yes |

**Request example:**

```http
GET /api/customers/3fa85f64-5717-4562-b3fc-2c963f66afa6 HTTP/1.1
Host: localhost:5077
```

**Response `200 OK`:**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "Jane",
  "lastName": "Investigator",
  "email": "jane.investigator@example.com",
  "phoneNumber": "+15551234567"
}
```

**Response `404 Not Found`:**

```json
{
  "errors": ["Customer not found."]
}
```

---

## Account Controller

**Route prefix:** `/api`

---

### POST /api/accounts

Create an account for an existing customer.

| Property | Value |
|----------|-------|
| **Method** | `POST` |
| **Authentication** | None (planned: Analyst+) |
| **Purpose** | Link financial account to customer for monitoring and investigation |

**Request body:**

```json
{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "accountNumber": "ACC-001-987654",
  "balance": 1000.00
}
```

**Validation rules** (`CreateAccountCommandValidator`):

| Field | Rules |
|-------|-------|
| `customerId` | Required (non-empty GUID) |
| `accountNumber` | Required, max 50 |
| `balance` | >= 0 |

**Response `201 Created`:**

```json
{
  "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7"
}
```

**Location header:** `GET /api/accounts/{id}`

**Response `404 Not Found` — customer missing:**

```json
{
  "errors": ["Customer not found."]
}
```

**Response `400 Bad Request` — duplicate account number:**

```json
{
  "errors": ["An account with this account number already exists."]
}
```

---

### GET /api/accounts/{id}

Retrieve account details by ID.

| Property | Value |
|----------|-------|
| **Method** | `GET` |
| **Authentication** | None (planned: Analyst+) |
| **Purpose** | View account balance and status during investigation |

**Path parameters:**

| Name | Type | Required |
|------|------|----------|
| `id` | `uuid` | Yes |

**Response `200 OK`:**

```json
{
  "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "accountNumber": "ACC-001-987654",
  "balance": 1000.00,
  "status": 0
}
```

**`status` enum:** `0` = Active, `1` = Suspended, `2` = Closed

**Response `404 Not Found`:**

```json
{
  "errors": ["Account not found."]
}
```

---

### GET /api/customers/{customerId}/accounts

List all accounts belonging to a customer.

| Property | Value |
|----------|-------|
| **Method** | `GET` |
| **Authentication** | None (planned: Analyst+) |
| **Purpose** | Aggregate account view for customer-centric investigations |

**Path parameters:**

| Name | Type | Required |
|------|------|----------|
| `customerId` | `uuid` | Yes (validated non-empty) |

**Response `200 OK`:**

```json
[
  {
    "id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "accountNumber": "ACC-001-987654",
    "balance": 1000.00,
    "status": 0
  }
]
```

**Response `404 Not Found` — customer missing:**

```json
{
  "errors": ["Customer not found."]
}
```

---

### PUT /api/accounts/{id}/close

Close an account. Domain rule: balance must be zero.

| Property | Value |
|----------|-------|
| **Method** | `PUT` |
| **Authentication** | None (planned: SeniorAnalyst+) |
| **Purpose** | Finalize account lifecycle when investigation or offboarding completes |

**Path parameters:**

| Name | Type | Required |
|------|------|----------|
| `id` | `uuid` | Yes (validated non-empty) |

**Request body:** None

**Response `200 OK`:** Empty body

**Response `400 Bad Request` — non-zero balance:**

```json
{
  "errors": ["Account balance must be zero before closing."]
}
```

**Response `400 Bad Request` — already closed:**

```json
{
  "errors": ["Account is already closed."]
}
```

**Response `404 Not Found`:**

```json
{
  "errors": ["Account not found."]
}
```

---

## Planned Endpoints (Not Yet Implemented)

Derived from domain model and product roadmap:

| Method | Endpoint | Purpose |
|--------|----------|---------|
| `POST` | `/api/transactions` | Initiate transfer between accounts |
| `PUT` | `/api/transactions/{id}/complete` | Complete with risk score |
| `PUT` | `/api/transactions/{id}/fail` | Mark transaction failed |
| `GET` | `/api/transactions` | Search/filter monitoring feed |
| `POST` | `/api/auth/login` | JWT token issuance |
| `POST` | `/api/cases` | Create investigation case |
| `POST` | `/api/cases/{id}/reports` | AI-assisted report generation |

---

## Error Code Summary

| HTTP Status | When |
|-------------|------|
| `200` | Successful read or close operation |
| `201` | Resource created |
| `400` | Validation failure or business rule violation |
| `404` | Customer or account not found |
| `500` | Unhandled server exception |

---

## Related Documents

- [System Design](system-design.md)
- [Database Design](database-design.md)
