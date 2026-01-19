# Notification Service

SignalR microservice quản lý thông báo real-time cho hệ thống HRM.

## Mục lục

- [Kiến trúc](#kiến-trúc)
- [Công nghệ](#công-nghệ)
- [Nghiệp vụ](#nghiệp-vụ)
- [SignalR Hub API](#signalr-hub-api)
- [REST API](#rest-api)
- [CQRS Pattern](#cqrs-pattern)
- [Domain Entities](#domain-entities)
- [Database Schema](#database-schema)
- [Event-Driven Architecture](#event-driven-architecture)
- [Luồng xử lý](#luồng-xử-lý)
- [Cấu hình](#cấu-hình)
- [Chạy ứng dụng](#chạy-ứng-dụng)

---

## Kiến trúc

**Clean Architecture 4-Layer Pattern:**

```
src/
├── API/                        # Layer 1: Presentation
│   ├── BackgroundServices/     # RabbitMQ consumer
│   ├── Controllers/            # REST API endpoints
│   ├── Hubs/                   # SignalR Hub
│   └── Program.cs              # Entry point & DI configuration
│
├── Application/                # Layer 2: Business Logic
│   ├── Features/               # CQRS Commands & Queries
│   │   └── Notifications/
│   │       ├── Commands/       # CreateNotification, MarkAsRead...
│   │       ├── Queries/        # GetNotifications, GetUnreadCount
│   │       └── Dtos/           # Data Transfer Objects
│   └── Common/
│       └── Abstractions/       # Repository interfaces
│
├── Domain/                     # Layer 3: Enterprise Business Rules
│   ├── Entities/               # Notification, UserConnection...
│   └── Enums/                  # NotificationType, Priority, Channel
│
└── Infrastructure/             # Layer 4: Data Access
    └── Data/                   # DbContext, Configurations
```

---

## Công nghệ

| Công nghệ | Phiên bản | Mục đích |
|-----------|-----------|----------|
| .NET | 8.0 | Framework |
| ASP.NET Core | 8.0 | Web framework |
| SignalR | 8.0 | Real-time WebSocket communication |
| Entity Framework Core | 8.0 | ORM |
| PostgreSQL | 16 | Primary database |
| RabbitMQ | 3.x | Event consumer (từ Time Service) |
| MediatR | 12.x | CQRS pattern implementation |
| Keycloak | 23.0 | JWT Authentication & SSO |
| Serilog | - | Structured logging |

---

## Nghiệp vụ

### Quản lý thông báo

| Chức năng | Mô tả |
|-----------|-------|
| Nhận thông báo | Real-time qua SignalR |
| Xem danh sách | Liệt kê thông báo của user |
| Đánh dấu đã đọc | Mark as read (single/all) |
| Lưu trữ | Archive thông báo cũ |
| Preferences | Cấu hình nhận thông báo theo loại |

### Loại thông báo (NotificationType)

| Type | Trigger | Mô tả |
|------|---------|-------|
| `LeaveRequestCreated` | Tạo đơn nghỉ phép | Gửi tới Manager |
| `LeaveRequestApproved` | Duyệt đơn nghỉ | Gửi tới Employee |
| `LeaveRequestRejected` | Từ chối đơn nghỉ | Gửi tới Employee |
| `LeaveRequestCancelled` | Hủy đơn nghỉ | Gửi tới Manager/HR |
| `AttendanceReminder` | Nhắc chấm công | Gửi tới Employee |
| `AttendanceAbnormal` | Chấm công bất thường | Gửi tới HR |
| `OvertimeRequestCreated` | Tạo đơn tăng ca | Gửi tới Manager |
| `OvertimeRequestApproved` | Duyệt tăng ca | Gửi tới Employee |
| `OvertimeRequestRejected` | Từ chối tăng ca | Gửi tới Employee |
| `EmployeeOnboarding` | Nhân viên mới | Gửi tới HR |
| `EmployeeOffboarding` | Nhân viên nghỉ việc | Gửi tới HR |
| `BirthdayReminder` | Sinh nhật | Gửi tới Team |
| `WorkAnniversary` | Kỷ niệm làm việc | Gửi tới Team |
| `PolicyUpdate` | Cập nhật chính sách | Broadcast |
| `SystemAnnouncement` | Thông báo hệ thống | Broadcast |
| `General` | Thông báo chung | Tùy config |

### Độ ưu tiên (Priority)

| Priority | Mô tả | Behavior |
|----------|-------|----------|
| `Low` | Thông tin | Hiển thị bình thường |
| `Normal` | Mặc định | Hiển thị bình thường |
| `High` | Quan trọng | Highlight, sound |
| `Urgent` | Khẩn cấp | Popup, sound |

### Kênh gửi (Channel)

| Channel | Mô tả |
|---------|-------|
| `InApp` | Thông báo trong ứng dụng (SignalR) |
| `Email` | Gửi email (future) |
| `Push` | Push notification (future) |
| `SMS` | Tin nhắn SMS (future) |

---

## SignalR Hub API

### Hub Endpoint

```
ws://localhost:5005/hubs/notification
```

### Client → Server Methods

| Method | Parameters | Mô tả |
|--------|------------|-------|
| `JoinGroup` | `groupName: string` | Tham gia group (user_{userId}) |
| `LeaveGroup` | `groupName: string` | Rời khỏi group |
| `MarkAsRead` | `notificationId: Guid` | Đánh dấu đã đọc |
| `GetUnreadNotifications` | - | Lấy danh sách chưa đọc |

### Server → Client Events

| Event | Payload | Mô tả |
|-------|---------|-------|
| `ReceiveNotification` | `NotificationDto` | Nhận thông báo mới |
| `NotificationRead` | `notificationId: Guid` | Xác nhận đã đọc |
| `UnreadCountUpdated` | `count: int` | Cập nhật số chưa đọc |

### Connection Flow

```javascript
// Client-side (JavaScript)
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/notification", {
        accessTokenFactory: () => getToken()
    })
    .withAutomaticReconnect()
    .build();

// Listen for notifications
connection.on("ReceiveNotification", (notification) => {
    console.log("New notification:", notification);
    // Update UI
});

// Connect
await connection.start();
```

---

## REST API

### Endpoints

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/api/notifications` | Danh sách thông báo (có phân trang) | Required |
| `POST` | `/api/notifications/{id}/read` | Đánh dấu đã đọc | Required |
| `POST` | `/api/notifications/read-all` | Đánh dấu tất cả đã đọc | Required |
| `DELETE` | `/api/notifications/{id}` | Xóa thông báo | Required |

**Query Parameters cho GET /api/notifications:**
- `unreadOnly` (bool): Chỉ lấy thông báo chưa đọc
- `page` (int): Trang hiện tại (mặc định: 1)
- `pageSize` (int): Số item/trang (mặc định: 20)

### Response Examples

```json
// GET /api/notifications
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "Đơn nghỉ phép đã được duyệt",
      "message": "Đơn xin nghỉ từ 20/01 - 22/01 đã được HR phê duyệt",
      "type": "LeaveRequestApproved",
      "priority": "Normal",
      "isRead": false,
      "createdAt": "2025-01-17T10:00:00Z",
      "actionUrl": "/leave/requests/123"
    }
  ],
  "totalCount": 15,
  "unreadCount": 3
}
```

---

## CQRS Pattern

### Commands

| Command | Input | Output | Mô tả |
|---------|-------|--------|-------|
| `CreateNotificationCommand` | UserId, Title, Message, Type, Priority, Data, ActionUrl | `Guid` | Tạo thông báo |
| `MarkAsReadCommand` | NotificationId | `bool` | Đánh dấu đã đọc |
| `MarkAllAsReadCommand` | UserId | `bool` | Đọc tất cả |
| `ArchiveNotificationCommand` | NotificationId | `bool` | Lưu trữ |

### Queries

| Query | Input | Output | Mô tả |
|-------|-------|--------|-------|
| `GetNotificationsQuery` | UserId, UnreadOnly? | `IEnumerable<NotificationDto>` | Lấy danh sách |
| `GetUnreadCountQuery` | UserId | `int` | Đếm chưa đọc |

### NotificationDto

```csharp
public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }        // Enum as string
    public string Priority { get; set; }    // Enum as string
    public string? Data { get; set; }       // JSON data
    public string? ActionUrl { get; set; }
    public bool IsRead { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
}
```

---

## Domain Entities

### Notification

```csharp
public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; }
    public string? Data { get; set; }           // JSON for extra data
    public string? ActionUrl { get; set; }      // Link to related resource
    public bool IsRead { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
```

### UserConnection

```csharp
public class UserConnection
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; }    // SignalR connection ID
    public string? DeviceType { get; set; }     // web, mobile, desktop
    public string? DeviceInfo { get; set; }
    public string? IpAddress { get; set; }
    public DateTime ConnectedAt { get; set; }
    public DateTime? DisconnectedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
}
```

### NotificationTemplate

```csharp
public class NotificationTemplate
{
    public Guid Id { get; set; }
    public string Code { get; set; }            // LEAVE_APPROVED
    public string Name { get; set; }            // Leave Request Approved
    public NotificationType Type { get; set; }
    public string TitleTemplate { get; set; }   // "Đơn nghỉ phép {action}"
    public string MessageTemplate { get; set; } // "Đơn xin nghỉ từ {startDate}..."
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### NotificationPreference

```csharp
public class NotificationPreference
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public bool EmailEnabled { get; set; }
    public bool PushEnabled { get; set; }
    public bool InAppEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### NotificationLog

```csharp
public class NotificationLog
{
    public Guid Id { get; set; }
    public Guid? NotificationId { get; set; }
    public Guid UserId { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Status { get; set; }          // Sent, Delivered, Failed
    public string? Error { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}
```

---

## Database Schema

### ERD Diagram

```
┌───────────────────┐         ┌────────────────────────┐
│     Users         │         │   NotificationTemplates│
│   (External)      │         └────────────────────────┘
└───────────────────┘
         │
         │ 1:N
         ▼
┌───────────────────┐         ┌────────────────────────┐
│   Notifications   │         │  NotificationPreferences│
└───────────────────┘         └────────────────────────┘
         │                              │
         │ 1:N                          │
         ▼                              │
┌───────────────────┐                   │
│ NotificationLogs  │                   │
└───────────────────┘                   │
                                        │
┌───────────────────┐                   │
│  UserConnections  │<──────────────────┘
│  (SignalR state)  │
└───────────────────┘
```

### Bảng chính

| Bảng | Columns chính | Indexes |
|------|---------------|---------|
| `notifications` | id, user_id, title, message, type, priority, is_read, created_at | INDEX(user_id), INDEX(created_at), INDEX(is_read) |
| `user_connections` | id, user_id, connection_id, connected_at | UNIQUE(connection_id), INDEX(user_id) |
| `notification_templates` | id, code, name, type, title_template, message_template | UNIQUE(code) |
| `notification_preferences` | id, user_id, type, email_enabled, push_enabled, in_app_enabled | UNIQUE(user_id, type) |
| `notification_logs` | id, notification_id, user_id, channel, status, sent_at | INDEX(notification_id), INDEX(sent_at) |

### Connection String

```
Host=postgres-notification;Port=5432;Database=notification_db;Username=notification_user;Password=notification_pass
```

---

## Event-Driven Architecture

### RabbitMQ Consumer

Service consume events từ RabbitMQ Exchange `hrm.events` (Topic):

```
┌─────────────────┐          ┌──────────────────────────────┐
│   Time Service  │─publish─►│         RabbitMQ             │
│   (Outbox)      │          │  Exchange: hrm.events (Topic)│
└─────────────────┘          └──────────────────────────────┘
                                           │
                    ┌──────────────────────┼──────────────────────┐
                    │                      │                      │
                    ▼                      ▼                      ▼
          ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────┐
          │ leave_request_  │   │ leave_request_  │   │ leave_request_  │
          │    created      │   │   approved      │   │   rejected      │
          └────────┬────────┘   └────────┬────────┘   └────────┬────────┘
                   │                     │                     │
                   └─────────────────────┼─────────────────────┘
                                         │ bind
                                         ▼
                             ┌──────────────────────┐
                             │  notification.queue  │
                             └──────────┬───────────┘
                                        │ consume
                                        ▼
                             ┌──────────────────────┐
                             │ Notification Service │
                             │  (RabbitMqConsumer)  │
                             └──────────────────────┘
                                        │
                          ┌─────────────┼─────────────┐
                          │             │             │
                          ▼             ▼             ▼
                    ┌──────────┐  ┌──────────┐  ┌──────────┐
                    │ Save DB  │  │ SignalR  │  │  Email   │
                    │          │  │ Push     │  │ (future) │
                    └──────────┘  └──────────┘  └──────────┘
```

### Event Processing

```csharp
// RabbitMQ message format
{
  "event": "leave_request_approved",
  "payload": {
    "requestId": "guid",
    "employeeId": "EMP001",
    "leaveType": "Annual",
    "approvedBy": "manager-id"
  },
  "userIds": ["user-keycloak-id"],
  "employeeIds": ["EMP001"]
}

// Processing flow:
// 1. Parse message
// 2. Get notification template by event type
// 3. Render title/message with payload
// 4. Save notification to database
// 5. Send via SignalR to connected users
// 6. Log delivery status
```

### Events Handled

| Event | Template | Recipients |
|-------|----------|------------|
| `leave_request_created` | LEAVE_CREATED | Manager |
| `leave_request_approved` | LEAVE_APPROVED | Employee |
| `leave_request_rejected` | LEAVE_REJECTED | Employee |
| `overtime_request_created` | OT_CREATED | Manager |
| `overtime_request_approved` | OT_APPROVED | Employee |
| `attendance_checked_in` | ATTENDANCE_IN | User (optional) |
| `attendance_abnormal` | ATTENDANCE_ABNORMAL | HR |

---

## Luồng xử lý

### Real-time Notification Flow

```
┌──────────────┐     ┌───────────────┐     ┌──────────────────────┐
│ Time Service │────>│   RabbitMQ    │────>│ Notification Service │
│ (Event)      │     │    Queue      │     │  (Consumer)          │
└──────────────┘     └───────────────┘     └──────────────────────┘
                                                     │
                           ┌─────────────────────────┼─────────────────┐
                           │                         │                 │
                           ▼                         ▼                 ▼
                    ┌──────────────┐        ┌──────────────┐    ┌───────────┐
                    │ Save to DB   │        │Get Connected │    │Send Email │
                    │ (Notification)│        │   Users      │    │ (future)  │
                    └──────────────┘        └──────────────┘    └───────────┘
                                                     │
                                                     ▼
                                            ┌──────────────┐
                                            │ SignalR Hub  │
                                            │  (Push)      │
                                            └──────────────┘
                                                     │
                                                     ▼
                                            ┌──────────────┐
                                            │   Frontend   │
                                            │  (WebSocket) │
                                            └──────────────┘
```

### User Connection Lifecycle

```
┌──────────┐                    ┌───────────────────┐
│ Frontend │                    │ NotificationHub   │
└──────────┘                    └───────────────────┘
     │                                   │
     │  Connect (with JWT)               │
     │──────────────────────────────────>│
     │                                   │ ─┬─ Validate JWT
     │                                   │  ├─ Save UserConnection
     │                                   │  └─ Add to group: user_{userId}
     │                                   │
     │  OnConnected confirmation         │
     │<──────────────────────────────────│
     │                                   │
     │  ... active session ...           │
     │                                   │
     │  ReceiveNotification              │
     │<──────────────────────────────────│ (When event arrives)
     │                                   │
     │  Disconnect                       │
     │──────────────────────────────────>│
     │                                   │ ─┬─ Remove from group
     │                                   │  └─ Update UserConnection.DisconnectedAt
     │                                   │
```

### Integration with Socket Service

```
┌──────────────┐         ┌───────────────────┐         ┌─────────────────┐
│   Frontend   │◄───────>│  Socket Service   │◄───────>│Notification Svc │
│  (Browser)   │ WebSocket│   (Node.js)      │   HTTP  │   (.NET)        │
└──────────────┘         └───────────────────┘         └─────────────────┘
       │                          │                           │
       │ Connect                  │                           │
       │─────────────────────────>│                           │
       │                          │                           │
       │                          │◄──── RabbitMQ ────────────│
       │                          │     (events)              │
       │                          │                           │
       │◄─── Push notification ───│                           │
       │                          │                           │
```

---

## Cấu hình

### Environment Variables

| Variable | Mô tả | Giá trị mặc định |
|----------|-------|------------------|
| `ASPNETCORE_ENVIRONMENT` | Môi trường | Development |
| `ASPNETCORE_URLS` | URLs lắng nghe | http://+:8080 |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection | - |
| `RabbitMQ__Host` | RabbitMQ host | rabbitmq |
| `RabbitMQ__Port` | RabbitMQ port | 5672 |
| `RabbitMQ__Username` | RabbitMQ user | hrm_user |
| `RabbitMQ__Password` | RabbitMQ password | hrm_pass |
| `RabbitMQ__Exchange` | RabbitMQ exchange | hrm.events |
| `Keycloak__Authority` | Keycloak realm URL | http://keycloak:8080/realms/hrm |
| `Keycloak__Audience` | API audience | hrm-api |

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5434;Database=notification_db;Username=notification_user;Password=notification_pass"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "hrm_user",
    "Password": "hrm_pass",
    "Exchange": "hrm.events",
    "Queue": "notification.queue"
  },
  "Keycloak": {
    "Authority": "http://localhost:8080/realms/hrm",
    "Audience": "hrm-api",
    "RequireHttps": false
  },
  "SignalR": {
    "EnableDetailedErrors": true,
    "KeepAliveInterval": 15,
    "ClientTimeoutInterval": 30
  }
}
```

---

## Chạy ứng dụng

### Với Docker Compose (Khuyến nghị)

```bash
# Từ thư mục hrm-deployment
cd hrm-deployment

# Chạy toàn bộ hệ thống
docker compose up -d

# Hoặc chỉ chạy Notification Service + dependencies
docker compose up -d postgres-notification rabbitmq keycloak notification-service
```

### Local Development

```bash
# 1. Start dependencies
cd hrm-deployment
docker compose up -d postgres-notification rabbitmq keycloak

# 2. Run migrations (nếu có)
cd ../hrm-Notification-Service
dotnet ef database update --project src/Infrastructure --startup-project src/API

# 3. Run service
dotnet run --project src/API
```

### Docker Build

```bash
# Build image
docker build -t hrm-notification-service .

# Run container
docker run -p 5005:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5434;Database=notification_db;Username=notification_user;Password=notification_pass" \
  -e RabbitMQ__Host="host.docker.internal" \
  hrm-notification-service
```

### Ports

| Port | Protocol | Mô tả |
|------|----------|-------|
| 8080 (external: 5005) | HTTP/WebSocket | REST API + SignalR Hub |

### Health Check

```bash
# HTTP health check
curl http://localhost:5005/health

# Test SignalR Hub
# Use browser console or SignalR client
```

---

## Client Integration

### JavaScript (Browser)

```javascript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5005/hubs/notification", {
        accessTokenFactory: () => localStorage.getItem("token")
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Event handlers
connection.on("ReceiveNotification", (notification) => {
    console.log("New notification:", notification);
    showNotificationToast(notification);
    updateBadgeCount();
});

connection.on("UnreadCountUpdated", (count) => {
    updateBadge(count);
});

// Start connection
async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR Connected");
    } catch (err) {
        console.error("SignalR Connection Error:", err);
        setTimeout(startConnection, 5000);
    }
}

startConnection();

// Mark as read
async function markAsRead(notificationId) {
    await connection.invoke("MarkAsRead", notificationId);
}
```

### React Hook Example

```typescript
import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

export function useNotifications() {
    const [notifications, setNotifications] = useState([]);
    const [unreadCount, setUnreadCount] = useState(0);
    const [connection, setConnection] = useState(null);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl('/hubs/notification', {
                accessTokenFactory: () => getToken()
            })
            .withAutomaticReconnect()
            .build();

        newConnection.on('ReceiveNotification', (notification) => {
            setNotifications(prev => [notification, ...prev]);
            setUnreadCount(prev => prev + 1);
        });

        newConnection.start()
            .then(() => setConnection(newConnection))
            .catch(console.error);

        return () => {
            newConnection.stop();
        };
    }, []);

    return { notifications, unreadCount, connection };
}
```

---

## Notification Templates

### Default Templates

| Code | Type | Title Template | Message Template |
|------|------|----------------|------------------|
| `LEAVE_CREATED` | LeaveRequestCreated | Đơn xin nghỉ phép mới | {employeeName} đã gửi đơn xin nghỉ từ {startDate} đến {endDate} |
| `LEAVE_APPROVED` | LeaveRequestApproved | Đơn nghỉ phép được duyệt | Đơn xin nghỉ từ {startDate} đến {endDate} đã được phê duyệt |
| `LEAVE_REJECTED` | LeaveRequestRejected | Đơn nghỉ phép bị từ chối | Đơn xin nghỉ từ {startDate} đến {endDate} đã bị từ chối. Lý do: {reason} |
| `OT_CREATED` | OvertimeRequestCreated | Đơn xin tăng ca mới | {employeeName} đã gửi đơn xin tăng ca ngày {date} |
| `OT_APPROVED` | OvertimeRequestApproved | Đơn tăng ca được duyệt | Đơn xin tăng ca ngày {date} đã được phê duyệt |

---

## Troubleshooting

### Lỗi kết nối Database

```bash
# Kiểm tra container PostgreSQL
docker logs hrm-postgres-notification

# Kiểm tra kết nối
docker exec -it hrm-postgres-notification psql -U notification_user -d notification_db -c "\dt"
```

### Lỗi RabbitMQ Consumer

```bash
# Kiểm tra RabbitMQ
docker logs hrm-rabbitmq

# Kiểm tra queue bindings
curl -u hrm_user:hrm_pass http://localhost:15672/api/queues/%2F/notification.queue

# Kiểm tra exchange
curl -u hrm_user:hrm_pass http://localhost:15672/api/exchanges/%2F/hrm.events
```

### Lỗi SignalR Connection

```bash
# Check CORS settings
# Check Keycloak token validity
# Check WebSocket support in proxy/load balancer
```

### Debug SignalR

```javascript
// Enable detailed logging
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/notification")
    .configureLogging(signalR.LogLevel.Debug)
    .build();
```

---

© 2025 HRM System - Clean Architecture
