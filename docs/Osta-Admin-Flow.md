# Osta Platform – Admin Flow

توثيق لرحلة الـ Admin ومسؤولياته الأساسية في إدارة المنصة.

## 1. Flowchart

```mermaid
flowchart TD
    A[Admin Login] --> B[Admin Dashboard]

    B --> C[Manage Users]
    C --> C1[View Customers List]
    C --> C2[View Technicians List]
    C --> C3[Activate / Deactivate Account]
    C --> C4[Delete User - if no active bookings]

    B --> D[Technician Approval]
    D --> D1[View Pending Technicians]
    D1 --> D2[Review Uploaded Documents]
    D2 --> D3{Decision}
    D3 -->|Approve| D4[Technician Status: Verified]
    D3 -->|Reject| D5[Technician Status: Rejected + Reason]
    D4 --> D6[Notify Technician]
    D5 --> D6

    B --> E[Manage Categories]
    E --> E1[Add Category]
    E --> E2[Edit Category]
    E --> E3[Delete / Deactivate Category]

    B --> F[Manage Services]
    F --> F1[Add Service under Category]
    F --> F2[Edit Service - Price / Description]
    F --> F3[Delete / Deactivate Service]

    B --> G[Manage Bookings]
    G --> G1[View All Bookings]
    G --> G2[Filter by Status / Date / Technician]
    G --> G3[View Booking Details]
    G --> G4[Manually Cancel Booking - edge cases]

    B --> H[Manage Complaints]
    H --> H1[View Complaints List]
    H1 --> H2[Review Complaint Details]
    H2 --> H3{Resolution}
    H3 -->|Resolve| H4[Mark as Resolved + Response]
    H3 -->|Escalate| H5[Escalate / Take Action on User]
    H4 --> H6[Notify Customer]
    H5 --> H6

    B --> I[Reports & Statistics]
    I --> I1[Total Customers / Technicians]
    I --> I2[Total Bookings - by Status]
    I --> I3[Revenue Reports]
    I --> I4[Most Requested Services]
    I --> I5[Average Ratings]
```

## 2. Notes / Business Rules

- الـ Admin هو المسؤول الوحيد عن اعتماد أو رفض حسابات الفنيين بناءً على المستندات المرفوعة (بطاقة الرقم القومي / رخصة / شهادات).
- مايقدرش يمسح فني أو عميل عنده Bookings نشطة (Active) — لازم الطلبات تخلص أو تتلغي الأول.
- إدارة التصنيفات (Categories) والخدمات (Services) هي الأساس اللي بيبني عليه الـ Customer اختياراته في الـ Browse Flow.
- الشكاوى (Complaints) بتتجمع من العملاء والفنيين، والـ Admin بيراجعها ويقرر يحلها مباشرة أو يصعّدها (زي إيقاف حساب فني مثلاً).
- الـ Reports & Statistics بتتغذى من بيانات الـ Bookings والـ Payments والـ Reviews، وممكن تتحدث بشكل Real-time أو عن طريق Background Job/Scheduled Job.
- ممكن لاحقًا تضاف صلاحية Sub-role زي "Support" يكون له صلاحيات محدودة (مثلاً Complaints بس) من غير Full Admin Access.
