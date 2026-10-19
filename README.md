# Osta

Backend API for a home services marketplace connecting customers with skilled, verified technicians — plumbing, electrical, carpentry, painting, AC maintenance, and more.

Built with **ASP.NET Core Web API**, **Clean Architecture**, **CQRS + MediatR**, **Entity Framework Core**, **JWT Authentication**, **Stripe**, **Redis**, **RabbitMQ**, **Hangfire**, and **SignalR**.

---

## Table of contents

- [Overview](#overview)
- [Tech stack](#tech-stack)
- [Project structure](#project-structure)
- [Implementation status](#implementation-status)
- [Getting started](#getting-started)
- [API surface](#api-surface)
- [Architecture notes](#architecture-notes)
- [Testing](#testing)
- [Contributing](#contributing)

---

## Overview

Osta connects **customers** with **technicians** in a fast, secure, and organized way. Customers sign up via email/password, Google, or Facebook; search by specialization, location, rating, and availability; book a service (with automatic technician double-booking prevention); pay securely via Stripe (with optional discount coupons); chat in real time; track status; and leave a review. Technicians build a profile, get verified by an admin, manage their services and availability, track their earnings per completed job, and request payouts through their preferred method (Vodafone Cash / Bank Transfer / InstaPay).

## Tech stack

| Layer | Technology |
| --- | --- |
| API | ASP.NET Core Web API, Swashbuckle / Swagger, API versioning |
| Architecture | Clean Architecture, Modular Monolith, CQRS |
| Request handling | MediatR |
| Data access | Entity Framework Core, SQL Server |
| Auth | ASP.NET Core Identity, JWT, refresh tokens, resource+action-based permissions (RBAC), Email/Password authentication, Google OAuth, Facebook OAuth |
| Payments | Stripe (Payment Intents, webhooks, refunds) |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Caching | Redis (Category/Service read endpoints) |
| Messaging | RabbitMQ (async notification delivery) |
| Real-time | SignalR (chat) |
| Scheduled jobs | Hangfire (appointment reminders) |
| Logging | Serilog |
| Testing | xUnit |
| Containerization | Docker & Docker Compose |
| CI/CD | GitHub Actions *(planned)* |
| Deployment | AWS (EC2) *(planned)* |

## Project structure

The solution is a **Modular Monolith** (Clean Architecture, CQRS) split across the following projects:

```
Osta.sln
├── Osta.API                  → ASP.NET Core Web API (controllers, Swagger, SignalR Hub, Hangfire dashboard). Composition root: references Osta.Core and Osta.Service.
├── Osta.Core                  → Application layer: Command/Query handlers, validators, mapping, and DI aggregation. Depends on the module projects for their interfaces/contracts.
├── Osta.Service               → Implements the interfaces consumed by Osta.Core. Business logic for Category, Service, ServiceArea, Technician, Review, Complaint, FavoriteTechnician, Technician Earnings/Payout/Wallet.
├── Osta.Booking                → Booking contracts and logic (create/confirm/refuse/cancel/complete) + Appointment scheduling with overlap prevention.
├── Osta.Chat                    → Chat contracts and real-time delivery abstractions (SignalR).
├── Osta.Payment                 → Stripe adapter (Payment Intents, webhook signature verification, refunds). No database access — pure external-service integration.
├── Osta.BackgroundJobs           → Hangfire jobs (appointment reminders).
├── Osta.Notification            → Notification DTOs, RabbitMQ publisher abstraction, and email delivery (MailKit).
├── Osta.Notification.Worker      → Standalone BackgroundService host; consumes RabbitMQ messages (e.g. payout-completed) and dispatches email.
├── Osta.Infrastructure            → EF Core DbContext, entity configurations, repositories, Redis cache integration, external service integrations.
├── Osta.Identity                   → ASP.NET Core Identity, JWT issuing/validation, Google OAuth, RBAC.
├── Osta.Domain                      → Domain entities & enums (folder: Osta.Data). No project references.
├── Osta.SharedKernel                 → Base types, Response/error wrappers, common exceptions. Near-zero dependencies.
└── Osta.Test                          → Unit / integration tests.
```

Project references (as declared in the `.csproj` files):

```
Osta.API            → Osta.Core, Osta.Service
Osta.Core           → Osta.Booking, Osta.Chat, Osta.Payment, Osta.Identity, Osta.BackgroundJobs
Osta.Service        → Osta.Core, Osta.Infrastructure, Osta.Booking, Osta.Chat, Osta.Payment,
                      Osta.Identity, Osta.Notification, Osta.BackgroundJobs, Osta.Domain, Osta.SharedKernel
Osta.Booking        → Osta.Domain
Osta.Chat           → Osta.Domain, Osta.SharedKernel
Osta.Payment        → Osta.Domain
Osta.BackgroundJobs → Osta.Domain
Osta.Identity       → Osta.Domain, Osta.Notification
Osta.Infrastructure → Osta.Domain, Osta.SharedKernel
Osta.Notification   → (none)
Osta.Domain         → (none)
```

How the pieces fit together:

- `Osta.Core` (handlers) depends on interfaces defined in the module projects; `Osta.Service` implements them. `Osta.Core` never references `Osta.Service`, so the dependency direction stays inverted.
- `Osta.API` is the composition root: it references both `Osta.Core` and `Osta.Service` and registers the implementations in DI.
- `Osta.Booking`, `Osta.Chat`, `Osta.Payment`, and `Osta.BackgroundJobs` do not reference each other — they only depend on `Osta.Domain` (and `Osta.SharedKernel` for Chat).
- `Osta.Payment` deliberately has no DB dependency, keeping the payment gateway swappable behind `IPaymentService`.

## Implementation status

| Module | DB schema | API |
| --- | --- | --- |
| Identity (Auth, Authorization, Role, Permissions, Email/Password, Google login, Facebook login) | ✅ | ✅ |
| Category / Service / ServiceArea | ✅ | ✅ |
| Technician (profile, verification, services, service areas, availability, search) | ✅ | ✅ |
| Booking | ✅ | ✅ |
| Appointment (with overlap-prevention rule) | ✅ | ✅ |
| Payment (Stripe Payment Intent, webhook, refund, history) | ✅ | ✅ |
| Coupon (single/bulk create, apply, usage tracking) | ✅ | ✅ |
| Technician Payout / Wallet | ✅ | ✅ |
| Review | ✅ | ✅ |
| Complaint | ✅ | ✅ |
| FavoriteTechnician | ✅ | ✅ |
| Chat (REST + SignalR real-time delivery) | ✅ | ✅ |
| MediaBooking | ✅ | ✅ |
| Notification (payout-completed email via RabbitMQ) | ✅ | 🚧 async only — booking/status notifications not yet built |
| Redis caching (Category/Service) | — | ✅ |
| Background jobs (Hangfire appointment reminders) | — | ✅ |
| Localization (AR/EN) | — | ⏳ not started |
| Live technician tracking | — | ⏳ not started |
| CI/CD pipeline | — | ⏳ not started |

## Getting started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (8.0+)
- SQL Server (LocalDB, container, or remote instance)
- Redis
- RabbitMQ
- A [Stripe](https://dashboard.stripe.com) account (test mode) + [Stripe CLI](https://stripe.com/docs/stripe-cli) for local webhook testing
- Docker & Docker Compose *(optional, for containerized dependencies)*

### Setup

```bash
# Clone the repo
git clone https://github.com/<your-org>/Osta.git
cd Osta

# Restore dependencies
dotnet restore

# Apply EF Core migrations
dotnet ef database update --project Osta.Infrastructure --startup-project Osta.API

# Run the API
dotnet run --project Osta.API

# In a separate terminal, run the notification worker
dotnet run --project Osta.Notification.Worker
```

### Configuration

Add your connection strings and secrets to `Osta.API/appsettings.Development.json` (or user secrets):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=OstaDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Issuer": "",
    "Audience": "",
    "Key": ""
  },
  "Authentication": {
    "Google": {
      "ClientId": "",
      "ClientSecret": ""
    },
    "Facebook": {
      "AppId": "",
      "AppSecret": ""
    }
  },
  "Email": {
    "SmtpHost": "",
    "SmtpPort": 587,
    "SenderEmail": "",
    "SenderPassword": ""
  },
  "Payment": {
    "Stripe": {
      "SecretKey": "",
      "PublishableKey": "",
      "WebhookSecret": ""
    }
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "",
    "Password": ""
  }
}
```

For local Stripe webhook testing:

```bash
stripe listen --forward-to https://localhost:<port>/api/Payment/webhook
```

### Docker

```bash
docker-compose up --build
```

Once running, Swagger UI is available at `https://localhost:<port>/swagger`.

## API surface

Live endpoints today (see Swagger for the full, versioned list):

- **Authentication** — SignUp, LogIn, Logout, email confirmation, password reset, refresh token, profile, Google login, Facebook login
- **Authorization** — role assignment, role↔permission management (resource+action-based RBAC)
- **Role, Category, Service, ServiceArea** — CRUD, pagination
- **Technician** — CRUD, pagination, search, rating filter, verify/reject, service assignment, service-area assignment, availability management, own-profile
- **Booking / Appointment** — create, confirm/refuse, cancel, complete, technician-overlap-safe scheduling, approve/reject
- **Payment** — create-intent, webhook, refund, payment history
- **Coupon** — create, bulk-generate, update, delete (soft), get by id/code, apply
- **Payout / Wallet** — request, cancel, reject, complete, balance, my/pending listings
- **Review / Complaint / FavoriteTechnician / MediaBooking** — CRUD + relevant listing endpoints
- **Chat** — send message, per-booking history (REST) + real-time delivery (SignalR Hub)

## Architecture notes

- **Modular Monolith**: each business domain owns its own commands, queries, handlers, DTOs, and validators, while sharing one deployment unit and database — set up to split into microservices later if needed (API Gateway, `Osta.Payment` / `Osta.Notification` are already isolated as natural extraction candidates).
- **CQRS via MediatR**: reads and writes are modeled as separate requests/handlers, with a shared `Response<T>` result wrapper and a `ValidationBehavior` pipeline running FluentValidation before every handler.
- **RBAC**: authorization is resource+action based (`Permission` / `RolePermission`) rather than plain role checks.
- **Payment gateway isolation**: `Osta.Payment` implements `IPaymentService` with zero DB coupling — all persistence (Payment record, Coupon usage, Earning creation) is orchestrated by `Osta.Core` handlers, so the gateway itself is swappable.
- **Async notifications**: command handlers publish lightweight DTOs to RabbitMQ; `Osta.Notification.Worker` — a separate process — consumes and dispatches them, keeping the request path fast and decoupled from email delivery.
- **Known follow-up (modularity)**: modules currently share a single `Osta.Domain` project and one `DbContext`/database, and `Osta.Identity` still references `Osta.Notification`. Planned: move the email-sender abstraction out of `Osta.Notification`, keep `Osta.Core` dependent on abstractions only, and add architecture tests in `Osta.Test` to enforce the reference rules above.
- **Known follow-up**: Stripe webhook processing does not yet guard against duplicate event delivery (idempotency by `event.id`) — flagged as next-priority reliability work.

## Testing

```bash
dotnet test
```

`Osta.Test` covers unit and integration tests for the application and domain layers. **Priority coverage pending** for: `ApplyCouponCommandHandler`, `RequestPayoutCommandHandler`, `CreatePaymentIntentCommandHandler`, `HandleStripeWebhookCommandHandler`, and appointment-overlap validation.

## Contributing

1. Create a feature branch off `main`.
2. Follow the existing CQRS/MediatR command-query structure for new endpoints.
3. Add FluentValidation validators for new commands.
4. Add/update tests in `Osta.Test`.
5. Open a PR.

---

**Status:** Active development — Identity, Service, Technician, Booking, Appointment, Payment, Coupon, Payout/Wallet, Review, Complaint, FavoriteTechnician, MediaBooking, and Chat are live. Booking-lifecycle/payment-status notifications, localization, and live technician tracking are next up.
