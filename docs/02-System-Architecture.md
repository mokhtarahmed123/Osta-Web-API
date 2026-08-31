# System Architecture

# Osta Plus Backend Architecture

---

## Table of Contents

1. [Architecture Overview](#1-architecture-overview)
2. [Architectural Principles](#2-architectural-principles)
3. [Architecture Style](#3-architecture-style)
4. [Clean Architecture Layers](#4-clean-architecture-layers)
5. [Technology Stack](#5-technology-stack)
6. [Implementation Status](#6-implementation-status)
7. [Future Scalability](#7-future-scalability)

---

## 1. Architecture Overview

Osta Plus follows **Clean Architecture** principles with a **Modular Monolith** architecture style. The system is designed to be scalable, maintainable, testable, and easy to extend in the future.

The application separates business logic from infrastructure concerns while following the **CQRS** (Command Query Responsibility Segregation) pattern with **MediatR** to decouple requests from their handlers.

The architecture also uses distributed technologies such as RabbitMQ (asynchronous notification delivery) and Hangfire (scheduled/background jobs), and integrates with a third-party payment gateway (Stripe), making it easy to evolve into Microservices if required in the future.

---

## 2. Architectural Principles

The system is designed based on the following principles:

- Separation of Concerns (SoC)
- Single Responsibility Principle (SRP)
- Dependency Inversion Principle (DIP)
- Open/Closed Principle (OCP)
- Clean Architecture
- Domain-Driven Design (DDD) concepts
- CQRS Pattern
- Modular Monolith Architecture
- SOLID Principles
- RESTful API Design

---

## 3. Architecture Style

The project follows a **Modular Monolith** architecture. Each business domain is implemented as an independent module while sharing the same deployment unit and database.

### Current Solution Modules

- Osta.API
- Osta.Booking
- Osta.Core
- Osta.Domain
- Osta.Identity
- Osta.Infrastructure
- Osta.Notification (+ Osta.Notification.Worker — RabbitMQ consumer host)
- Osta.Payment
- Osta.Service
- Osta.SharedKernel
- Osta.Test

Real-time communication (chat) is implemented via **SignalR**, alongside the existing REST endpoints (hybrid: REST for history/persistence, SignalR Hub for live delivery). **Redis** is used as a distributed cache for the high-read Category/Service endpoints.

Each business-facing module (Service, Booking, Payment, Notification) contains its own:

- Commands
- Queries
- Handlers
- DTOs
- Validators
- Business Rules

---

## 4. Clean Architecture Layers

### Dependency Chain (inner → outer)

```
Osta.SharedKernel   (zero / near-zero dependencies — lowest-level project)
      ▲
Osta.Domain          references SharedKernel
      ▲
Osta.Identity        references Domain, SharedKernel
      ▲
Osta.Infrastructure  references Domain, SharedKernel, Identity
      ▲
Osta.Payment         references Domain, SharedKernel (Stripe integration, no DB access)
Osta.Service         references Domain, Infrastructure, SharedKernel
Osta.Booking         references Domain, Service, Infrastructure, SharedKernel
Osta.Notification    references Domain, Infrastructure, SharedKernel (+ RabbitMQ.Client)
      ▲
Osta.Core            composition root — references Identity, Infrastructure,
                     Service, Booking, Notification, Payment
      ▲
Osta.API             references Core only
```

```
┌────────────────────────────────────────────────────────────┐
│                          Osta.API                            │
│              (Sdk.Web) Controllers · Swagger                 │
└──────────────────────────────┬───────────────────────────────┘
                                │ references
┌──────────────────────────────▼───────────────────────────────┐
│                          Osta.Core                             │
│               Composition Root / DI Aggregator                │
└───┬─────────┬─────────┬─────────┬───────────────────────────┘
    │         │         │         │
┌───▼──┐ ┌────▼───┐ ┌───▼────┐ ┌──▼──────────────┐
│Service│ │Booking │ │Payment │ │Notification      │
└───┬──┘ └────┬───┘ └───┬────┘ └──┬────────────────┘
    │         │         │          │
    └─────────┴─────────┴──────────┘
                          │ references
                ┌─────────▼──────────┐
                │ Osta.Infrastructure │
                └─────────┬────────────┘
                          │ references
                  ┌───────▼────────┐
                  │  Osta.Identity  │
                  └───────┬─────────┘
                          │ references
                   ┌──────▼───────┐
                   │  Osta.Domain  │
                   └──────┬────────┘
                          │ references
                ┌─────────▼─────────┐
                │  Osta.SharedKernel  │
                └────────────────────┘

                (Separate process)
        ┌─────────────────────────────┐
        │  Osta.Notification.Worker    │
        │  (Hosted Service / Consumer) │
        │  Consumes RabbitMQ messages  │
        │  → sends email/push          │
        └─────────────────────────────┘
```

### Osta.API
- ASP.NET Core Web API (`Microsoft.NET.Sdk.Web`)
- Swashbuckle / Swagger, API versioning (`v{version}` route segments)
- Entry point of the application
- References only **Osta.Core**, keeping the composition root as the single point of contact
- Hosts controllers: `Authentication`, `Authorization`, `Role`, `Category`, `Service`, `ServiceArea`, `Technician`, `TechnicianAvailability`, `TechnicianService`, `TechnicianServiceArea`, `TechnicianVerification`, `Booking`, `Appointment`, `Payment`, `Payout`, `Wallet`, `Coupon`, `Review`, `Complaint`, `FavoriteTechnician`, `Chat`, `MediaBooking`
- Also hosts Hangfire dashboard (`/hangfire`), the recurring appointment-reminder job, and the **Chat SignalR Hub** (real-time message delivery alongside the existing REST send/history endpoints)

### Osta.Core
- Acts as the **composition root**: pulls together Identity, Infrastructure, Service, Booking, Payment, and Notification so the API stays lightweight
- Owns DI registration / service collection extensions consumed by `Osta.API`
- Hosts all Command/Query handlers and FluentValidation validators (organized per feature: `Feature/Payment`, `Feature/Coupon`, `Feature/Technician/.../TechnicianPayout`, etc.)

### Osta.Service
- Business/application logic for Category, Service, ServiceArea, Technician (profile, verification, service assignment, service areas, availability), Review, Complaint, FavoriteTechnician, TechnicianEarning, TechnicianPayout, TechnicianWallet
- References Domain, Infrastructure, SharedKernel

### Osta.Booking
- Booking domain (create/cancel/confirm/refuse/complete) and Appointment scheduling (create/update/approve/reject), including the technician double-booking (overlap) prevention rule
- References Domain, Service, Infrastructure, SharedKernel
- Fully exposed through `Osta.API` (`Booking`, `Appointment` controllers)

### Osta.Payment
- Encapsulates all Stripe SDK interaction: `CreatePaymentIntentAsync`, `RefundPaymentAsync`, `ConstructWebhookEvent` (signature verification)
- Deliberately has **no database access** — it is a pure external-service adapter, consumed by `Osta.Core` handlers (`CreatePaymentIntentCommandHandler`, `HandleStripeWebhookCommandHandler`, `RefundPaymentCommandHandler`) which own persistence via `IPaymentRepository`
- References Domain, SharedKernel

### Osta.Notification
- Defines notification DTOs (`PayoutNotification`, etc.) and the `ISendNotificationMessage` publisher abstraction (RabbitMQ producer)
- **Osta.Notification.Worker**: a separate `BackgroundService` host that consumes queued messages (e.g. `payout-notification`) and dispatches them via `IEmailService`
- References Domain, Infrastructure, SharedKernel

### Osta.Infrastructure
- EF Core `DbContext`, repository implementations, external service integrations (email, storage)
- Hangfire SQL Server storage configuration and the recurring appointment-reminder job registration
- **Redis distributed cache integration** (`IDistributedCache` / StackExchange.Redis) for Category and Service read endpoints, with cache invalidation on create/update/delete
- References Domain, SharedKernel, Identity

### Osta.Identity
- ASP.NET Core Identity implementation: `User`, `Role`, `RefreshToken`, JWT issuing/validation, Google OAuth login
- Backs the `Authentication`, `Authorization`, and `Role` controllers
- References Domain, SharedKernel

### Osta.Domain
- The actual domain entities and enums, with **no outgoing project references**
- Includes payment-related entities added in this phase: `Payment`, `Coupons`, `CouponUsage`, `TechnicianEarning`, `TechnicianPayout`, `TechnicianWallet`, `PayoutStatus`, `PayoutMethod`, `DiscountTypeEnum`, `PaymentStatus`, `PaymentMethod`

### Osta.SharedKernel
- Cross-cutting building blocks: base entity/audit types, `Response<T>`/`ResponseHandler` result wrappers, common exceptions (`BadRequestException`, `NotFoundException`), constants, `ICurrentUserService`
- Near-zero dependencies; sits beneath even `Osta.Domain`

### Osta.Test
- Unit/integration tests for the Application and Domain layers
- **Priority coverage still pending** for: `ApplyCouponCommandHandler`, `RequestPayoutCommandHandler`, `CreatePaymentIntentCommandHandler`, `HandleStripeWebhookCommandHandler`, appointment-overlap validation

---

## 5. Technology Stack

| Technology | Purpose |
|---|---|
| ASP.NET Core Web API | Backend Framework |
| Clean Architecture | Project Architecture |
| Modular Monolith | Architecture Style |
| CQRS | Command & Query Separation |
| MediatR | Request Handling |
| Entity Framework Core | ORM |
| SQL Server | Database |
| ASP.NET Identity | Authentication |
| JWT | Authorization |
| Google OAuth | Social Login |
| Stripe.net | Payment Processing |
| RabbitMQ.Client | Asynchronous Messaging (notifications) |
| Hangfire (SQL Server storage) | Scheduled / Background Jobs |
| SignalR | Real-time chat delivery |
| Redis (StackExchange.Redis) | Distributed Caching (Category/Service reads) |
| FluentValidation | Validation |
| AutoMapper | Object Mapping |
| Serilog | Logging |
| Swagger (OpenAPI) | API Documentation |
| Postman | API Testing |
| Xunit | Unit Testing |
| Docker & Docker Compose | Containerization |
| Git & GitHub | Version Control |
| GitHub Actions | CI/CD *(planned)* |
| AWS Cloud (EC2) | Deployment *(planned)* |

---

## 6. Implementation Status

| Module | Status | Notes |
|---|---|---|
| Identity (Auth, Authorization, Role) | ✅ Implemented | SignUp, LogIn, Logout, email confirmation, reset password, refresh token, Google login, role/permission management |
| Category / Service / ServiceArea | ✅ Implemented | Full CRUD + pagination (note: `GET /Service/Satrt` is a typo for `Start`) |
| Technician | ✅ Implemented | Profile CRUD, verify/reject, service assignment, service areas, availabilities, search, pagination, rating filter, own-profile endpoint |
| Booking | ✅ Implemented | Create, confirm/refuse, cancel, complete, history |
| Appointment | ✅ Implemented | CRUD, approve/reject, technician overlap-prevention business rule |
| Payment | ✅ Implemented | Stripe Payment Intent, webhook, refund, payment history |
| Coupon | ✅ Implemented | CRUD, bulk generation, apply, usage tracking |
| Technician Payout / Wallet | ✅ Implemented | Request, cancel, reject, complete, balance validation, email notification via RabbitMQ |
| Review | ✅ Implemented | CRUD, own/technician views |
| Complaint | ✅ Implemented | CRUD, status update, per-booking view |
| FavoriteTechnician | ✅ Implemented | Add/remove/list |
| Chat | ✅ Implemented (REST + SignalR) | Send message / per-booking history via REST, plus a SignalR Hub for real-time delivery |
| MediaBooking | ✅ Implemented | CRUD + filter by repair type, per-booking listing |
| Notification | 🚧 Partially implemented | Payout-completion email via RabbitMQ + Worker; booking/status-update notifications not yet built |
| Background Jobs | ✅ Implemented | Hangfire recurring appointment-reminder job |
| Redis Caching | ✅ Implemented | Category/Service read endpoints cached via Redis, invalidated on write |
| Localization (AR/EN) | ⏳ Not started | Planned |
| Live Tracking | ⏳ Not started | Planned |
| CI/CD Pipeline | ⏳ Not started | Planned |

---

## 7. Future Scalability

The architecture is designed to support future migration to Microservices if required.

Potential future improvements include:

- API Gateway / Rate Limiting
- Extend Redis caching to additional read-heavy endpoints (Technician search/profile)
- Separate Notification Service (already isolated as `Osta.Notification` + a standalone worker process — a natural extraction candidate)
- Separate Payment Service (already isolated as `Osta.Payment` with no DB coupling — a natural extraction candidate)
- Event-Driven Communication (Outbox pattern on top of the existing RabbitMQ integration)
- Extend SignalR beyond chat to live technician GPS tracking
- Webhook idempotency guard (dedupe Stripe `event.id` before processing)
- Kubernetes Deployment
- AWS Managed Services (RDS, S3, ElastiCache)

---

**Document Version:** 3.0
**Status:** Draft — updated to include Osta.Payment, Osta.Notification.Worker, Hangfire, and current Swagger-verified module status
