# Osta Platform – Feature Flows (Mini Diagrams)

Flow مبسط وسريع لكل ميزة من المقترحات، لتوضيح الفكرة بس من غير تفاصيل زيادة.

---

## 1. Coupons & Discounts

```mermaid
flowchart TD
    A[Customer Enters Coupon Code] --> B{Valid & Not Expired?}
    B -->|No| C[Show Error]
    B -->|Yes| D[Apply Discount to Total]
    D --> E[Proceed to Payment]
```

---

## 2. Favorites

```mermaid
flowchart TD
    A[Customer Views Technician Profile] --> B[Click Add to Favorites]
    B --> C[(Save in Favorites Table)]
    C --> D[Shown in My Favorites List]
    D --> E[Quick Book Next Time]
```

---

## 3. Push Notifications

```mermaid
flowchart TD
    A[Event Happens - e.g. Booking Accepted] --> B[Publish to RabbitMQ]
    B --> C[Notification Service Consumes Event]
    C --> D[Call Firebase Cloud Messaging]
    D --> E[Notification Delivered to Device]
```

---

## 4. Localization (AR/EN)

```mermaid
flowchart TD
    A[Request with Accept-Language Header] --> B{AR or EN?}
    B -->|AR| C[Return Arabic Strings]
    B -->|EN| D[Return English Strings]
    C --> E[Response Sent]
    D --> E
```

---

## 5. Real-time Chat (SignalR)

```mermaid
flowchart TD
    A[User Opens Chat] --> B[Connect to SignalR Hub]
    B --> C[Send Message]
    C --> D[Hub Broadcasts to Other Party]
    D --> E[Message Saved in DB]
    E --> F[Instant Delivery]
```

---

## 6. Live Tracking (Technician Location)

```mermaid
flowchart TD
    A[Technician App Sends GPS Update] --> B[SignalR Hub Receives Location]
    B --> C[Broadcast to Customer]
    C --> D[Customer Map Updates Live]
```

---

## 7. Recommendation Engine (Nearest Technician)

```mermaid
flowchart TD
    A[Customer Requests Service] --> B[Get Available Technicians in Area]
    B --> C[Calculate Distance - Haversine]
    C --> D[Sort by Distance + Rating]
    D --> E[Show Top Matches]
```

---

## 8. Wallet System

```mermaid
flowchart TD
    A[Customer Tops Up Wallet] --> B[(Wallet Balance Updated)]
    B --> C[Customer Books a Service]
    C --> D{Balance Sufficient?}
    D -->|Yes| E[Deduct from Wallet]
    D -->|No| F[Fallback to Card Payment]
```

---

## 9. Redis Caching

```mermaid
flowchart TD
    A[Request for Categories/Services] --> B{Cached in Redis?}
    B -->|Yes| C[Return Cached Data - Fast]
    B -->|No| D[Query Database]
    D --> E[Store in Redis]
    E --> C
```

---

## 10. Outbox Pattern (Reliable Messaging)

```mermaid
flowchart TD
    A[Save Booking + Outbox Row - Same Transaction] --> B[(Outbox Table)]
    B --> C[Background Worker Polls Outbox]
    C --> D[Publish Event to RabbitMQ]
    D --> E[Mark Row as Processed]
```

---

## 11. CI/CD Pipeline

```mermaid
flowchart TD
    A[Push Code to GitHub] --> B[GitHub Actions Triggered]
    B --> C[Build + Run Tests]
    C --> D{Tests Passed?}
    D -->|No| E[Fail Pipeline + Notify]
    D -->|Yes| F[Build & Push Docker Image]
    F --> G[Deploy]
```

---

## 12. Rate Limiting / API Gateway

```mermaid
flowchart TD
    A[Client Sends Request] --> B[API Gateway]
    B --> C{Within Rate Limit?}
    C -->|No| D[Return 429 Too Many Requests]
    C -->|Yes| E[Forward to Target Service]
```

---

## 13. Background Jobs - Appointment Reminders (Hangfire)

```mermaid
flowchart TD
    A[Hangfire Scheduled Job Runs] --> B[Check Upcoming Appointments]
    B --> C{Within 1 Hour?}
    C -->|Yes| D[Send Reminder Notification]
    C -->|No| E[Skip]
```

---

## 14. Gamification (Technician Badges)

```mermaid
flowchart TD
    A[Booking Completed + Rated] --> B[Update Technician Stats]
    B --> C{Meets Badge Criteria?}
    C -->|Yes| D[Award Badge - e.g. Top Rated]
    C -->|No| E[No Change]
    D --> F[Badge Shown on Profile]
```
