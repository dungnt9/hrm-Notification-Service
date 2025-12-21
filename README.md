# Notification Service

This service manages real-time notifications for the HRM system, including SignalR hubs, notification templates, and user preferences.

## Features

- Real-time notifications via SignalR
- Notification templates and preferences
- RabbitMQ consumer for event-driven notifications
- Notification status tracking (delivered, failed, etc.)
- User connection management

## Tech Stack

- .NET 8, ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- RabbitMQ
- SignalR
- Docker

## Endpoints

- REST: `/api/notifications`, `/api/templates`, `/api/preferences`
- SignalR Hub: `/hubs/notification`

## Database

- Connection: `Host=postgres-notification;Port=5432;Database=notification_db;Username=notification_user;Password=notification_pass`
- Seed data: `Data/seed-data.sql` (auto-applied on first run)

## Environment Variables

- `ConnectionStrings__DefaultConnection`
- `RabbitMQ__Host`, `RabbitMQ__Username`, `RabbitMQ__Password`
- `Keycloak__Authority`, `Keycloak__Audience`

## Running Locally

```sh
docker-compose up -d postgres-notification rabbitmq
# (or run all infra)
dotnet ef database update --project NotificationService
ASPNETCORE_ENVIRONMENT=Development dotnet run --project NotificationService
```

## Docker

Service is built and run via Docker Compose. See root `docker-compose.yml`.

## Health Check

- `/health` endpoint for readiness/liveness

## Notes

- Requires PostgreSQL and RabbitMQ to be healthy before startup
- Notification templates are customizable
- User preferences control notification delivery

---

© 2025 HRM System
