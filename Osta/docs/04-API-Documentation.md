
# 04 — API Documentation

## Osta Plus — REST API Reference

---

## Table of Contents

1. [Conventions](#1-conventions)
2. [Authentication](#2-authentication)
3. [Authorization](#3-authorization)
4. [Role](#4-role)
5. [Category](#5-category)
6. [Service](#6-service)
7. [ServiceArea](#7-servicearea)
8. [Technician](#8-technician)
9. [TechnicianAvailability](#9-technicianavailability)
10. [TechnicianService](#10-technicianservice)
11. [TechnicianServiceArea](#11-technicianservicearea)
12. [TechnicianVerification](#12-technicianverification)
13. [Booking](#13-booking)
14. [Appointment](#14-appointment)
15. [Payment](#15-payment)
16. [Coupon](#16-coupon)
17. [Payout](#17-payout)
18. [Wallet](#18-wallet)
19. [Review](#19-review)
20. [Complaint](#20-complaint)
21. [FavoriteTechnician](#21-favoritetechnician)
22. [MediaBooking](#22-mediabooking)
23. [Chat](#23-chat)
24. [Error Response Shape](#24-error-response-shape)

---

## 1. Conventions

- Base URL (local dev): `http://localhost:5083`
- Versioned modules use the route prefix `/api/v{version}/...` (current version: `1`). `Authentication`, `Payment`, `Payout`, `Wallet` are not versioned (`/api/...`).
- All authenticated endpoints require:
  ```
  Authorization: Bearer <jwt_access_token>
  ```
- All successful responses are wrapped in a standard envelope:
  ```json
  {
    "succeeded": true,
    "message": "string",
    "data": { }
  }
  ```
- All request/response bodies are `application/json` unless stated otherwise.
- Dates use ISO-8601 (`"2026-09-01T10:00:00Z"`); date-only fields use `"2026-09-01"`.

---

## 2. Authentication

*Base route: `/api/Authentication`* — **no version prefix**

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/SignUp` | ❌ | Register a new user (customer by default) |
| POST | `/LogIn` | ❌ | Authenticate with email/password, returns JWT + refresh token |
| POST | `/Logout` | ✅ | Invalidate the current session/refresh token |
| POST | `/SendEmail` | ❌ | Send an email confirmation link |
| GET | `/ConfirmEmail` | ❌ | Confirm email via token (query params `userId`, `token`) |
| POST | `/SendResetPassword` | ❌ | Send a password-reset email |
| GET | `/ConfirmResetPassword` | ❌ | Validate a password-reset token |
| POST | `/ResetPassword` | ❌ | Set a new password using a valid reset token |
| POST | `/RefreshToken` | ❌ | Exchange a refresh token for a new access token |
| GET | `/MyProfile` | ✅ | Get the current authenticated user's profile |
| POST | `/google-login` | ❌ | Authenticate via Google ID token |
| POST | `/facebook-login` | ❌ | Authenticate via Facebook access token |

### POST `/SignUp`
**Request**
```json
{
  "fullName": "Ahmed Mostafa",
  "email": "ahmed@example.com",
  "password": "P@ssw0rd123",
  "phoneNumber": "01012345678"
}
```
**Response**
```json
{
  "succeeded": true,
  "message": "Account created. Please confirm your email.",
  "data": { "userId": "8af469cb-d96e-4922-9467-ebed58e80038" }
}
```

### POST `/LogIn`
**Request**
```json
{ "email": "ahmed@example.com", "password": "P@ssw0rd123" }
```
**Response**
```json
{
  "succeeded": true,
  "message": "Login successful.",
  "data": {
    "accessToken": "eyJhbGciOi...",
    "refreshToken": "d2f1a7...",
    "expiresIn": 3600
  }
}
```

### POST `/google-login`
**Request**
```json
{ "idToken": "eyJhbGciOi..." }
```
**Response:** same shape as `/LogIn`.

### POST `/facebook-login`
**Request**
```json
{ "accessToken": "EAAB..." }
```
**Response:** same shape as `/LogIn`.

### POST `/RefreshToken`
**Request**
```json
{ "accessToken": "eyJhbGciOi...", "refreshToken": "d2f1a7..." }
```
**Response:** same shape as `/LogIn`.

---

## 3. Authorization

*Base route: `/api/v{version}/Authorization`* — requires `Admin` role unless noted

| Method | Route | Description |
|---|---|---|
| POST | `/AssignRole/{userId}/{roleId}` | Assign a role to a user |
| POST | `/RemoveRoleFromUser/{roleId}/{userId}` | Remove a role from a user |
| GET | `/UserIsInRole/{userId}/{roleId}` | Check whether a user has a given role → `{ "data": true }` |
| GET | `/GetUserRoles/{userId}` | List all roles assigned to a user |
| POST | `/roles/{roleId}/permissions` | Grant a permission to a role |
| GET | `/roles/{roleId}/permissions` | List a role's permissions |
| DELETE | `/roles/{roleId}/permissions/{permissionId}` | Revoke a permission from a role |
| GET | `/roles/{roleId}/permissions/{permissionId}` | Check whether a role has a given permission |
| GET | `/permissions/{permissionId}/roles` | List all roles holding a given permission |

### POST `/roles/{roleId}/permissions`
**Request**
```json
{ "permissionId": 4 }
```

---

## 4. Role

*Base route: `/api/v{version}/Role`* — `Admin` only

| Method | Route | Description |
|---|---|---|
| POST | `/` | Create a role |
| PUT | `/` | Update a role |
| GET | `/` | List all roles |
| DELETE | `/{roleId}` | Delete a role |
| GET | `/{roleId}` | Get a role by id |

### POST `/`
**Request**
```json
{ "name": "Technician" }
```

---

## 5. Category

*Base route: `/api/v{version}/Category`*

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/ping` | ❌ | Health check |
| POST | `/` | Admin | Create a category |
| GET | `/` | ❌ | List all categories *(Redis-cached)* |
| GET | `/{id}` | ❌ | Get a category by id |
| DELETE | `/{id}` | Admin | Delete a category |
| PUT | `/{id}` | Admin | Update a category |
| GET | `/Paginated` | ❌ | Paginated list — query: `pageNumber`, `pageSize` |

### POST `/`
**Request**
```json
{ "name": "Plumbing" }
```
**Response**
```json
{ "succeeded": true, "data": { "id": 3, "name": "Plumbing" } }
```

---

## 6. Service

*Base route: `/api/v{version}/Service`*

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/Satrt` | ❌ | *(typo for `Start`)* health check |
| POST | `/` | Admin | Create a service |
| GET | `/` | ❌ | List all services *(Redis-cached)* |
| GET | `/{Id}` | ❌ | Get a service by id |
| DELETE | `/{Id}` | Admin | Delete a service |
| PUT | `/{id}` | Admin | Update a service |

### POST `/`
**Request**
```json
{ "name": "Leak Repair", "price": 250, "categoryId": 3 }
```

---

## 7. ServiceArea

*Base route: `/api/v{version}/ServiceArea`*

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/ping` | ❌ | Health check |
| POST | `/` | Admin | Create a service area |
| GET | `/` | ❌ | List all service areas |
| GET | `/{id}` | ❌ | Get by id |
| DELETE | `/{id}` | Admin | Delete |
| PUT | `/{id}` | Admin | Update |

### POST `/`
**Request**
```json
{ "name": "Nasr City" }
```

---

## 8. Technician

*Base route: `/api/v{version}/Technician`*

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/Request` | Authenticated user | Submit a request to become a technician |
| GET | `/{Id}` | ❌ | Get technician profile by id |
| DELETE | `/{Id}` | Admin | Delete a technician |
| GET | `/` | ❌ | List technicians |
| PATCH | `/` | Technician | Update own profile |
| GET | `/Paginated/{pageNumber}/{pageSize}` | ❌ | Paginated technician list |
| GET | `/rate/{rate}` | ❌ | Filter technicians by minimum rating |
| GET | `/Search` | ❌ | Search technicians — query: `category`, `area`, `keyword` |
| GET | `/My-Profile` | Technician | Get the current technician's own profile |

### POST `/Request`
**Request**
```json
{
  "bio": "10 years experience in residential plumbing.",
  "nationalId": "29001011234567",
  "yearsOfExperience": 10,
  "serviceIds": [1, 4],
  "serviceAreaIds": [2, 5]
}
```

**Business rule:** the system checks the caller already exists in `AspNetUsers` before creating the linked `Technician` row (1:1 on `UserId`).

---

## 9. TechnicianAvailability

*Base route: `/api/v{version}/TechnicianAvailability`*

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/Technician/availabilities` | Technician | Add an availability slot |
| DELETE | `/Technician/availabilities/{id}` | Technician | Remove a slot |
| PATCH | `/Technician/availabilities/{id}` | Technician | Update a slot |
| GET | `/availabilities` | ❌ | List all availabilities |
| GET | `/Technician/{technicianId}/availabilities` | ❌ | List a technician's availabilities |
| GET | `/availabilities/{id}` | ❌ | Get a single availability by id |

### POST `/Technician/availabilities`
**Request**
```json
{ "dayOfWeek": "Monday", "startTime": "09:00:00", "endTime": "17:00:00" }
```
**Business rule:** `startTime` must be strictly before `endTime`; duplicate day+time-range slots for the same technician are rejected.

---

## 10. TechnicianService

*Base route: `/api/v{version}/TechnicianService`*

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/services/{serviceId}` | ❌ | List technicians offering a given service |
| POST | `/services` | Technician | Assign a service to self |

---

## 11. TechnicianServiceArea

*Base route: `/api/v{version}/TechnicianServiceArea`*

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/service-areas` | Technician | Assign a service area to self |
| PATCH | `/service-areas` | Technician | Update assigned service areas |
| DELETE | `/service-areas` | Technician | Remove a service area assignment |
| GET | `/service-areas/{serviceAreaId}` | ❌ | List technicians covering a service area |

---

## 12. TechnicianVerification

*Base route: `/api/v{version}/TechnicianVerification`* — `Admin` only

| Method | Route | Description |
|---|---|---|
| PATCH | `/{id}/verify` | Approve a technician's verification request |
| PATCH | `/{id}/reject` | Reject a technician's verification request |

### PATCH `/{id}/reject`
**Request**
```json
{ "reasonOfReject": "National ID document is unreadable." }
```

---

## 13. Booking

*Base route: `/api/v{version}/Booking`*

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/` | Customer | Create a booking |
| GET | `/technician/bookings` | Technician | List own bookings |
| GET | `/technician/bookings/{id}` | Technician | Get a specific booking |
| PATCH | `/technician/confirm/{bookingId}` | Technician | Confirm/accept a booking |
| PATCH | `/technician/Refuse/{bookingId}` | Technician | Refuse/reject a booking |
| PATCH | `/customer/Cancel/{bookingId}` | Customer | Cancel a booking (only before service starts) |
| GET | `/my-bookings` | Customer | List own booking history |
| PATCH | `/{id}/complete` | Technician | Mark a booking as completed |

### POST `/`
**Request**
```json
{
  "technicianId": 7,
  "serviceId": 4,
  "city": "Cairo",
  "governorate": "Cairo",
  "area": "Nasr City",
  "street": "Makram Ebeid",
  "bookingDate": "2026-09-05T00:00:00"
}
```

---

## 14. Appointment

*Base route: `/api/v{version}/Appointment`*

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/` | Technician | Schedule an appointment for a booking |
| GET | `/` | Authenticated | List appointments |
| PUT | `/{appointmentId}` | Technician | Reschedule an appointment |
| GET | `/{appointmentId}` | Authenticated | Get a single appointment |
| PATCH | `/{appointmentId}/approve` | Customer | Approve a proposed appointment time |
| PATCH | `/{appointmentId}/reject` | Customer | Reject a proposed appointment time |

### POST `/`
**Request**
```json
{
  "bookingId": 12,
  "scheduledStart": "2026-09-05T10:00:00",
  "scheduledEnd": "2026-09-05T12:00:00"
}
```
**Business rule (BR-014 / BR-035a):** rejected with `400 Bad Request` — *"You already have an appointment scheduled at this time."* — if the technician has any other active appointment where `ScheduledStart < newEnd AND ScheduledEnd > newStart`.

---

## 15. Payment

*Base route: `/api/Payment`* — **no version prefix**

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/create-intent` | Customer | Create a Stripe Payment Intent for a booking |
| POST | `/webhook` | ❌ *(Stripe signature verified instead)* | Stripe webhook receiver |
| POST | `/refund` | Admin | Refund a completed payment |
| GET | `/my-payments` | Customer | List own payment history |

### POST `/create-intent`
**Request**
```json
{
  "bookingId": 12,
  "amount": 850,
  "couponCode": "SAVE20"
}
```
**Response**
```json
{
  "succeeded": true,
  "data": {
    "clientSecret": "pi_3XXXX_secret_XXXX",
    "paymentIntentId": "pi_3XXXX"
  }
}
```
**Business rules:**
- If `couponCode` is supplied, it is validated (active window, usage limit, not already used by this customer) and the discount is deducted from `amount` **before** the Stripe Intent is created.
- If a `Completed` payment already exists for `bookingId`, the request is rejected — *"This booking has already been paid for."*
- If a `Pending` payment already exists, its Stripe amount is updated in place rather than creating a duplicate Intent.

### POST `/refund`
**Request**
```json
{ "paymentId": 25 }
```
**Business rule:** only payments with `Status = Completed` may be refunded.

### GET `/my-payments`
**Response**
```json
{
  "succeeded": true,
  "data": [
    {
      "id": 25,
      "bookingId": 12,
      "amount": 680,
      "status": "Completed",
      "method": "Card",
      "transactionId": "pi_3XXXX",
      "createdAt": "2026-08-25T20:34:22Z",
      "technicianName": "Mohamed Ali"
    }
  ]
}
```

---

## 16. Coupon

*Base route: `/api/v{version}/Coupon`*

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/` | Admin | Create a single coupon |
| GET | `/` | Admin | List coupons — query: `isActive` |
| POST | `/bulk` | Admin | Bulk-generate coupons with auto codes |
| PUT | `/{id}` | Admin | Update a coupon |
| DELETE | `/{id}` | Admin | Soft-deactivate a coupon (`IsActive = false`) |
| GET | `/{id}` | Admin | Get a coupon by id |
| GET | `/code/{code}` | Authenticated | Get a coupon by code |
| POST | `/apply` | Authenticated | Validate & preview a coupon's discount |

### POST `/`
**Request**
```json
{
  "code": "SAVE20",
  "discountType": 0,
  "discountValue": 20,
  "startDate": "2026-08-24",
  "endDate": "2026-09-24",
  "usageLimit": 100
}
```
`discountType`: `0 = Percentage`, `1 = FixedAmount`.

### POST `/bulk`
**Request**
```json
{
  "count": 20,
  "discountType": 0,
  "discountValue": 15,
  "startDate": "2026-08-24",
  "endDate": "2026-09-24",
  "usageLimit": 1
}
```
**Response**
```json
{ "succeeded": true, "data": ["A1B2C3D4", "E5F6G7H8", "..."] }
```

### POST `/apply`
**Request**
```json
{ "code": "SAVE20", "userId": "8af469cb-...", "originalAmount": 500 }
```
**Response**
```json
{
  "succeeded": true,
  "data": {
    "couponId": 1,
    "originalAmount": 500,
    "discountApplied": 100,
    "finalAmount": 400
  }
}
```
**Business rules:** rejected if the coupon is inactive, outside its date window, at its usage limit, or already used by this customer (BR-038d–h).

---

## 17. Payout

*Base route: `/api/Payout`* — **no version prefix**

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/request` | Technician | Request a payout |
| PUT | `/{payoutId}/cancel` | Technician (owner only) | Cancel own pending payout request |
| PUT | `/{payoutId}/reject` | Admin | Reject a pending request (with reason) |
| PUT | `/{payoutId}/complete` | Admin | Mark a request as completed |
| GET | `/my` | Technician | List own payout requests |
| GET | `/pending` | Admin | List all pending requests |
| GET | `/{payoutId}` | Admin / owning Technician | Get a single payout request |

### POST `/request`
**Request**
```json
{
  "amount": 500,
  "method": 0,
  "receivingDetails": "01012345678"
}
```
`method`: `0 = VodafoneCash`, `1 = BankTransfer`, `2 = InstaPay`.

**Business rules (BR-038i–n):**
- Rejected if `amount` exceeds `TechnicianWallet.Amount − SUM(pending payout amounts)`.
- Rejected if an identical-amount request is already `Pending`.

### PUT `/{payoutId}/reject`
**Request**
```json
{ "rejectionReason": "Receiving details could not be verified." }
```

### PUT `/{payoutId}/complete`
No body required. On success, triggers a `PayoutNotification` message on the `payout-notification` RabbitMQ queue → email sent to the technician by `Osta.Notification.Worker`.

---

## 18. Wallet

*Base route: `/api/Wallet`* — **no version prefix**

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/balance` | Technician | Get current wallet balance |

**Response**
```json
{ "succeeded": true, "data": { "amount": 1250.00 } }
```

---

## 19. Review

*Base route: `/api/v{version}/Review`*

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/` | Customer | Submit a review for a completed booking |
| GET | `/` | ❌ | List all reviews |
| PUT | `/{id}` | Customer (owner) | Update own review |
| DELETE | `/{id}` | Customer (owner) / Admin | Delete a review |
| GET | `/{id}` | ❌ | Get a review by id |
| GET | `/my` | Customer | List own submitted reviews |
| GET | `/technician/my` | Technician | List reviews received |

### POST `/`
**Request**
```json
{ "bookingId": 12, "rating": 5, "comment": "Fast and professional." }
```
**Business rule:** only allowed once the booking is `Completed`; one review per booking per customer.

---

## 20. Complaint

*Base route: `/api/v{version}/Complaint`*

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/` | Customer | File a complaint linked to a booking |
| GET | `/` | Admin | List all complaints |
| PUT | `/{id}` | Customer (owner) | Update own complaint |
| DELETE | `/{id}` | Customer (owner) / Admin | Delete a complaint |
| GET | `/{id}` | Authenticated | Get a complaint by id |
| PATCH | `/{id}/status` | Admin | Update complaint status |
| GET | `/my` | Customer | List own complaints |
| GET | `/booking/{bookingId}` | Authenticated | List complaints for a booking |

### POST `/`
**Request**
```json
{ "bookingId": 12, "description": "Technician arrived 2 hours late." }
```

---

## 21. FavoriteTechnician

*Base route: `/api/v{version}/FavoriteTechnician`*

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/my` | Customer | List own favorite technicians |
| POST | `/{technicianId}` | Customer | Add a technician to favorites |
| DELETE | `/{technicianId}` | Customer | Remove a technician from favorites |

---

## 22. MediaBooking

*Base route: `/api/v{version}/MediaBooking`*

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/` | Technician | Upload booking media (before/after photos) |
| PUT | `/{id}` | Technician (owner) | Update a media record |
| DELETE | `/{id}` | Technician (owner) / Admin | Delete a media record |
| GET | `/{id}` | Authenticated | Get a media record by id |
| GET | `/booking/{bookingId}` | Authenticated | List all media for a booking |
| GET | `/booking/{bookingId}/type/{repairType}` | Authenticated | List media filtered by repair type (Before/After) |

### POST `/`
**Request**
```json
{ "bookingId": 12, "url": "https://cdn.example.com/media/abc.jpg", "repairType": 1 }
```

---

## 23. Chat

*Base route: `/api/v{version}/Chat`* (REST) + **SignalR Hub** for real-time delivery

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/send` | Authenticated | Persist and send a message |
| GET | `/booking/{bookingId}` | Authenticated | Get chat history for a booking |

### POST `/send`
**Request**
```json
{ "bookingId": 12, "content": "I'll be there in 15 minutes." }
```

### SignalR Hub
- Connect: `wss://<host>/hubs/chat` (JWT bearer token passed via `access_token` query string or the standard Authorization header on the handshake, per the client's SignalR configuration)
- Client method invoked on new message: `ReceiveMessage(senderId, bookingId, content, sentAt)`
- REST `POST /send` and the Hub work together: sending a message persists it via the same command handler and then broadcasts it to connected clients in the relevant booking's chat group.

---

## 24. Error Response Shape

All non-2xx responses follow this shape:

```json
{
  "succeeded": false,
  "message": "Insufficient available balance. Available: 300 EGP",
  "data": null
}
```

| Status | Meaning |
|---|---|
| 400 | Bad Request — validation failure or business-rule violation |
| 401 | Unauthorized — missing/invalid JWT |
| 403 | Forbidden — authenticated but wrong role/ownership |
| 404 | Not Found |
| 500 | Unhandled server error |

---

**Document Version:** 1.0
**Status:** Draft — covers all live Swagger modules as of the current implementation; request/response examples for Booking/Appointment/Review/Complaint/MediaBooking field names are best-effort and should be verified against the actual Command/DTO definitions before external distribution.
