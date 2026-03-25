# Sample API Repository

A reference ASP.NET Core Web API project demonstrating Clean Architecture, Domain-Driven Design principles, and modern .NET 10 best practices. This project serves as a showcase of clean code, proper separation of concerns, and robust software architecture.

## 🚀 Quick Start

Get the project up and running in seconds. It uses an in-memory SQLite database, so no external database engine is required!

```bash
# 1. Clone the repository
git clone <your-repo-url>
cd Sample

# 2. Restore dependencies
dotnet restore

# 3. Run the API
cd Sample.Api
dotnet run
```

**Accessing the API:**
- Swagger UI / Documentation: `http://localhost:<port>/swagger`
- The application automatically seeds initial data (including permissions and default users) into the in-memory SQLite database upon startup.

## 🏗️ Architecture

The solution follows a Clean Architecture approach, ensuring the core business logic is isolated from framework-specific implementation details.

```text
┌────────────────────────────────────────────────────────┐
│                   Sample.Api (Presentation)            │
│  - Controllers (REST endpoints)                        │
│  - Middleware (Exception Handling, Logging)            │
│  - JWT Auth Helpers & Startup Configuration            │
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼────────────────────────────┐
│                   Sample.Core (Business Logic)         │
│  - Services (Business use cases)                       │
│  - Mappers (AutoMapper profiles)                       │
│  - Dependency Injection Config                         │
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼────────────────────────────┐
│                   Sample.Common (Shared Resources)     │
│  - DTOs (Requests, Responses, PagedResults)            │
│  - Custom Exceptions (Domain, NotFound, etc.)          │
│  - Validators (FluentValidation rules)                 │
└────────────────────────────────────────────────────────┘

* Note: Persistence logic (Entity Framework Core using SQLite In-Memory) 
is orchestrated within the Core/Api layers for simplicity in this sample, 
adhering to repository/service patterns.
```

## 🛠️ Tech Stack & Libraries

- **Framework:** .NET 10, C#
- **Database:** SQLite (In-Memory configuration for sampling)
- **ORM:** Entity Framework Core
- **Validation:** FluentValidation
- **Object Mapping:** AutoMapper
- **Authentication:** JWT (JSON Web Tokens)
- **Testing:** xUnit, Moq, AutoFixture, FluentAssertions

## 📂 Project Structure

```text
Sample/
├── Sample.Api/              # API Host, Endpoints, Middleware, Auth
├── Sample.Core/             # Business Logic (Services), Mapping
├── Sample.Common/           # Cross-cutting: DTOs, Exceptions, Validators
└── Sample.Test/             # Unit Tests (xUnit, Moq)
```

## 🎯 Best Practices & Key Features

This project implements several industry-standard patterns and practices:

- ✅ **Clean Architecture:** Strict separation of concerns between API, Core, and Common logic.
- ✅ **Centralized Exception Handling:** Custom HTTP Middleware (`ExceptionHandlingMiddleware`) catches domain exceptions and formats standard RFC problem details responses.
- ✅ **Automated Validation:** `FluentValidation` securely validates incoming requests before processing.
- ✅ **In-Code Data Seeding:** Permissions, roles, and initial state are seeded automatically in the application logic, bypassing the need for external scripts.
- ✅ **Secure Authentication:** JWT token generation and validation encapsulated in `Helpers/AuthManager`.
- ✅ **Meaningful Domain Exceptions:** Specific error handling using `DomainException`, `NotFoundException`, `UnauthorizedException`, etc.
- ✅ **Comprehensive Unit Testing:** Testing driven by `xUnit`, `Moq`, and custom AutoFixture data attributes (`[DefaultData]`).

## 🌐 API Endpoints & Usage

### 🔑 Authentication (Login)

Authenticates a user and issues a JWT token for protected routes.

**POST** `/v1/user/login`

**Request Body:**
```json
{
  "username": "john.berry",
  "password": "John.Berry.123!"
}
```

**Under the Hood:**
- Authentication is delegated to the `AuthManager` helper service.
- Upon successful validation, it generates a **JWT token** to be used in subsequent requests.
- Protected endpoints use a custom `[Authorize]` attribute to validate the token, verify roles, and guarantee the user holds the necessary permissions for that specific resource.

---

### 🧑‍💻 Get Users (Paginated & Filtered)

Retrieves a list of users. If no parameters are provided, it falls back to default pagination settings.

**GET** `/v1/user?pageNumber=1&pageSize=10&sortingProperty=firstName`

**Query Parameters:**

| Parameter         | Description                                      | Default     |
|-------------------|--------------------------------------------------|-------------|
| `pageNumber`      | The current page to retrieve.                    | `1`         |
| `pageSize`        | The number of records per page.                  | `10`        |
| `sortingProperty` | The exact property/field name to sort the items. |             |
| `sortingType`     | `Ascending` or `Descending` order.               | `Ascending` |

> 💡 **Tech Note:** An abstract `Filter` base class handles common metadata like pagination and sorting seamlessly. The project architecture supports dynamic data mapping and fallback in-memory filtering when needed.

**Response Structure:**

Returns a structured `PagedResult` object containing the requested slice of data and total row counts.

```json
{
  "realRows": 100,
  "limitRows": 10,
  "data": [
    {
      "username": "john.berry",
      "firstName": "John",
      "lastName": "Berry",
      "role": "SuperUser",
      "permissions": [
        "user.list",
        "user.manage"
      ]
    }
  ]
}
```

## 🧪 Testing

The test suite ensures the reliability of the business services using isolated unit tests.

```bash
# Run all tests
cd Sample.Test
dotnet test

# Run tests with detailed logging
dotnet test --logger "console;verbosity=detailed"
```

Tests follow the **Arrange-Act-Assert (AAA)** pattern and heavily utilize Mock abstractions for deterministic execution.

## 🤝 Contributing

This is a personal portfolio repository intended to showcase architectural patterns and clean code practices. However, suggestions, feedback, and discussions are always welcome in the Issues tab!

