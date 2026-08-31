# Domain Model

## Osta Plus Domain Model

---

## Table of Contents

1. [Overview](#1-overview)
2. [Domain Modules](#2-domain-modules)
3. [Module Details](#3-module-details)
4. [Aggregate Roots](#4-aggregate-roots)
5. [Value Objects](#5-value-objects)
6. [Enumerations](#6-enumerations)
7. [Future Modules](#7-future-modules)

---

## 1. Overview

The Osta Plus domain is organized into multiple business modules following a **Modular Monolith** architecture. Domain entities and enums live in **`Osta.Domain`** (zero outgoing references), shared building blocks live in **`Osta.SharedKernel`**. Business logic for each module is implemented in its own project where one exists (`Osta.Identity`, `Osta.Service`, `Osta.Booking`, `Osta.Payment`, `Osta.Notification`); cross-cutting modules (Complaint, Admin, Support) currently live inside `Osta.Infrastructure` / `Osta.Core`.

---

## 2. Domain Modules

| Module | Backing Project | Status |
| --- | --- | --- |
| Identity | `Osta.Identity` | ✅ Implemented |
| Customer | `Osta.Domain` + `Osta.Service` | 🚧 Partially implemented (profile/address management pending) |
| Technician | `Osta.Service` | ✅ Implemented |
| Service | `Osta.Service` | ✅ Implemented |
| Booking & Appointment | `Osta.Booking` | ✅ Implemented |
| Payment | `Osta.Payment` (adapter) + `Osta.Core` (persistence/orchestration) | ✅ Implemented |
| Coupon | `Osta.Core` + `Osta.Payment` (settings) | ✅ Implemented |
| Technician Earning / Payout / Wallet | `Osta.Service` + `Osta.Core` | ✅ Implemented |
| Review | `Osta.Service` | ✅ Implemented |
| Complaint | *(cross-cutting)* | ✅ Implemented |
| FavoriteTechnician | `Osta.Service` | ✅ Implemented |
| Chat | *(cross-cutting)* | ✅ Implemented (REST + SignalR) |
| Notification | `Osta.Notification` + `Osta.Notification.Worker` | 🚧 Partially implemented |
| Administration | *(cross-cutting, `Osta.Infrastructure`)* | 🚧 Partially implemented |

---

## 3. Module Details

### 3.1 Identity Module — *Implemented*
**Purpose:** Authentication, authorization, user accounts, and role management.
**Main Entities:** `User`, `Role`, `RefreshToken`, `Permission`, `RolePermission`

---

### 3.2 Customer Module — *Partially implemented*
**Purpose:** Represents customers who request home services.
**Main Entity:** `Customer` (backed by `User`)
**Responsibilities:** Manage profile, create bookings, view booking history, submit reviews, manage favorite technicians, file complaints, view payment history.

---

### 3.3 Technician Module — *Implemented*
**Purpose:** Represents skilled workers who provide services.
**Main Entity:** `Technician`
**Responsibilities:** Manage profile (CRUD, verify, reject), manage provided services and service areas, define working availability, accept/reject bookings, complete jobs, receive customer reviews, track earnings, request payouts.

---

### 3.4 Service Module — *Implemented*
**Purpose:** Defines the services offered through the platform.
**Main Entities:** `Category`, `Service`, `TechnicianService`

---

### 3.5 Booking & Appointment Module — *Implemented*
**Purpose:** Handles service booking and scheduling.
**Main Entities:** `Booking`, `Appointment`, `Media`
**Responsibilities:**
- Create/cancel/confirm/refuse/complete bookings.
- Schedule appointments with **technician double-booking prevention** (core business rule — see Business-Rules.md BR-014/BR-035a).
- Approve/reject appointments.
- Store before/after service media.
- Trigger scheduled reminders (Hangfire) for upcoming appointments.

---

### 3.6 Payment Module — *Implemented*
**Purpose:** Handles all financial transactions between customers and the platform.
**Main Entities:** `Payment`, `Coupons`, `CouponUsage`
**Responsibilities:**
- Create a Stripe Payment Intent for a booking, optionally applying a coupon discount first.
- Verify and process Stripe webhook events (`payment_intent.succeeded`, `payment_intent.payment_failed`) with signature validation.
- Record coupon usage (once, per customer) upon confirmed payment success.
- Process refunds for completed payments via the Stripe Refund API.
- Expose payment history per customer.
- Trigger technician earning calculation on successful payment.

**Design note:** `Osta.Payment` is a pure Stripe adapter with **no database dependency** — all persistence (Payment entity, Coupon usage, Earning creation) is orchestrated by `Osta.Core` command handlers, keeping the payment-gateway integration swappable (e.g. a future Paymob adapter could implement the same `IPaymentService` interface without touching the handlers).

---

### 3.7 Coupon Module — *Implemented*
**Purpose:** Discount codes and promotional campaigns.
**Main Entities:** `Coupons`, `CouponUsage`
**Responsibilities:**
- Single or bulk (auto-generated code) coupon creation.
- Percentage or fixed-amount discount types.
- Activity window (`StartDate`/`EndDate`), usage limit, and per-customer single-use enforcement.
- Soft-deactivation (no hard delete) to preserve historical usage records.

---

### 3.8 Technician Earning / Payout / Wallet Module — *Implemented*
**Purpose:** Tracks technician income and manages fund withdrawal requests.
**Main Entities:** `TechnicianEarning`, `TechnicianPayout`, `TechnicianWallet`
**Responsibilities:**
- Automatically record gross/fee/net earning per successful booking payment.
- Track technician wallet balance.
- Accept payout requests (amount + method + receiving details) with real-time available-balance validation (accounting for pending requests).
- Support the full request → (approve) → complete / reject → cancel lifecycle.
- Notify the technician by email once a payout is completed.

**Design note:** actual fund transfer is performed **manually outside the system** (bank transfer / mobile wallet, done by an administrator); the module's responsibility is limited to request tracking, balance validation, and audit trail — not real-time bank integration.

---

### 3.9 Review Module — *Implemented*
**Purpose:** Manages customer feedback.
**Main Entity:** `Review`
**Responsibilities:** Store ratings and written reviews; only allowed after booking completion; one review per booking.

---

### 3.10 Complaint Module — *Implemented*
**Purpose:** Handles customer complaints tied to a specific booking.
**Main Entity:** `Complaint`
**Responsibilities:** Submit/update/delete complaints, admin status management, per-booking and own-complaints views.

---

### 3.11 FavoriteTechnician Module — *Implemented*
**Purpose:** Lets customers bookmark preferred technicians for faster future bookings.
**Main Entity:** `FavoriteTechnician`

---

### 3.12 Chat Module — *Implemented (REST + SignalR)*
**Purpose:** Enables customer–technician communication per booking.
**Main Entity:** `Message`
**Status:** Hybrid design — messages are persisted and retrievable via REST (`POST /Chat/send`, `GET /Chat/booking/{id}`), while a **SignalR Hub** delivers new messages to connected clients in real time.

---

### 3.13 Notification Module — *Partially implemented*
**Purpose:** Handles asynchronous user notifications.
**Main Entity:** `Notification`; DTO: `PayoutNotification`
**Responsibilities:**
- Publish notification messages to RabbitMQ from within command handlers (e.g. `CompletePayoutCommandHandler`).
- `Osta.Notification.Worker` (a standalone `BackgroundService`) consumes the queue and dispatches email via `IEmailService`.
- **Implemented:** payout-completion email.
- **Not yet implemented:** booking-lifecycle and payment-status notifications.

---

### 3.14 Administration Module — *Partially implemented*
**Purpose:** Provides platform management features.
**Responsibilities:** User/technician management, technician verification, category management, coupon management, payout approval, refund processing, complaint resolution. Reports/analytics and system settings are not yet implemented.

---

## 4. Aggregate Roots

- `Customer`
- `Technician`
- `Booking`
- `Payment`
- `Coupons`
- `TechnicianPayout`
- `Category`

Each Aggregate Root is responsible for maintaining the consistency of its internal business rules, and lives in `Osta.Domain` regardless of whether its owning module currently has a dedicated project.

---

## 5. Value Objects

Currently modeled as plain properties rather than dedicated Value Object types (candidates for future refactor):

- `Address` *(fields inline on Booking, not yet extracted)*
- `Money` *(currently `decimal` fields — `Amount`, `NetAmount`, etc.)*
- `PhoneNumber`
- `Email`
- `WorkingHours` *(fields inline on `TechnicianAvailability`)*

---

## 6. Enumerations

Implemented in `Osta.Domain`:

- `BookingStatus`
- `PaymentStatus` (Pending, Completed, Failed, Refunded)
- `PaymentMethod` (Card, Cash)
- `DiscountTypeEnum` (Percentage, FixedAmount)
- `PayoutStatus` (Pending, Approved, Rejected, Completed, Cancelled)
- `PayoutMethod` (VodafoneCash, BankTransfer, InstaPay)
- `ComplaintStatus`
- `StatusOfTechnicianRequestEnum`
- `RepairMediaTypeEnum` / `MediaFileType`
- `DayOfWeek`

Planned:
- `NotificationType`
- `UserRole` (currently role names are string-based via ASP.NET Identity rather than a fixed enum)

---

## 7. Future Modules

- Wallet top-up / customer-side wallet *(technician-side wallet already implemented; customer-side not started)*
- Subscription Module
- Recommendation Module (nearest-technician matching)
- Analytics Module
- Real-time Chat (SignalR upgrade of the existing Chat module)
- Live Tracking Module

---

**Document Version:** 2.0
**Status:** Draft — added Payment, Coupon, Technician Earning/Payout/Wallet, Complaint, FavoriteTechnician, Chat, and Notification modules; enumerations section completed against actual `Osta.Domain` types
