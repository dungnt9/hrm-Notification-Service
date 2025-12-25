# Notification Service

This service manages real-time notifications for the HRM system, including REST API endpoints, SignalR hub, RabbitMQ event consumer, and user notification preferences.

## Features

### Real-time Communication
- **SignalR Hub** for WebSocket-based real-time notifications
- **Group-based messaging** - Users added to `user_{userId}` groups for targeted notifications
- **Auto-reconnect** with exponential backoff strategy
- **JWT authentication** for WebSocket connections

### Notification Management
- **REST API** for notification operations
- **User-specific queries** - Get notifications per user
- **Mark as read** - Individual or bulk marking
- **Notification filtering** - Unread only, pagination
- **User connection tracking** - Active user sessions

### Event-Driven Architecture
- **RabbitMQ Consumer** - Listens for events from Time Service
- **Event types supported**:
  - `leave_request_created` - New leave request for approver
  - `leave_request_approved` - Approval notification to employee
  - `leave_request_rejected` - Rejection notification to employee
  - `attendance.checked_in` - Check-in confirmation
  - `attendance.checked_out` - Check-out confirmation
  - `overtime.requested` - New overtime for approval
  - `overtime.approved` - Overtime approval notification
  - `overtime.rejected` - Overtime rejection notification

### Customization
- **Notification Templates** - Customizable message templates per event type
- **User Preferences** - Control delivery channels (Email, Push, In-App, SMS)
- **Notification Priorities** - Low, Normal, High, Urgent
- **Notification Logging** - Track delivery status and failures

## Tech Stack

- .NET 8, ASP.NET Core Web API
- Entity Framework Core (ORM)
- PostgreSQL (persistent storage)
- RabbitMQ (event bus consumer)
- SignalR (real-time WebSocket)
- Serilog (structured logging)
- Docker

## REST API Endpoints

### Notifications (`/api/notifications`)
- `GET /` - Get notifications with pagination and filters
  - Query: `?unreadOnly=true&page=1&pageSize=20`
  - Response: notifications array, totalCount, unreadCount

- `POST /{id}/read` - Mark single notification as read
  - Param: `id` (notification ID)
  - Response: 200 OK

- `POST /read-all` - Mark all user notifications as read
  - Response: 200 OK

- `GET /templates` - Get notification templates (Admin)
  - Query: `?page=1&pageSize=20`
  - Response: templates array with pagination

- `GET /preferences` - Get user notification preferences
  - Response: preferences object (email, push, inApp, sms enabled)

- `PUT /preferences` - Update user notification preferences
  - Body: `{ emailEnabled: bool, pushEnabled: bool, inAppEnabled: bool, smsEnabled: bool }`
  - Response: 200 OK

### SignalR Hub (`/hubs/notification`)
- Connection: Requires JWT token in query string: `?access_token=TOKEN`
- Authentication: JWT Bearer token validation
- Auto-reconnect: Yes (1s → 3s → 5s exponential backoff)

**Server methods** (called by backend):
- `ReceiveNotification(title, message, type, data, timestamp)` - Push to user group
- `BroadcastNotification(title, message, type, data)` - Send to all users

**Client methods** (called by frontend):
- `MarkAsRead(notificationId)` - Mark as read via WebSocket
- `GetUnreadNotifications()` - Request unread list

## Database Schema

### Core Tables

**Notification**
- Columns: Id, UserId, Title, Message, Type, Priority, Data, ActionUrl, IsRead, IsArchived, CreatedAt, ReadAt, ExpiresAt
- Purpose: Store all notifications
- Indexed: UserId, CreatedAt for efficient queries

**UserConnection**
- Columns: Id, UserId, ConnectionId, DeviceType, DeviceInfo, IpAddress, ConnectedAt, DisconnectedAt, LastActivityAt
- Purpose: Track active SignalR connections per user
- Use: Send to specific connections, identify online users

**NotificationTemplate**
- Columns: Id, Code, Name, Type, TitleTemplate, MessageTemplate, IsActive, CreatedAt, UpdatedAt
- Purpose: Customizable message templates per notification type
- Variables: {employeeName}, {leaveType}, {approverName}, {reason}, etc.

**NotificationPreference**
- Columns: Id, UserId, Type, EmailEnabled, PushEnabled, InAppEnabled, CreatedAt, UpdatedAt
- Purpose: User control over notification channels
- Use: Determine which channels to use for each notification type

**NotificationLog**
- Columns: Id, NotificationId, UserId, Channel, Status, Error, SentAt, DeliveredAt
- Purpose: Audit trail of notification delivery attempts
- Use: Troubleshooting, analytics, retry logic

### Supporting Enums

**NotificationType**
- LeaveRequestCreated, LeaveRequestApproved, LeaveRequestRejected, LeaveRequestCancelled
- OvertimeRequestCreated, OvertimeRequestApproved, OvertimeRequestRejected
- AttendanceReminder, AttendanceAbnormal
- EmployeeOnboarding, EmployeeOffboarding
- BirthdayReminder, WorkAnniversary
- PolicyUpdate, SystemAnnouncement, General

**NotificationPriority**
- Low, Normal, High, Urgent

**NotificationChannel**
- InApp, Email, Push, SMS

## Database Connection

- **Host**: postgres-notification (Docker) / localhost (local)
- **Port**: 5432
- **Database**: notification_db
- **Username**: notification_user
- **Password**: notification_pass
- **Seed data**: `Data/seed-data.sql` (auto-applied on first run)

## Environment Variables

- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string
- `RabbitMQ__Host` - RabbitMQ host (default: rabbitmq)
- `RabbitMQ__Username` - RabbitMQ username (default: hrm_user)
- `RabbitMQ__Password` - RabbitMQ password (default: hrm_pass)
- `Keycloak__Authority` - Keycloak endpoint (default: http://keycloak:8080/realms/hrm)
- `Keycloak__Audience` - Keycloak audience (default: hrm-api)
- `ASPNETCORE_ENVIRONMENT` - Development/Production

## Running Locally

```sh
# Start dependencies only
docker-compose up -d postgres-notification rabbitmq

# Or start everything
docker-compose up -d

# Run migrations
dotnet ef database update --project NotificationService

# Run the service
ASPNETCORE_ENVIRONMENT=Development dotnet run --project NotificationService
```

## Docker

Service is built and run via Docker Compose. See root `docker-compose.yml`.

Port mapping:
- Container: 8080 (HTTP)
- Host: 5005 (HTTP)

## Health Check

- `GET /health` - Service readiness/liveness probe
- Returns 200 OK if database and RabbitMQ are healthy

## Features in Detail

### 1. Real-time Delivery
- Uses SignalR WebSocket for instant delivery
- Group-based routing (per user)
- Connection tracking (online/offline)
- Automatic reconnection support

### 2. Persistent Storage
- All notifications stored in database
- Unread status tracking
- Archive functionality
- Expiration policies

### 3. Event Processing
- RabbitMQ consumer processes events from Time Service
- Outbox pattern for reliability
- Retry logic with exponential backoff
- Structured logging of all events

### 4. User Preferences
- Per-user notification channel preferences
- Per-notification-type settings
- Override system defaults
- Update via API

### 5. Template System
- Customizable message templates
- Variable substitution
- Per-company customization capability
- Type-specific templates

### 6. Connection Management
- Track active user sessions
- Device information capture
- IP address logging
- Last activity timestamps

## Event Flow

```
Time Service publishes event to RabbitMQ
    ↓
Notification Service consumer processes event
    ↓
Create notification in database
    ↓
Check user preferences
    ↓ (InApp enabled)
Get user's SignalR connections
    ↓
Broadcast via WebSocket to user group
    ↓
Log delivery status
    ↓
Frontend receives real-time notification
```

## Integration Points

- **API Gateway** - Called via REST API proxy at `/api/notifications`
- **Time Service** - Consumes events from RabbitMQ
- **Employee Service** - Employee data in notification context
- **Frontend** - SignalR client connects to `/hubs/notification`

## Notes

- Requires PostgreSQL and RabbitMQ to be healthy
- Notification templates are company/system-configurable
- User preferences control notification delivery channels
- SignalR connections auto-managed by framework
- All timestamps in UTC
- Notifications have optional expiration
- Connection IDs change on reconnect (tracked separately)
- InApp notifications always stored in database regardless of preferences

## API Response Examples

### Get Notifications
```json
{
  "data": [
    {
      "id": "uuid",
      "title": "Leave Request Approved",
      "message": "Your leave request for 2025-01-10 has been approved",
      "type": "LeaveRequestApproved",
      "data": "{\"leaveType\": \"Annual\", \"approverName\": \"John Doe\"}",
      "isRead": false,
      "createdAt": "2025-01-09T14:30:00Z",
      "readAt": null
    }
  ],
  "totalCount": 15,
  "page": 1,
  "pageSize": 20,
  "unreadCount": 3
}
```

### Get Preferences
```json
{
  "id": "uuid",
  "emailEnabled": true,
  "pushEnabled": true,
  "inAppEnabled": true,
  "smsEnabled": false
}
```

---

© 2025 HRM System
