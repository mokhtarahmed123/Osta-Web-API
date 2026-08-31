# Software Requirements Specification (SRS)

# Osta Plus

**Home Services Marketplace Platform**

---

## Table of Contents

1. [Introduction](#1-introduction)
2. [Project Overview](#2-project-overview)
3. [Project Objectives](#3-project-objectives)
4. [User Roles](#4-user-roles)
5. [Functional Requirements](#5-functional-requirements)
6. [Non-Functional Requirements](#6-non-functional-requirements)
7. [Business Rules](#7-business-rules)
8. [Use Cases](#8-use-cases)
9. [Solution Structure & Implementation Status](#9-solution-structure--implementation-status)

---

## 1. Introduction

### 1.1 Purpose

This Software Requirements Specification (SRS) defines the functional and non-functional requirements for **Osta Plus**, a home services marketplace platform. It serves as a reference for developers, testers, project managers, and stakeholders throughout the software development lifecycle.

### 1.2 Scope

Osta Plus is a web-based platform that connects customers with skilled technicians across different service categories such as plumbing, electrical work, carpentry, painting, air conditioning maintenance, and more.

The platform enables customers to search for qualified technicians, book services, track service requests, make payments (including coupon-based discounts and refunds), and leave reviews. Technicians can manage their profiles, receive booking requests, update job statuses, track their earnings, request payouts, and build their professional reputation through customer ratings.

Administrators are responsible for managing users, technicians, service categories, bookings, payments, coupons, technician payouts, and monitoring the overall system.

---

## 2. Project Overview

### 2.1 Problem Statement

Customers often struggle to find reliable and skilled technicians quickly, while technicians face difficulties reaching new customers in an organized and trustworthy way. Most service requests rely on personal recommendations or social media, making the process time-consuming and unreliable.

### 2.2 Proposed Solution

Osta Plus is a platform that connects customers with skilled technicians in a fast, secure, and organized manner. Customers can search for technicians based on specialization, location, ratings, availability, and pricing. They can book services, monitor booking status, make payments, apply discount coupons, and submit reviews after service completion.

For technicians, the platform provides an opportunity to showcase their skills, receive booking requests, manage their schedules, track their earnings from completed jobs, and request payouts through their preferred payment method.

---

## 3. Project Objectives

The main objectives of Osta Plus are:

- Connect customers with skilled and verified technicians.
- Provide a fast and reliable way to book home services.
- Help technicians expand their customer base and manage service requests efficiently.
- Improve service quality through ratings and customer reviews.
- Provide a secure authentication and authorization system.
- Enable real-time booking management and service tracking.
- Provide a secure, auditable payment and payout pipeline for both customers and technicians.
- Support scalable, maintainable, and secure backend architecture.
- Deliver a modern RESTful API following industry best practices.

---

## 4. User Roles

### 4.1 Customer

A customer can:
- Register and log in (including Google login).
- Manage their profile.
- Search for technicians.
- View technician profiles.
- Add/remove technicians to/from favorites.
- Book services.
- Track booking status.
- Make payments via Stripe.
- Apply coupon codes for discounts.
- View payment history.
- Rate and review completed services.
- View booking history.
- File complaints related to a booking.
- Chat with the assigned technician.

### 4.2 Technician

A technician can:
- Register and create a professional profile.
- Add provided services and service areas.
- Set availability.
- Accept or reject booking requests.
- Update booking status.
- Manage completed jobs (with overlap-prevention on appointment scheduling).
- View earnings per completed booking.
- Request a payout (Vodafone Cash / Bank Transfer / InstaPay) and track its status.
- Cancel a pending payout request.
- Receive customer reviews.
- Receive notifications (e.g., payout completed) via email.

### 4.3 Administrator

An administrator can:
- Manage customers and technicians.
- Verify technician accounts.
- Manage service categories.
- Manage bookings.
- Monitor payments and process refunds.
- Create, update, deactivate, and bulk-generate discount coupons.
- Review, approve/reject, and complete technician payout requests.
- Handle customer complaints.
- Manage platform settings.
- View reports and analytics.

---

## 5. Functional Requirements

The system shall provide the following functionalities:

### 5.1 Authentication & Authorization — *Implemented*
- User registration.
- User login (including Google OAuth login).
- JWT authentication.
- Refresh tokens.
- Role-based authorization.
- Password reset / email confirmation.

### 5.2 Customer Management — *Partially implemented*
- Manage customer profiles.
- Update personal information.
- Manage addresses.

### 5.3 Technician Management — *Implemented*
- Technician profile management (CRUD, verify, reject).
- Service assignment management.
- Service area assignment management.
- Availability management.

### 5.4 Booking Management — *Implemented*
- Create booking.
- Cancel booking (customer).
- Accept/confirm or reject/refuse booking (technician).
- Mark booking as complete.
- Booking history (`my-bookings`).
- Appointment scheduling with technician double-booking (overlap) prevention.
- Appointment approve/reject flow.

### 5.5 Service Management — *Implemented*
- Create categories.
- Create services.
- Search services.
- Filter services / service areas.

### 5.6 Payment Management — *Implemented*
- Create Stripe Payment Intent (`create-intent`).
- Stripe webhook handling for payment confirmation (`payment_intent.succeeded` / `payment_intent.payment_failed`).
- Coupon validation and discount application at payment time.
- Refund processing (`refund`), restricted to completed payments.
- Customer payment history (`my-payments`).
- Automatic technician earning calculation (gross amount, platform fee, net amount) on successful payment.

### 5.7 Coupon & Discount Management — *Implemented*
- Create a single coupon (percentage or fixed-amount discount).
- Bulk-generate coupons with auto-generated codes.
- Update / soft-delete (deactivate) a coupon.
- Retrieve a coupon by ID or by code.
- List all coupons (filterable by active status).
- Apply a coupon to a given amount (validates activity window, usage limit, and per-user usage).
- Coupon usage tracking (`CouponUsages`) recorded upon successful payment, preventing duplicate use by the same customer.

### 5.8 Technician Earnings & Payout Management — *Implemented*
- Automatic earning record creation (gross amount, platform fee, net amount) per completed payment.
- Technician wallet balance tracking.
- Technician payout request (amount, method: Vodafone Cash / Bank Transfer / InstaPay, receiving details) with available-balance validation (accounting for pending requests).
- Technician can cancel their own pending payout request.
- Admin can reject a pending payout request (with reason) or mark it as completed.
- Admin can list all pending payout requests; technician can list their own payout history.
- Email notification to technician upon payout completion (via RabbitMQ consumer).

### 5.9 Review Management — *Implemented*
- Add, update, delete reviews.
- View a review by ID.
- View own submitted reviews (customer) and received reviews (technician).

### 5.10 Complaint Management — *Implemented*
- Submit, update, delete a complaint linked to a booking.
- View own complaints (customer) and complaints per booking.
- Update complaint status (admin).

### 5.11 Favorites Management — *Implemented*
- Add/remove a technician to/from favorites.
- View own favorite technicians list.

### 5.12 Chat — *Implemented (REST + SignalR)*
- Send a message linked to a booking (REST).
- Retrieve chat history for a booking (REST).
- Real-time message delivery via a SignalR Hub.

### 5.13 Notification Management — *Partially implemented*
- Payout-completed email notifications via RabbitMQ consumer.
- Booking / status-update notifications — *not yet implemented*.

### 5.14 Background Jobs — *Implemented*
- Recurring Hangfire job for upcoming-appointment reminders (runs on a fixed interval, flags `ReminderSent` to avoid duplicate sends).

---

## 6. Non-Functional Requirements

### 6.1 Performance
- Fast API response times.
- Efficient database queries.
- Caching using Redis — *implemented for Category/Service read endpoints*.

### 6.2 Security
- JWT authentication.
- Role-based authorization.
- Password hashing.
- HTTPS communication.
- Input validation (FluentValidation).
- Stripe webhook signature verification.

### 6.3 Scalability
- Modular architecture.
- CQRS pattern.
- MediatR.
- RabbitMQ for asynchronous messaging (notification delivery).

### 6.4 Reliability
- Exception handling.
- Logging.
- Backup and recovery support.
- Idempotency safeguards for payment webhooks — *recommended follow-up, not yet implemented*.

### 6.5 Maintainability
- Clean Architecture.
- SOLID principles.
- Dependency Injection.
- Code documentation.

### 6.6 Availability
- High system availability.
- Monitoring and health checks — *planned*.

---

## 7. Business Rules

- A technician cannot accept two bookings at overlapping times. — *Implemented at the Appointment creation/update level.*
- Customers cannot review incomplete bookings.
- Payments can only be processed for accepted bookings.
- Only verified technicians can receive booking requests.
- A completed booking cannot be modified.
- Customers can only cancel bookings before the technician starts the service.
- Ratings are allowed only after successful service completion.
- A coupon can only be used once per customer, and only within its active date range and usage limit.
- A payout request cannot exceed the technician's available wallet balance minus any pending payout requests.
- A payout request can only be cancelled by its owning technician, and only while still pending.
- Only completed payments can be refunded.

> These rules form a core part of the project, as they will later translate into validation logic and business rules within the Command Handlers. See **Business Rules.md** for the full, numbered list (BR-001 → BR-049).

---

## 8. Use Cases

### 8.1 Customer
- Register
- Login (including Google login)
- Search Technician
- View Technician Details
- Add/Remove Favorite Technician
- Book Service
- Cancel Booking
- Pay for Service
- Apply Coupon Code
- View Payment History
- Review Technician
- File Complaint
- Chat with Technician

### 8.2 Technician
- Accept Booking
- Reject Booking
- Start Service
- Complete Service
- View Earnings
- Request Payout
- Cancel Payout Request
- View Payout History

### 8.3 Admin
- Verify Technician
- Manage Categories
- Manage Users
- Manage Coupons (create, bulk-generate, update, deactivate)
- Process Refund
- Approve / Reject / Complete Technician Payout
- Handle Complaints
- View Reports

---

## 9. Solution Structure & Implementation Status

The solution is currently implemented as **10+ projects** (see `System-Architecture.md` for the full dependency diagram):

`Osta.API`, `Osta.Booking`, `Osta.Core`, `Osta.Domain`, `Osta.Identity`, `Osta.Infrastructure`, `Osta.Notification`, `Osta.Payment`, `Osta.Service`, `Osta.SharedKernel`, `Osta.Test`

Live Swagger surface today covers: **Authentication, Authorization, Role, Category, Service, ServiceArea, Technician** (profile, verification, services, service areas, availability, search), **Appointment, Booking, Chat, Complaint, Coupon, FavoriteTechnician, MediaBooking, Payment, Payout, Review, TechnicianAvailability, TechnicianService, TechnicianServiceArea, TechnicianVerification, Wallet**.

| Module | Status | Notes |
|---|---|---|
| Identity (Auth, Authorization, Role) | ✅ Implemented | SignUp, LogIn, Logout, email confirmation, reset password, refresh token, Google login, role/permission management |
| Category / Service / ServiceArea | ✅ Implemented | Full CRUD + pagination + search |
| Technician | ✅ Implemented | Profile CRUD, verify/reject, service assignment, service areas, availabilities, search, pagination, rating filter |
| Booking | ✅ Implemented | Create, confirm/refuse, cancel, complete, history |
| Appointment | ✅ Implemented | CRUD, approve/reject, overlap-prevention business rule |
| Payment | ✅ Implemented | Stripe Payment Intent, webhook, refund, payment history |
| Coupon | ✅ Implemented | CRUD, bulk generation, apply, usage tracking |
| Payout / Wallet | ✅ Implemented | Request, cancel, reject, complete, balance check, email notification |
| Review | ✅ Implemented | CRUD, own/technician views |
| Complaint | ✅ Implemented | CRUD, status update, per-booking view |
| FavoriteTechnician | ✅ Implemented | Add/remove/list |
| Chat | ✅ Implemented | REST send/history + SignalR Hub for real-time delivery |
| Notification | 🚧 Partially implemented | Payout-completion email via RabbitMQ consumer; booking/status notifications not yet built |
| Background Jobs | ✅ Implemented | Hangfire recurring appointment-reminder job |
| Redis Caching | ✅ Implemented | Category/Service read endpoints |
| Localization (AR/EN) | ⏳ Not started | Planned |
| Live Tracking | ⏳ Not started | Planned |

---

## 10. Known Follow-Ups / Technical Debt

- **Webhook idempotency:** Stripe may redeliver the same event; the webhook handler should guard against processing the same `event.id` twice.
- **`GET /Service/Satrt`:** endpoint name is a typo for `Start` — recommended rename for API consistency (breaking change, coordinate with any consumers).
- **Unit test coverage:** priority handlers pending automated tests — `ApplyCouponCommandHandler`, `RequestPayoutCommandHandler`, `CreatePaymentIntentCommandHandler`, `HandleStripeWebhookCommandHandler`, appointment-overlap validation.
- **Role-based endpoint audit:** confirm `[Authorize(Roles = ...)]` is correctly applied across all Coupon, Payment, and Payout admin/technician-only endpoints.

---

**Document Version:** 3.0
**Status:** Draft — updated to reflect Payment, Coupon, Payout/Wallet, Appointment overlap rule, and Notification implementation
