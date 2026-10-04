# OpsFlow

OpsFlow is an issue and claim management platform designed to help organizations raise, assign, track, review, and resolve workplace issues through a centralized workflow.

The application provides role-based access for users, employees, and managers and supports organization-level issue management, attachments, status tracking, authentication, notifications, and issue history.

---

## 1. Project Overview

OpsFlow provides a structured workflow for handling issues raised by employees/users inside an organization.

Instead of managing issues through informal communication, OpsFlow provides a centralized system where:

- Users can raise issues.
- Issues can contain descriptions and image/video attachments.
- Managers can receive and manage issues.
- Issues can be assigned to employees.
- Assigned employees can work on issues.
- Employees can submit completed work for review.
- Managers can review and close issues.
- Every important status/action can be recorded in issue history.
- Email notifications can be sent during important workflow events.
- Organizations and users can be managed centrally.

The main goal is to make issue handling **structured, traceable, role-based, and easier to manage**.

---

# 2. Purpose of the Project

The primary purpose of OpsFlow is to provide an internal issue-management workflow for organizations.

### Problems it solves

Without a centralized system, issue management can become difficult because:

- Issues may be reported through different communication channels.
- There may be no clear ownership of an issue.
- Users may not know the current status.
- Managers may not know which employee is handling an issue.
- Attachments and supporting evidence can become difficult to track.
- There may be no proper history of status changes.
- Resolution and review activities may not be clearly recorded.

OpsFlow addresses these problems by providing a centralized workflow from **issue creation to final resolution**.

---

# 3. Core Workflow

The basic OpsFlow workflow is:

```text
User
  |
  | Raise Issue
  v
Raised
  |
  | Manager assigns employee
  v
In Progress
  |
  | Employee completes work
  v
Review
  |
  | Manager reviews
  v
Closed
```

### Workflow explanation

#### 1. Raise Issue

A user creates an issue by providing:

- Issue title
- Description
- Optional photo
- Optional video

The issue is initially created with the appropriate workflow status.

#### 2. Assignment

A manager can assign the issue to an employee.

The assigned employee becomes responsible for working on the issue.

#### 3. In Progress

The assigned employee works on the issue.

The issue can contain supporting attachments and workflow history.

#### 4. Review

After completing the work, the assigned employee submits the issue for review.

The review transition is handled through the backend workflow rules.

#### 5. Closed

The manager reviews the submitted issue and can complete the final resolution workflow.

---

# 4. Main Features

## Authentication

OpsFlow provides authentication functionality including:

- User registration
- User login
- Authentication using Supabase
- JWT-based authentication
- Role-based access
- Organization association

The application uses authenticated user information to determine access to protected functionality.

---

## Role-Based Access Control

OpsFlow supports role-oriented workflows.

The major application roles include:

- User
- Employee
- Manager
- Admin-related management functionality where applicable

Different roles have different responsibilities.

### User

Users can:

- Create issues
- View issues they have raised
- View issue status
- View issue details
- View available attachments

### Employee

Employees can:

- View assigned issues
- Work on assigned issues
- Update issues according to the permitted workflow
- Submit completed issues for review

### Manager

Managers can:

- View issues under their organization/workflow
- Assign issues to employees
- Review submitted issues
- Manage issue workflow
- Close issues where permitted

### Administration

Administrative functionality includes user and organization management where configured.

---

# 5. Issue Management

Issue management is the core functionality of OpsFlow.

An issue contains information such as:

- Issue ID
- Title
- Description
- Raised by user
- Organization
- Manager
- Assigned employee
- Current status
- Created date
- Assignment date
- Updated date
- Resolution information
- Attachments
- History

This provides a complete view of an issue throughout its lifecycle.

---

# 6. Attachments

OpsFlow supports issue attachments.

Currently the issue creation workflow supports:

- Images
- Videos

Attachments are associated with an issue and can be retrieved when viewing issue details.

The frontend dynamically loads attachment content from the backend and displays supported image/video content inside the issue details interface.

This allows users to provide visual evidence when reporting an issue.

---

# 7. Issue Details

The issue details view provides a complete view of an issue.

It can display:

- Issue information
- Current status
- Raised-by information
- Assigned employee
- Manager information
- Description
- Attachments
- Issue history
- Timestamps
- Resolution information where available

The frontend uses dynamic state updates so that changes made after API operations are reflected correctly in the UI.

---

# 8. Issue History

OpsFlow maintains workflow history for issues.

History can contain information such as:

- Action
- Previous status
- New status
- User who performed the action
- User name
- User email
- Comment
- Timestamp

Example:

```text
Issue Created
      |
      v
Assigned
      |
      v
In Progress
      |
      v
Submitted for Review
      |
      v
Closed
```

This provides traceability for issue workflow changes.

---

# 9. User Management

The user-management functionality allows administrators/authorized users to manage users.

Supported functionality includes:

- Fetch users
- View user details
- View organization information
- Change user roles
- Filter/search users

The frontend communicates with the authentication/user-management APIs to perform these operations.

---

# 10. Organization Management

Users are associated with organizations.

Organization information is used during:

- User registration
- User management
- Issue ownership/workflow
- Manager and employee relationships
- Organization-level access

This allows OpsFlow to support multiple organizations rather than treating the application as a single-organization system.

---

# 11. Email / SMTP Notifications

OpsFlow includes an SMTP-based notification module.

Email notifications can be used for important workflow events such as:

- Issue assignment
- Issue submission for review
- Workflow updates
- Other configured issue-related notifications

This helps keep the relevant users informed without requiring them to constantly check the application.

---

# 12. Frontend Architecture

The frontend is built using Angular.

The frontend follows a component/service based structure.

```text
Angular Application
│
├── Pages
│   ├── Authentication
│   ├── Claims / Issues
│   ├── My Issues
│   ├── Assigned Issues
│   ├── All Issues
│   └── User Management
│
├── Core
│   ├── Services
│   ├── Models
│   ├── Authentication
│   └── API communication
│
└── Shared UI / Components
```

### Angular responsibilities

The frontend handles:

- UI rendering
- User interactions
- Form handling
- API communication
- Authentication state
- Role-based UI behavior
- Issue filtering
- Issue details
- Attachment rendering
- Loading/error/success states
- Dynamic UI state updates

---

# 13. Backend Architecture

The backend is implemented using ASP.NET Core Web API.

The backend exposes APIs used by the Angular frontend.

Conceptually:

```text
Angular Frontend
       |
       | HTTP / REST API
       v
ASP.NET Core Web API
       |
       +-------------------+
       |                   |
       v                   v
 Authentication        Issue/Claim
       |                 Management
       |
       +---------+---------+
                 |
                 v
          Database / Services
```

The backend is responsible for:

- Business rules
- Authentication/authorization integration
- Issue workflow
- Role validation
- Organization logic
- Assignment rules
- Status transitions
- Attachment retrieval
- Issue history
- Database operations
- Email notifications

---

# 14. Architecture Style

OpsFlow follows a layered application architecture.

```text
┌─────────────────────────────────────────┐
│              Angular UI                 │
│                                         │
│ Components / Pages / Forms / State      │
└────────────────────┬────────────────────┘
                     │
                     │ HTTP REST APIs
                     v
┌─────────────────────────────────────────┐
│          ASP.NET Core Web API           │
│                                         │
│ Controllers                             │
│ Services / Business Logic               │
│ Authentication / Authorization          │
└────────────────────┬────────────────────┘
                     │
          ┌──────────┴──────────┐
          │                     │
          v                     v
┌───────────────────┐   ┌─────────────────┐
│ SQL / Application │   │ Supabase        │
│ Data              │   │ Auth / Data     │
└───────────────────┘   └─────────────────┘
```

The application separates:

- Presentation
- API/controller layer
- Business/service layer
- Data access
- Authentication
- External services

This separation makes the application easier to maintain and extend.

---

# 15. Technology Stack

## Frontend

- Angular
- TypeScript
- HTML
- CSS
- Angular Forms
- Angular HTTP Client
- RxJS

## Backend

- C#
- ASP.NET Core Web API
- Entity Framework Core
- LINQ
- REST APIs

## Authentication

- Supabase Auth
- JWT
- JWT validation
- Role-based authorization

## Database / Data Storage

- Microsoft SQL Server
- Supabase/PostgreSQL where applicable
- Entity Framework Core for relational application data

## Notifications

- SMTP
- Email notifications

## Development Tools

- Visual Studio / VS Code
- Node.js
- Angular CLI
- .NET CLI
- Git
- GitHub

---

# 16. Frontend API Communication

Angular services are used as the communication layer between the UI and backend.

Example architecture:

```text
Component
   |
   v
Angular Service
   |
   v
HTTP Client / API Client
   |
   v
ASP.NET Core API
```

For example:

```text
My Issues Component
        |
        v
ClaimService
        |
        v
Claims API
        |
        v
Backend
```

This keeps API calls outside the UI components and improves maintainability.

---

# 17. Dynamic UI State Management

OpsFlow uses Angular state updates to keep the UI synchronized with API operations.

For important asynchronous operations, the frontend updates state such as:

- Loading
- Success
- Error
- Selected issue
- Issue details
- Attachments
- Status
- Modal visibility
- Filters

`ChangeDetectorRef` is used in areas where explicit change detection is required.

Example:

```ts
this.cdr.markForCheck();
this.cdr.detectChanges();
```

This ensures UI changes are reflected after asynchronous API operations.

---

# 18. Issue Details Data Flow

When a user opens an issue:

```text
User clicks "View Details"
          |
          v
Angular Component
          |
          v
ClaimService
          |
          v
GET Claim Details API
          |
          v
Backend
          |
          v
Complete Claim Response
          |
          +---- Issue Information
          |
          +---- Attachments
          |
          +---- History
          |
          +---- Assignment Information
          |
          v
Angular State
          |
          v
Change Detection
          |
          v
Details UI
```

Attachments are then fetched dynamically using the attachment identifiers returned with the issue details.

---

# 19. Error and Loading Handling

The frontend provides state handling for API operations.

Examples include:

- Loading indicators
- Submit button states
- API error messages
- Empty states
- Attachment loading states
- Details loading states
- Success messages

This prevents users from interacting with controls while an operation is still being processed.

---

# 20. Security

Security is handled through authenticated API access and role-aware backend validation.

The application uses:

- Supabase authentication
- JWT tokens
- Protected API endpoints
- Role-based authorization
- Organization-based access rules
- Backend validation of workflow operations

Important workflow rules are enforced by the backend rather than relying only on frontend UI restrictions.

---

# 21. Example Issue Lifecycle

A typical issue can move through the following lifecycle:

```text
                    ┌──────────────┐
                    │     User     │
                    └──────┬───────┘
                           │
                           │ Create Issue
                           v
                    ┌──────────────┐
                    │    Raised    │
                    └──────┬───────┘
                           │
                           │ Assignment
                           v
                    ┌──────────────┐
                    │  In Progress │
                    └──────┬───────┘
                           │
                           │ Submit for Review
                           v
                    ┌──────────────┐
                    │    Review    │
                    └──────┬───────┘
                           │
                           │ Manager Resolution
                           v
                    ┌──────────────┐
                    │    Closed    │
                    └──────────────┘
```

Each transition can be recorded in the issue history.

---

# 22. Project Modules

The major modules of OpsFlow are:

```text
OpsFlow
│
├── Authentication Module
│   ├── Registration
│   ├── Login
│   └── JWT / RBAC
│
├── User Management Module
│   ├── Users
│   ├── Roles
│   └── Organizations
│
├── Claim / Issue Module
│   ├── Create Issue
│   ├── My Issues
│   ├── All Issues
│   ├── Assigned Issues
│   ├── Assignment
│   ├── Status Workflow
│   └── Issue History
│
├── Attachment Module
│   ├── Image Upload
│   ├── Video Upload
│   └── Attachment Retrieval
│
└── SMTP Module
    └── Email Notifications
```

---

# 23. Why OpsFlow?

OpsFlow provides a complete workflow instead of simply storing issue records.

The platform connects:

```text
Authentication
      +
Organizations
      +
Users & Roles
      +
Issue Management
      +
Assignment
      +
Attachments
      +
Workflow
      +
History
      +
Notifications
```

This creates a centralized system for managing operational issues from creation through resolution.

---

# 24. Future Enhancements

Potential future improvements include:

- Dashboard and analytics
- Advanced issue filtering
- SLA tracking
- Priority management
- Due dates
- Escalation workflows
- Real-time notifications
- Real-time issue updates
- Audit dashboards
- Advanced reporting
- Cloud deployment
- CI/CD automation
- Docker-based deployment
- Kubernetes deployment
- Monitoring and observability

---

# 25. Summary

OpsFlow is an organization-focused issue management platform built using **Angular and ASP.NET Core Web API**.

Its core purpose is to provide a reliable workflow for:

```text
Raise
  ↓
Assign
  ↓
Work
  ↓
Review
  ↓
Resolve
```

The platform combines authentication, role-based access, organization management, issue tracking, attachments, issue history, and email notifications into one centralized application.

The architecture separates the frontend, API/business logic, data access, authentication, and notification responsibilities, making the system easier to maintain and extend.

---

## Technology Summary

| Layer | Technology |
|---|---|
| Frontend | Angular + TypeScript |
| UI | HTML + CSS |
| Backend | ASP.NET Core Web API |
| Language | C# |
| ORM | Entity Framework Core |
| Querying | LINQ |
| Authentication | Supabase Auth |
| Authorization | JWT / Role-based access |
| Database | Microsoft SQL Server |
| Additional Data Services | Supabase/PostgreSQL where applicable |
| Notifications | SMTP |
| API Style | REST |
| Source Control | Git / GitHub |

---

## Project Goal

> **OpsFlow aims to provide a centralized, role-based, traceable, and scalable platform for managing organizational issues from creation to resolution.**
