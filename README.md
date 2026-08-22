<div align="center">

# HireFlow

**A production-ready job board REST API built with .NET 8 and Clean Architecture**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)](https://postgresql.org)
[![Redis](https://img.shields.io/badge/Redis-7-DC382D?logo=redis&logoColor=white)](https://redis.io)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.13-FF6600?logo=rabbitmq&logoColor=white)](https://rabbitmq.com)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)](https://docker.com)
[![SignalR](https://img.shields.io/badge/SignalR-Real--time-512BD4)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)

[API Docs](#api-reference) · [Getting Started](#getting-started) · [Architecture](#architecture)

</div>

---

## About

HireFlow is a full-featured job board platform where **companies** post jobs and **freelancers** apply — with real-time chat, async email notifications, CV management with auto-generated PDFs, Redis caching, and a complete authentication system.

Built as a portfolio project to demonstrate production-level .NET backend architecture.

**Key technical decisions:**
- **Clean Architecture** with strict layer separation — Domain has zero external dependencies
- **CQRS with MediatR** — every operation is a command or query with automatic FluentValidation pipeline
- **RabbitMQ** decouples email sending from HTTP requests — API never blocks on SMTP
- **SignalR** delivers chat messages in real-time without polling
- **Redis** caches job search results — database not hit on repeated identical queries
- **QuestPDF** generates professional PDF resumes from structured CV data
- **Soft delete** — no data ever permanently removed, full audit trail preserved

---

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    HireFlow.Api                          │
│   Controllers · Hubs · Middleware · Program.cs           │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                 HireFlow.Application                     │
│    CQRS Handlers · DTOs · Interfaces · Validators       │
└──────────────────────┬──────────────────────────────────┘
                       │
        ┌──────────────┴──────────────┐
        │                             │
┌───────▼────────┐          ┌─────────▼──────────────────┐
│ HireFlow.Domain│          │   HireFlow.Infrastructure   │
│ Entities·Enums │          │  EF Core · Redis · RabbitMQ │
│ Exceptions     │          │  SignalR · QuestPDF · SMTP  │
└────────────────┘          └─────────────────────────────┘
```

**Dependency rule:** arrows always point inward. Domain knows nothing about EF Core, ASP.NET, or any external library.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| Architecture | Clean Architecture + CQRS (MediatR) |
| Database | PostgreSQL 16 (EF Core 8) |
| Caching | Redis 7 (StackExchange.Redis) |
| Messaging | RabbitMQ 3.13 (MassTransit) |
| Real-time | SignalR |
| Auth | JWT Bearer + Refresh Token Rotation |
| Validation | FluentValidation (MediatR pipeline behavior) |
| Logging | Serilog (console + rolling file) |
| PDF Generation | QuestPDF |
| Testing | xUnit + Moq + FluentAssertions |
| Containerization | Docker + Docker Compose |

---

## Features

### Authentication
- [x] Register as Freelancer or Company — role never user-supplied (server-side assignment)
- [x] Login with JWT access token (120 min) + refresh token (7 days)
- [x] Refresh token rotation — old token revoked on each refresh
- [x] Forgot password — sends 6-digit code via email (RabbitMQ async)
- [x] Reset password — validates code, updates password, revokes all active sessions

### Jobs
- [x] Companies post, edit, and close job listings
- [x] Server-side search with filters: keyword, category, location, salary range
- [x] Pagination and sorting
- [x] Redis caching on search results — invalidated on any job change
- [x] Job expiry date support

### Applications
- [x] Freelancers apply with cover letter and CV selection
- [x] Unique constraint — one application per job per freelancer
- [x] Status pipeline: Pending → Reviewed → Accepted / Rejected
- [x] Email notification to freelancer on every status change (async via RabbitMQ)
- [x] Full status history audit trail
- [x] Freelancers can withdraw pending applications

### CV Management
- [x] Multiple CV versions per freelancer (Backend CV, Full Stack CV, etc.)
- [x] Structured CV: title, summary, skills, experience, education, projects, languages
- [x] Upload PDF/DOCX file as an alternative to structured CV
- [x] Set a default CV — auto-attached when applying without specifying one
- [x] Pick a specific CV per job application
- [x] Auto-generate professional PDF from structured CV data using QuestPDF
- [x] Companies can download applicant CV directly from application

### Chat
- [x] Real-time messaging between company and freelancer per application (SignalR)
- [x] Full message history via REST API
- [x] Unread message count
- [x] Mark messages as read
- [x] Access restricted — only conversation participants can send/view
- [x] Blocked on withdrawn applications

### Profiles
- [x] Freelancer profile: bio, skills, years of experience, portfolio URL, phone
- [x] Company profile: name, description, logo, website, location
- [x] Avatar upload for freelancers
- [x] Logo upload for companies
- [x] File validation — size and extension checked before saving

### Admin
- [x] Admin seeded automatically on first startup
- [x] Approve / suspend companies
- [x] Soft delete jobs and users
- [x] View all users and companies with pagination

### Infrastructure
- [x] Global exception handler — typed domain exceptions map to correct HTTP status codes
- [x] FluentValidation pipeline behavior — validation runs before every handler automatically
- [x] Serilog structured logging — console + daily rolling file (7-day retention)
- [x] Health checks: PostgreSQL (Unhealthy), Redis (Degraded), RabbitMQ (Degraded)
- [x] Automatic database migrations on startup
- [x] Soft delete with EF Core global query filters
- [x] File uploads served as static files via `UseStaticFiles`
- [x] Docker volumes for file and log persistence

---

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [.NET 8 SDK](https://dotnet.microsoft.com/download) — only for local development

### Run with Docker

```bash
# 1. Clone
git clone https://github.com/BehruzbekUmarov/HireFlow.git
cd HireFlow

# 2. Create environment file
cp .env.example .env
# Edit .env with your values

# 3. Start everything
docker compose up --build

# App available at:
# Swagger UI    → http://localhost:8080/swagger
# Health check  → http://localhost:8080/health
# Chat test UI  → http://localhost:8080
# RabbitMQ UI   → http://localhost:15672  (guest/guest)
```

### Run locally (hybrid mode)

```bash
# Start only the services in Docker
docker compose up postgres redis rabbitmq -d

# Run the API from Visual Studio or:
dotnet run --project src/HireFlow.Api
```

### Apply migrations

Migrations run automatically on startup via `MigrateAsync()`. To run manually:

```bash
dotnet ef database update \
  --project src/HireFlow.Infrastructure \
  --startup-project src/HireFlow.Api
```

---

## Environment Variables

Copy `.env.example` to `.env` and fill in your values:

```env
# Database
POSTGRES_PASSWORD=your_strong_password

# JWT
JWT_SECRET=your_random_secret_minimum_32_characters

# RabbitMQ
RABBITMQ_USERNAME=guest
RABBITMQ_PASSWORD=guest

# Email (Gmail App Password)
EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
EMAIL_USER=your@gmail.com
EMAIL_PASSWORD=your_16_char_app_password

# File storage
SERVER_IP=localhost
```

> Never commit `.env` to Git — it is in `.gitignore` by default.

---

## API Reference

Full interactive docs available at `/swagger` when running.

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register/freelancer` | Register as freelancer |
| POST | `/api/auth/register/company` | Register as company |
| POST | `/api/auth/login` | Login, returns JWT + refresh token |
| POST | `/api/auth/refresh` | Rotate refresh token |
| POST | `/api/auth/forgot-password` | Send 6-digit reset code via email |
| POST | `/api/auth/reset-password` | Reset password with code |

### Jobs

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/jobs` | — | Search jobs with filters |
| GET | `/api/jobs/{id}` | — | Get job detail |
| GET | `/api/jobs/my` | Company | Company's own listings |
| POST | `/api/jobs` | Company | Create a listing |
| PUT | `/api/jobs/{id}` | Company | Update a listing |
| PATCH | `/api/jobs/{id}/close` | Company | Close a listing |

### Applications

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/jobs/{jobId}/apply` | Freelancer | Apply with cover letter + CV |
| GET | `/api/applications/my` | Freelancer | My applications |
| GET | `/api/jobs/{jobId}/applications` | Company | Applicants for a job |
| PATCH | `/api/applications/{id}/status` | Company | Change status |
| PATCH | `/api/applications/{id}/withdraw` | Freelancer | Withdraw application |
| GET | `/api/applications/{id}/cv-download` | Company | Download applicant CV |

### CV Management

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/cvs` | Freelancer | Get all my CVs |
| GET | `/api/cvs/{id}` | Freelancer | Get specific CV |
| POST | `/api/cvs` | Freelancer | Create structured CV |
| POST | `/api/cvs/upload` | Freelancer | Upload PDF/DOCX as CV |
| PUT | `/api/cvs/{id}` | Freelancer | Update CV |
| DELETE | `/api/cvs/{id}` | Freelancer | Delete CV |
| PATCH | `/api/cvs/{id}/set-default` | Freelancer | Set as default CV |
| GET | `/api/cvs/{id}/download` | Freelancer | Download CV as PDF |

### Chat

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/applications/{id}/messages` | Both | Load conversation |
| POST | `/api/applications/{id}/messages` | Both | Send a message |
| PATCH | `/api/applications/{id}/messages/read` | Both | Mark as read |

### Profiles

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/profile/freelancer` | Freelancer | Get own profile |
| PUT | `/api/profile/freelancer` | Freelancer | Update profile |
| POST | `/api/profile/freelancer/avatar` | Freelancer | Upload avatar |
| GET | `/api/profile/company` | Company | Get company profile |
| PUT | `/api/profile/company` | Company | Update company profile |
| POST | `/api/profile/company/logo` | Company | Upload logo |

### Admin

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/admin/users` | Admin | All users |
| GET | `/api/admin/companies` | Admin | All companies |
| PATCH | `/api/admin/companies/{id}/approve` | Admin | Approve company |
| PATCH | `/api/admin/companies/{id}/suspend` | Admin | Suspend company |
| DELETE | `/api/admin/jobs/{id}` | Admin | Soft delete job |
| DELETE | `/api/admin/users/{id}` | Admin | Soft delete user |

### System

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Full health check |
| GET | `/health/live` | Liveness probe (Docker) |

### SignalR Hub

```
/hubs/chat

Client methods:
  JoinConversation(applicationId)   — join a conversation room
  LeaveConversation(applicationId)  — leave a conversation room

Server events:
  NewMessage(MessageDto)            — received when someone sends a message
```

---

## Key Design Decisions

### Why CQRS with MediatR?
Each operation is a self-contained command or query. Cross-cutting concerns — validation, logging — are added via pipeline behaviors without touching handler code. Adding a new validator means one new class, zero changes to existing code.

### Why RabbitMQ for emails?
SMTP can take 1-3 seconds. With RabbitMQ, the API publishes an event and returns immediately. The consumer sends the email in the background. If the email server is slow or down, the API is completely unaffected — messages retry automatically.

### Why SignalR for chat?
REST polling wastes bandwidth and adds latency. SignalR maintains a persistent connection and pushes messages instantly. When a company sends a message, the freelancer's browser receives it in milliseconds without any polling.

### Why QuestPDF for CV generation?
Many freelancers — especially in Uzbekistan — don't have a professional CV. Instead of requiring a file upload, HireFlow collects structured data (skills, experience, education) and generates a professional PDF automatically. Freelancers who do have a PDF can upload it directly instead.

### Why Redis caching on job search?
Job listings are read far more often than they are written. Caching search results for 5 minutes means 100 simultaneous users performing the same search hit PostgreSQL once, not 100 times. Cache is invalidated immediately when any job is created, updated, or closed.

### Why soft delete instead of hard delete?
Applications reference jobs. Users have application history. Deleting either would break referential integrity. Soft delete sets `IsDeleted = true` and hides records via EF Core global query filters while preserving all history.

### Why separate register endpoints?
`/register/freelancer` and `/register/company` hardcode the role server-side — it's never user-supplied. This prevents privilege escalation. Admin accounts are seeded directly in the database on first startup.

---

## Project Structure

```
HireFlow/
├── src/
│   ├── HireFlow.Domain/
│   │   ├── Entities/       # User, Job, Company, JobApplication, Message, FreelancerCv...
│   │   ├── Enums/          # UserRole, ApplicationStatus
│   │   └── Exceptions/     # NotFoundException, ForbiddenException...
│   │
│   ├── HireFlow.Application/
│   │   ├── Features/       # CQRS Commands and Queries grouped by feature
│   │   ├── DTOs/           # Request/Response shapes
│   │   ├── Services/       # ICacheService, IEmailService, ICvPdfService, IChatNotificationService...
│   │   ├── Events/         # RabbitMQ event messages
│   │   └── Common/         # ValidationBehavior, ChatConstants
│   │
│   ├── HireFlow.Infrastructure/
│   │   ├── Persistence/    # AppDbContext, EF Core configurations, migrations
│   │   ├── Security/       # TokenService, PasswordHasher
│   │   ├── Caching/        # RedisCacheService
│   │   ├── Messaging/      # MassTransit consumers
│   │   ├── Email/          # SmtpEmailService
│   │   ├── Storage/        # LocalFileStorageService
│   │   ├── Documents/      # CvDocument, CvPdfService (QuestPDF)
│   │   ├── Hubs/           # ChatHub (SignalR)
│   │   └── SignalR/        # ChatNotificationService
│   │
│   └── HireFlow.Api/
│       ├── Controllers/    # All API controllers
│       ├── Middlewares/    # ErrorHandlerMiddleware
│       ├── Extensions/     # Serilog, HealthChecks, DI wiring
│       ├── wwwroot/        # Chat test UI (index.html)
│       └── Program.cs
│
├── tests/
│   └── HireFlow.Tests/     # xUnit unit tests
│
├── docker-compose.yml
├── Dockerfile
├── .env.example
└── README.md
```

---

## Running Tests

```bash
dotnet test
```

Tests cover: registration, duplicate email detection, password hashing, job creation authorization, application status transitions, ownership checks, audit trail recording, and cache invalidation.

---

## Default Admin Account

Seeded automatically on first startup:

```
Email:    admin@hireflow.com
Password: Admin123!
```

---

## Author

**Bekhruzjon Umarov** — .NET Backend Developer

- GitHub: [@BehruzbekUmarov](https://github.com/BehruzbekUmarov)
- Location: Tashkent, Uzbekistan

---

*Built to demonstrate Clean Architecture, CQRS, real-time messaging, async email notifications, PDF generation, and production deployment patterns in .NET 8.*
