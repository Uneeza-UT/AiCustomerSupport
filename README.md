# .NET AI Customer Support API

An ASP.NET Core Web API for an AI-powered customer support system. The
API supports customer conversations, message history, AI-generated
responses, intent detection, escalation to human support, and
administrative operations.

The backend is designed around Clean Architecture principles and exposes
REST API endpoints for authentication, conversation management,
messaging, and administration. SignalR is used to deliver newly saved
messages in real time.

------------------------------------------------------------------------

## Features

-   User registration and login
-   JWT authentication
-   Email-related authentication functionality
-   Role-based authorization
    -   Customer
    -   Agent
    -   Admin
-   Conversation creation and management
-   Conversation and message history
-   Customer messaging
-   AI-generated customer support responses
-   Intent detection
-   Conversation escalation to human support
-   Agent and admin messaging
-   Conversation status management
-   Real-time message delivery using SignalR
-   Administrative APIs
-   Analytics-related API functionality
-   DTO-based API contracts
-   Input validation
-   Entity Framework Core data persistence
-   Swagger / OpenAPI documentation
-   Centralized error handling
-   Email notifications through Resend

------------------------------------------------------------------------

## User Roles

### Customer

Customers can:

-   Register and log in
-   Start a conversation by submitting an initial message
-   Send messages in their conversations
-   View their conversation history
-   Receive AI-generated responses while the conversation is handled by
    the AI
-   Continue sending messages after a conversation has been escalated
-   Receive replies from an assigned human support representative where
    applicable

Customers can only access conversations they are authorized to access.

### Agent

Agents can:

-   Access conversations assigned or otherwise made available to them
    through the authorization rules
-   View conversation history
-   Send messages to customers
-   Participate in conversations escalated from AI support

Agent messages are saved as agent messages and do not trigger an AI
response by default.

### Admin

Administrative functionality includes:

-   Access administrative APIs
-   Monitor and manage conversations according to authorization rules
-   View escalated conversations
-   Access analytics-related functionality

------------------------------------------------------------------------

## Technologies

The project uses the following technologies and architectural
components:

-   ASP.NET Core Web API
-   C#
-   Entity Framework Core
-   SQL Server
-   ASP.NET Core Identity
-   JWT authentication
-   Email services
-   Large Language Model (LLM) integration
-   SignalR
-   DTOs
-   Validation
-   Resend
-   Clean Architecture
-   Swagger / OpenAPI

------------------------------------------------------------------------

## Architecture

The project follows Clean Architecture principles, separating core
business logic from infrastructure and API concerns.

``` text
AI Customer Support API
│
├── API
│   ├── wwwroot
│   ├── Controllers
│   ├── Middleware
│   ├── Models
│   ├── Swagger
│   └── Program.cs
│
├── Application
│   ├── Contracts
│   │   ├── Email
│   │   ├── Identity
│   │   ├── Persistence
│   │   └── Services
│   │
│   ├── DTOs
│   ├── Exceptions
│   ├── Mapping Profiles
│   ├── Models
│   └── Validations
│
├── Domain
│   ├── Common
│   ├── Enums
│   └── Entities
│
└── Infrastructure
    ├── Identity
    │   ├── Configurations
    │   ├── DbContext
    │   └── Models
    │   └── Services
    │
    └── Infrastructure       
    │   ├── Email Service
    │   ├── Hubs
    │   └── Services
    │
    └── Persistence
        └── Configurations
        ├── DbContext
        ├── Repositories

```

-   **Domain:** Contains the core entities and enums.
-   **Application:** Contains DTOs, use cases, interfaces, and
    validation logic.
-   **Infrastructure:** Implements persistence, repositories, identity,
    JWT, email, and Hub integrations.
-   **API:** Exposes HTTP endpoints and contains controllers,
    middleware, Swagger configuration, and application startup.

------------------------------------------------------------------------

## Authentication

The API uses ASP.NET Core Identity for user management and JWT Bearer
tokens to authenticate API requests.

After logging in, the client receives a JWT token. Authenticated
requests use the following header:

``` http
Authorization: Bearer <token>
```

Protected endpoints use authentication and role-based authorization to
control access.

------------------------------------------------------------------------

## Using Swagger

1.  Use the **Login** endpoint to log in with an existing account, or
    create a new account using the **Register** endpoint.
2.  After a successful login response (HTTP 200), Swagger automatically
    retrieves the JWT token from the response and applies it to
    subsequent authorized requests.
3.  You can then test protected endpoints without manually copying the
    token into Swagger's Authorize dialog.

The automatic token handling is implemented through custom Swagger
JavaScript.

------------------------------------------------------------------------

## Authorization

The API applies role-based and conversation-level authorization.

Conversation access is scoped according to the requesting user's role
and relationship to the conversation. Customers must not be able to
access other customers' conversations. Agent and administrative access
is governed by the applicable authorization rules.

The application uses a conversation authorization-scope helper to
centralize the logic used when determining which conversations a user
can access.

------------------------------------------------------------------------

## Conversations

A customer must create a conversation before sending messages. Customers can create multiple conversations, allowing them to discuss different issues, ask questions, and seek assistance independently. Each conversation maintains its own message history.

The initial conversation request is made through:

``` http
POST /api/conversations
```

Subsequent messages are submitted through:

``` http
POST /api/conversations/{conversationId}/messages
```

The API stores conversation messages so that the conversation history
can be retrieved later.

------------------------------------------------------------------------

## Message Handling

Messages identify their sender using a sender type:

-   `Customer`
-   `AI`
-   `Agent`

Customer messages can trigger AI processing while the conversation is
being handled by the AI.

Agent and admin messages are saved without triggering an AI response by
default.

AI-generated messages are stored with `SenderType` set to `AI` and a
null `SenderId`, because an AI response is not associated with an
application user account.

The conversation history is retrieved in chronological order and
supplied to the AI service as context. Customer messages are mapped to
the LLM's `user` role, while AI messages are mapped to its `model` role.

------------------------------------------------------------------------

## AI Responses and Intent Detection

The AI integration processes customer messages using the conversation
history as context.

The AI response workflow supports:

-   Generating a response to a customer message
-   Identifying the intent of the customer's message
-   Returning structured response information to the application
-   Indicating when a conversation should be escalated to human support

Intent information is associated with messages where applicable. The
intent reference is nullable so that messages without an identified
intent can still be stored.

------------------------------------------------------------------------

## Conversation Escalation

The system supports escalation from AI-based support to human support.

When a conversation is escalated:

-   Its status is updated to `Escalated`.
-   Subsequent customer messages can still be submitted.
-   The AI stops generating further replies for that conversation.
-   An agent or admin can respond as a human support representative.
-   Administrative APIs can be used to identify escalated conversations.

This allows a conversation to move from automated support to human
assistance while retaining its existing message history.

------------------------------------------------------------------------

## Real-Time Messaging

SignalR is used to deliver newly saved messages to connected clients in
real time.

The general message flow is:

``` text
Customer
   │
   │ Sends message
   ▼
ASP.NET Core API
   │
   ├── Save customer message
   │
   ├── Retrieve conversation history
   │
   ├── Process message through AI service
   │       │
   │       ├── Generate AI response
   │       ├── Identify intent
   │       └── Determine whether escalation is required
   │
   ├── Save AI response when applicable
   │
   └── Deliver newly saved messages through SignalR
```

SignalR is responsible for real-time delivery. AI processing is handled
by the application service workflow rather than by the SignalR hub
itself.

Messages are persisted before they are delivered, allowing conversation
history to remain available when a user reconnects.

------------------------------------------------------------------------

## Message History

Conversation messages are stored in the database and can be retrieved
through the API.

The application uses chronological conversation history as context when
generating AI responses.

This separates the two responsibilities:

-   **REST API:** Creates conversations, submits messages, and retrieves
    persisted history.
-   **SignalR:** Delivers newly saved messages to connected clients in
    real time.

------------------------------------------------------------------------

## Validation and Error Handling

The application uses request DTOs and validation to check incoming data
before processing it.

Centralized error handling is used to provide consistent API error
responses.

Common HTTP status codes include:

-   `200 OK`
-   `201 Created`
-   `204 No Content`
-   `400 Bad Request`
-   `401 Unauthorized`
-   `403 Forbidden`
-   `404 Not Found`
-   `500 Internal Server Error`

------------------------------------------------------------------------

## Prerequisites

-   .NET SDK compatible with the project target framework
-   SQL Server
-   Entity Framework Core tools
-   An IDE or editor capable of building and running ASP.NET Core
    applications
-   Credentials and configuration required by the selected email and LLM
    providers

------------------------------------------------------------------------

## Configuration

Configure the application settings required by the project before
running the API.

These include:

-   SQL Server connection string
-   JWT configuration
-   Resend API key
-   LLM provider configuration

The exact configuration keys depend on the provider implementations and
options classes used by the project.

Example connection-string and JWT configuration:

``` json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-sql-server-connection-string"
  },
  "JwtSettings": {
    "Key": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "DurationInMinutes": 30
  },
  "EmailSettings": {
    "ApiKey": "your-secret-key",
    "FromAddress": "your-verified-sender-email",
    "FromName": "your-sender-name"
  },
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY",
    "Model": "YOUR_GEMINI_MODEL"
  }
}
```

Use the actual configuration section names and properties defined in the
application.

Sensitive values such as database credentials, JWT signing keys, email
credentials, and LLM API keys should be stored in .NET User Secrets,
environment variables, or deployment platform configuration rather than
committed to source control.

------------------------------------------------------------------------

## Database

The application uses Entity Framework Core for data access and SQL
Server for persistence.

The database stores application data such as:

-   Users and roles
-   Conversations
-   Messages
-   Attachments
-   Intents
-   Notifications

The exact schema is defined by the project's entity configurations and
Entity Framework Core migrations.

------------------------------------------------------------------------

## Database Setup

1.  Create a SQL Server database or configure an existing SQL Server
    instance.
2.  Configure the database connection string.
3.  Open the solution in your IDE.
4.  Migrations are committed to GitHub.
5.  After configuring the SQL Server connection string, run:

Select Persistence project as default project:
update-database -Context ApplicationDbContext

Select Identity project as default project:
update-database -Context ApplicationIdentityDbContext

6. Start the application.

------------------------------------------------------------------------

## Running the Application

Clone the repository:

``` bash
git clone <repository-url>
```

Navigate to the API project directory:

``` bash
cd <api-project-directory>
```

Restore dependencies:

``` bash
dotnet restore
```

Apply database migrations:

``` bash
dotnet ef database update
```

Run the application:

``` bash
dotnet run
```

------------------------------------------------------------------------

## Swagger / API Documentation

The API is documented using Swagger / OpenAPI.

Once the application is running, open:

``` text
https://localhost:<port>/swagger
```

Swagger can be used to explore and test the API endpoints, including
authentication, conversation creation, message submission, conversation
history, and administrative operations.

------------------------------------------------------------------------

## Testing the API

A typical testing flow is:

1.  Configure the SQL Server connection string.
2.  Configure JWT settings.
3.  Configure the required email and LLM provider settings.
4.  Apply database migrations.
5.  Start the API.
6.  Open Swagger.
7.  Register a customer account or log in with an existing account.
8.  Create a conversation.
9.  Submit subsequent messages to the conversation messages endpoint.
10. Verify that customer messages are saved and processed by the AI
    while the conversation is AI-handled.
11. Verify that AI responses and intent information are saved.
12. Retrieve conversation history and verify its chronological order.
13. Test escalation to human support.
14. Verify that the AI no longer responds after the conversation has
    been escalated.
15. Test agent or admin replies.
16. Test real-time delivery through SignalR.
17. Test authorization by attempting to access conversations outside the
    requesting user's permitted scope.
18. Test the administrative and analytics endpoints.

------------------------------------------------------------------------

## Test Accounts

The following demo accounts are pre-configured through the application's database seeding configuration and can be used to test the different user roles.

### Customer
Email: customer@example.com
Password: Customer1234!

### Admin
Email: admin@example.com
Password: Admin1234!

### Admin
Email: agent@example.com
Password: Agent1234!

 

------------------------------------------------------------------------

## Project Status

The project is an ASP.NET Core AI Customer Support backend API.

It provides customer conversations, persistent message history,
AI-assisted responses, intent detection, escalation to human support,
real-time message delivery, authentication, authorization, and
administrative API functionality.

A frontend application can consume the API and use SignalR for real-time
message updates.

------------------------------------------------------------------------

## Implementation Highlights

-   Clean Architecture
-   ASP.NET Core Web API
-   ASP.NET Core Identity
-   JWT authentication
-   Role-based authorization
-   Conversation-level authorization
-   Repository and service abstractions
-   Entity Framework Core
-   SQL Server
-   DTO-based API contracts
-   Conversation and message persistence
-   Chronological conversation history
-   LLM integration
-   AI-generated responses
-   Intent detection
-   Human-support escalation
-   SignalR real-time message delivery
-   Separation of AI processing and real-time delivery
-   Administrative APIs
-   Analytics-related functionality
-   Email notifications
-   Swagger / OpenAPI documentation
