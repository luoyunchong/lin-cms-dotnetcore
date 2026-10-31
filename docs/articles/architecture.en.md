**English** | [简体中文](architecture.md)

# Application architecture

This section describes the layered architecture of the LinCms.Core project.

## Layered architecture

```mermaid
graph TD
    A[User/client] --> B(Presentation Layer<br/>ASP.NET Core Web API<br/>`LinCms.Web`);
    B --> C(Application Layer<br/>Application services & DTOs<br/>`LinCms.Application`<br/>`LinCms.Application.Contracts`);
    C --> D(Domain Layer<br/>Core domain logic & entities<br/>`LinCms.Core`);
    C --> E(Infrastructure Layer<br/>Data access & external services<br/>`LinCms.Infrastructure`);
    D --> E;
    E --> F[Database (MySQL/SQL Server/...)];
    E --> G[Cache (Redis)];
    E --> H[File storage (local/Qiniu Cloud)];
    E --> I[Message queue (RabbitMQ/in-memory)];

    subgraph "Core projects"
        direction LR
        B; C; D; E;
    end

    subgraph "External dependencies"
        direction LR
        F; G; H; I;
    end

    style B fill:#f9f,stroke:#333,stroke-width:2px;
    style C fill:#ccf,stroke:#333,stroke-width:2px;
    style D fill:#9cf,stroke:#333,stroke-width:2px;
    style E fill:#cfc,stroke:#333,stroke-width:2px;
```

## Layer responsibilities

*   **Presentation Layer (`LinCms.Web`)**:
    *   Handles HTTP requests and responses.
    *   Contains API controllers.
    *   Configures and runs ASP.NET Core middleware.
    *   Provides the entry point for authentication and authorization.
    *   Delegates business processing to the application layer.

*   **Application Layer (`LinCms.Application`, `LinCms.Application.Contracts`)**:
    *   Defines application service interfaces and data transfer objects (DTOs).
    *   Implements application services and orchestrates domain logic.
    *   Handles validation, permission checks, and transaction management.
    *   Maps DTOs and domain entities.
    *   Bridges the domain and presentation layers.

*   **Domain Layer (`LinCms.Core`)**:
    *   Contains core business logic and rules.
    *   Defines domain entities and value objects.
    *   Defines repository interfaces (IRepositories).
    *   Defines domain services.
    *   Does not depend on other layers, apart from the .NET base libraries.

*   **Infrastructure Layer (`LinCms.Infrastructure`)**:
    *   Implements repository interfaces and database access using FreeSql ORM.
    *   Implements interactions with infrastructure such as file storage, caches, and message queues.
    *   Depends on interfaces defined by the domain layer.

## Plugins and tools


This layered architecture supports separation of concerns and improves maintainability, testability, and extensibility.

