# Osta Plus Business Rules

*Aligned with the current solution structure: `Osta.API`, `Osta.Booking`, `Osta.Core`, `Osta.Domain`, `Osta.Identity`, `Osta.Infrastructure`, `Osta.Notification` (+ `Osta.Notification.Worker`), `Osta.Payment`, `Osta.Service`, `Osta.SharedKernel`, `Osta.Test`.*

---

# 1. User Management — *Implemented (Osta.Identity)*

## BR-001
Every user must register before accessing the system.

## BR-002
Each user must have a unique email address.

## BR-003
Passwords must be securely hashed and never stored in plain text.

## BR-004
Only authenticated users can access protected resources.

## BR-005
Users can only update their own profile information.

---

# 2. Technician Management — *Implemented (Osta.Service)*

## BR-006
A technician must complete their profile before becoming available to customers.

## BR-007
A technician account must be verified by an administrator before becoming available for booking.

## BR-008
Administrators may approve or reject technician verification requests.

## BR-009
A technician can provide one or more services.

## BR-010
A technician can serve one or more service areas.

## BR-011
A technician can define weekly working availability.

## BR-012
A technician cannot have duplicate availability records for the same day and time period.

## BR-013
Only verified technicians can receive booking requests.

## BR-014
A technician cannot accept two bookings/appointments scheduled at overlapping times. — *Implemented at Appointment create/update time via an overlap query (`ScheduledStart < newEnd AND ScheduledEnd > newStart`), scoped to the technician's active (non-cancelled/rejected) bookings.*

---

# 3. Service Management — *Implemented (Osta.Service)*

## BR-015
Every service must belong to an existing category.

## BR-016
Only administrators can create, update, or delete services.

## BR-017
Only administrators can create, update, or delete service categories.

## BR-018
A service category cannot be deleted while it contains active services.

## BR-019
A technician cannot be assigned the same service more than once.

---

# 4. Service Area Management — *Implemented (Osta.Service)*

## BR-020
Only existing service areas can be assigned to technicians.

## BR-021
A technician cannot be assigned the same service area more than once.

---

# 5. Technician Availability Management — *Implemented (Osta.Service)*

## BR-022
Availability must belong to an existing technician.

## BR-023
Availability must contain a valid day of the week.

## BR-024
The start time must be earlier than the end time.

## BR-025
Start time and end time cannot be equal.

## BR-026
Only the owner technician can create, update, or delete their availability.

---

# 6. Address Management — *Not yet implemented*

## BR-027
Each address must belong to a valid customer.

## BR-028
Customers may own multiple addresses.

---

# 7. Booking & Appointment Management — *Implemented (Osta.Booking)*

## BR-029
Only authenticated customers can create bookings.

## BR-030
A booking must reference an existing technician.

## BR-031
A booking must reference an existing service.

## BR-032
Customers cannot create bookings in the past.

## BR-033
Each booking must have exactly one status.

## BR-034
Completed bookings cannot be modified.

## BR-035
Customers can only cancel bookings before the technician starts the service.

## BR-035a *(new)*
An appointment cannot be created or rescheduled to a time slot that overlaps with another active appointment for the same technician. *(See BR-014.)*

## BR-035b *(new)*
A booking is marked complete only via the dedicated `complete` action; a completed booking's associated appointment cannot be further modified.

---

# 8. Payment Management — *Implemented (Osta.Payment + Osta.Core)*

## BR-036
Payments are only allowed for accepted bookings.

## BR-037
Every payment must belong to exactly one booking.

## BR-038
Refund requests follow the platform cancellation policy; only payments with `Status = Completed` may be refunded.

## BR-038a *(new)*
A booking with an existing payment cannot be re-charged while that payment is still `Completed`; a new Payment Intent may only be created if no completed payment exists for the booking, or the prior attempt is `Pending`/`Failed`.

## BR-038b *(new)*
Payment status transitions are driven exclusively by verified Stripe webhook events (`payment_intent.succeeded`, `payment_intent.payment_failed`) — the signature/HMAC on each webhook request must be validated before any state change is applied.

## BR-038c *(new)*
Upon a successful payment, the system automatically calculates and records the technician's earning for that booking (gross amount, platform fee, net amount).

---

# 9. Coupon & Discount Management — *Implemented (Osta.Core)*

## BR-038d *(new)*
A coupon may only be applied while `IsActive = true` and the current date falls within `[StartDate, EndDate]`.

## BR-038e *(new)*
A coupon cannot be applied once its `UsedCount` reaches `UsageLimit` (unless `UsageLimit = 0`, meaning unlimited).

## BR-038f *(new)*
A coupon can be used at most once per customer, enforced both at the application layer (pre-check) and at the database layer (unique index on `CouponUsages.(CouponId, UserId)`).

## BR-038g *(new)*
The discount applied can never exceed the original payment amount (i.e. the final amount can never go below zero).

## BR-038h *(new)*
Coupon usage is only recorded (and `UsedCount` incremented) after the associated payment is confirmed successful — not at the time the coupon is merely validated/quoted.

---

# 10. Technician Earnings & Payout Management — *Implemented (Osta.Service + Osta.Core)*

## BR-038i *(new)*
A technician's available balance for payout purposes equals total net earnings minus the sum of all `Completed` and currently `Pending` payout amounts.

## BR-038j *(new)*
A payout request amount cannot exceed the technician's available balance.

## BR-038k *(new)*
A technician cannot submit a new payout request while another pending request for the exact same amount already exists.

## BR-038l *(new)*
Only the requesting technician may cancel their own payout request, and only while it is still `Pending`.

## BR-038m *(new)*
Only an administrator may reject (with a mandatory reason) or complete a pending/approved payout request.

## BR-038n *(new)*
Actual fund transfer to the technician occurs manually, outside the system, using the `Method` and `ReceivingDetails` supplied with the request; the system's role is limited to tracking, validation, and audit.

---

# 11. Review Management — *Implemented (Osta.Service)*

## BR-039
Customers can submit only one review per completed booking.

## BR-040
Reviews are allowed only after the booking has been completed.

## BR-041
Only the customer who created the booking may submit a review.

---

# 12. Complaint Management — *Implemented*

## BR-041a *(new)*
A complaint must reference an existing booking.

## BR-041b *(new)*
Only an administrator may change a complaint's status.

---

# 13. Notification Management — *Partially implemented*

## BR-042
The system should notify technicians and customers about important events.

## BR-043
Notifications should be generated for:
- Booking Created
- Booking Accepted
- Booking Rejected
- Booking Cancelled
- Booking Completed
- Technician Verification
- Payment Status Updates
- **Payout Completed** *(implemented — delivered via RabbitMQ + email)*
- **Upcoming Appointment Reminder** *(implemented — Hangfire recurring job, `ReminderSent` flag prevents duplicates)*

---

# 14. Security — *Implemented (Osta.Identity / Osta.API)*

## BR-044
Role-based authorization must be enforced throughout the system.

## BR-045
JWT tokens must be validated before accessing protected endpoints.

## BR-046
Sensitive information must only be transmitted over HTTPS.

## BR-046a *(new)*
Administrative and financial actions (coupon management, refunds, payout approval/rejection/completion) must be restricted to the `Admin` role; payout request/cancel must be restricted to the `Technician` role.

---

# 15. Audit & Logging — *Partially implemented*

## BR-047
Important business operations must be logged.

## BR-048
Verification, rejection, create, update, and delete operations should be recorded.

## BR-049
System errors must be logged without exposing sensitive information to users.

---

# 16. Future Enhancements

The following business rules may be introduced in future releases:

- Loyalty Programs
- Subscription Plans
- Promotional Campaigns
- Dynamic Pricing
- Smart Technician Recommendation (Haversine-based nearest-technician matching)
- Advanced Scheduling Policies
- Recurring Bookings
- Technician Performance Metrics / Gamification badges
- Stripe webhook idempotency (dedupe by `event.id`)

---

**Document Version:** 3.0
**Status:** Draft — added Payment (BR-036–038c), Coupon (BR-038d–038h), Payout/Wallet (BR-038i–038n), Appointment overlap (BR-035a/b), and Complaint (BR-041a/b) rule groups, all cross-checked against the implemented handlers
