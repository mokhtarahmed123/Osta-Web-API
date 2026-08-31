# Osta Platform – Customer Booking Flow

توثيق لرحلة المستخدم (Customer Journey) من الـ Signup لحد الـ Booking History، شامل نقطة الـ RabbitMQ.

## 1. Flowchart

```mermaid
flowchart TD
    A[User Signup] --> B{Signup Method}
    B -->|Email + Password| C[Send Confirmation Email]
    B -->|Google / Facebook OAuth| D[Account Auto-Confirmed]
    C --> E[User Clicks Confirmation Link]
    E --> F[Account Activated]
    D --> F

    F --> G[Browse Categories]
    G --> H[Select Category]
    H --> I[View Services in Category]
    I --> J[Select Service]
    J --> K[Click Book Service]

    K --> L[(RabbitMQ: Publish BookingRequested Event)]
    L --> M[Notify Available Technicians]
    M --> N[Technician Reviews Request]

    N --> O{Who Proposes the Date}
    O -->|Customer proposes date| P[Technician Approves / Rejects]
    O -->|Technician proposes date| Q[Customer Approves / Rejects]
    P --> R[Appointment Confirmed]
    Q --> R

    R --> S{Address}
    S -->|Custom address entered for this booking| T[Use Entered Address]
    S -->|No custom address| U[Use Default Saved Address]
    T --> V[Technician Heads to Location]
    U --> V

    V --> W[Technician Performs the Service]
    W --> X[Technician Marks Booking as Completed]
    X --> Y[(Booking Status Updated in Bookings Table)]
    Y --> Z[(Record Archived to BookingHistory Table)]

    Z --> AA{Any Issue?}
    AA -->|Yes| AB[Customer Submits a Complaint]
    AA -->|No| AC[Flow Ends Successfully]
```

## 2. RabbitMQ Sequence (Booking Event)

```mermaid
sequenceDiagram
    participant Customer
    participant BookingService as Booking Service (Producer)
    participant RabbitMQ
    participant TechnicianService as Technician/Notification Service (Consumer)
    participant Technician

    Customer->>BookingService: Click "Book Service"
    BookingService->>BookingService: Create Booking (Status: Pending)
    BookingService->>RabbitMQ: Publish "BookingRequested" Event
    RabbitMQ-->>TechnicianService: Consume Event
    TechnicianService->>Technician: Send Notification (New Booking Request)
    Technician->>TechnicianService: Accept + Propose/Confirm Date
    TechnicianService->>BookingService: Update Booking (Status: Confirmed)
    BookingService->>Customer: Notify Booking Confirmed
```

## 3. Notes / Assumptions

- الافتراض إن الـ Signup ممكن يبقى عن طريق Email/Password (محتاج Email Confirmation) أو عن طريق Google/Facebook OAuth (يبقى الحساب Verified تلقائيًا).
- الـ RabbitMQ بتشتغل في لحظة الـ Booking Request (Producer = Booking Service, Consumer = Technician/Notification Service) عشان تبلّغ الفنيين المتاحين من غير ما الـ Booking Service يستنى Response مباشر (Async communication).
- الـ Appointment ممكن يتحدد من الكستمر أو من التكنيكال، والطرف التاني يوافق أو يرفض.
- العنوان: لو الكستمر دخل عنوان مخصص للحجز ده يتستخدم، لو مفيش يترجع للعنوان الافتراضي المتخزن في البروفايل بتاعه.
- بعد ما التكنيكال يخلص الشغل، هو اللي يأكد الإتمام في الـ Booking Table، وبعدين السجل يترحّل لجدول الـ BookingHistory.
- في الآخر الكستمر ممكن يفتح Complaint لو حصلت مشكلة.

> ملاحظة: الملف ده Markdown عادي وفيه Mermaid diagrams بتترسم أوتوماتيك على GitHub من غير أي إعدادات إضافية.
