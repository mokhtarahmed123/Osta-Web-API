# Osta Platform – Technician Flow

توثيق لرحلة الفني (Technician) من الـ Signup لحد استلام الأرباح والرد على التقييمات.

## 1. Flowchart

```mermaid
flowchart TD
    A[Technician Signup] --> B{Signup Method}
    B -->|Email + Password| C[Send Confirmation Email]
    B -->|Google / Facebook OAuth| D[Account Auto-Confirmed]
    C --> E[Technician Clicks Confirmation Link]
    E --> F[Email Verified]
    D --> F

    F --> G[Upload Verification Documents]
    G --> G1[National ID]
    G --> G2[License / Certificates]
    G1 --> H[Account Status: Pending Admin Approval]
    G2 --> H

    H --> I{Admin Review}
    I -->|Rejected| J[Notify Technician: Rejected + Reason]
    J --> G
    I -->|Approved| K[Account Verified]

    K --> L[Complete Profile]
    L --> L1[Select Specializations]
    L --> L2[Select Work Areas / Cities]
    L1 --> M[Technician Goes Online / Available]
    L2 --> M

    M --> N[(RabbitMQ: Consume BookingRequested Event)]
    N --> O[Receive Booking Request Notification]
    O --> P{Technician Decision}
    P -->|Reject| Q[Booking Returned to Pool / Next Technician]
    P -->|Accept| R{Appointment Date}
    R -->|Technician Proposes Date| S[Wait for Customer Approval]
    R -->|Accept Customer's Proposed Date| T[Appointment Confirmed]
    S --> T

    T --> U[Update Task Status: On The Way]
    U --> V[Update Task Status: Started]
    V --> W[Perform the Service]
    W --> X[Upload Before / After Photos - optional]
    X --> Y[Mark Booking as Completed]

    Y --> Z[(Booking Status Updated in DB)]
    Z --> AA[Earnings Updated]
    AA --> AB[View Earnings Dashboard]
    Z --> AC[Customer Leaves a Review]
    AC --> AD[Technician Responds to Review]
```

## 2. Booking Status – State Diagram (Technician Side)

```mermaid
stateDiagram-v2
    [*] --> Pending
    Pending --> Accepted: Technician Accepts
    Pending --> Rejected: Technician Rejects
    Accepted --> OnTheWay
    OnTheWay --> Started
    Started --> Completed
    Completed --> Rated
    Pending --> Cancelled: Customer Cancels
    Accepted --> Cancelled: Cancelled Before Start
    Rejected --> [*]
    Cancelled --> [*]
    Rated --> [*]
```

## 3. Notes / Business Rules

- الفني منيتفعّلش أوتوماتيك بعد الـ Signup — لازم يرفع المستندات (بطاقة الرقم القومي + رخصة/شهادات) ويستنى موافقة الـ Admin.
- لو الـ Admin رفض، الفني بيتبلّغ بالسبب ويقدر يرفع مستندات تاني (يرجع لخطوة الـ Upload).
- بعد الموافقة، الفني لازم يحدد التخصصات (Specializations) ومناطق العمل (Work Areas) قبل ما يظهر في نتائج البحث للعملاء.
- الفني بيستقبل طلبات الحجز عن طريق RabbitMQ Consumer (نفس الـ Event اللي اتنشر من الـ Booking Service وقت الـ Booking Request).
- **Business Rule مهمة:** الفني مايقدرش يقبل طلبين (Bookings) في نفس التوقيت (Overlapping Time Slot) — لازم يتعمل Check على مواعيده الحالية قبل الـ Accept.
- تحديث حالة المهمة (On The Way → Started → Completed) بيحصل من الفني نفسه، وده اللي بيغذي حالة الـ Booking في قاعدة البيانات.
- بعد الإكمال، الأرباح بتتحدث تلقائي في حساب الفني، ويقدر يشوفها في Earnings Dashboard.
- بعد ما العميل يقيّم الخدمة، الفني يقدر يرد على التقييم (Reply on Review).
