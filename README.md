# User Management API

A simple ASP.NET Core Web API for managing users, built with GitHub Copilot as part of the Coursera project.

## Features

- ✅ Full **CRUD** endpoints (GET, POST, PUT, DELETE)
- ✅ **Data Validation** using Data Annotations
- ✅ **Logging Middleware** for request/response tracking
- ✅ **Authentication Middleware** using API Key
- ✅ **Error Handling Middleware** for global exception management
- ✅ **Swagger/OpenAPI** documentation
- ✅ **Dependency Injection** with scoped services

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Run the API

```bash
cd UserManagementAPI
dotnet run
```

Navigate to: `http://localhost:5000/swagger`

---

## Authentication

All endpoints require an API key header:

```
X-API-Key: my-secret-api-key-12345
```

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create a new user |
| PUT | `/api/users/{id}` | Update an existing user |
| DELETE | `/api/users/{id}` | Delete a user |

---

## Example Requests

### Create a User (POST)
```json
POST /api/users
Headers: X-API-Key: my-secret-api-key-12345

{
  "name": "Jane Doe",
  "email": "jane@example.com",
  "age": 25,
  "role": "User"
}
```

### Update a User (PUT)
```json
PUT /api/users/1
Headers: X-API-Key: my-secret-api-key-12345

{
  "name": "Jane Smith",
  "email": "jane.smith@example.com",
  "age": 26,
  "role": "Admin"
}
```

---

## Validation Rules

- **Name**: Required, 2–100 characters
- **Email**: Required, valid email format, must be unique
- **Age**: Required, between 1 and 120
- **Role**: Required (e.g., "Admin", "User")

---

## Middleware

| Middleware | Purpose |
|------------|---------|
| `ErrorHandlingMiddleware` | Catches unhandled exceptions globally |
| `RequestLoggingMiddleware` | Logs all requests and responses with timing |
| `ApiKeyAuthMiddleware` | Validates API key on all requests |

---

## Project Structure

```
UserManagementAPI/
├── Controllers/
│   └── UsersController.cs      # CRUD endpoints
├── Middleware/
│   └── Middlewares.cs          # Logging, Auth, Error handling
├── Models/
│   └── User.cs                 # User model with validation
├── Services/
│   └── UserService.cs          # Business logic layer
├── Program.cs                  # App configuration & pipeline
└── appsettings.json            # Logging configuration
```

---

## Built With
- ASP.NET Core 8
- GitHub Copilot (for code generation and debugging)
- Swagger/OpenAPI