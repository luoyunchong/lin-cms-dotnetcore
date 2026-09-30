# 程序架构

本节描述 LinCms.Core 项目的整体分层架构。

## 分层架构图

```mermaid
graph TD
    A[用户/客户端] --> B(Presentation Layer<br/>ASP.NET Core Web API<br/>`LinCms.Web`);
    B --> C(Application Layer<br/>应用服务 & DTOs<br/>`LinCms.Application`<br/>`LinCms.Application.Contracts`);
    C --> D(Domain Layer<br/>核心领域逻辑 & 实体<br/>`LinCms.Core`);
    C --> E(Infrastructure Layer<br/>数据访问 & 外部服务<br/>`LinCms.Infrastructure`);
    D --> E;
    E --> F[数据库 (MySQL/SQL Server/...)];
    E --> G[缓存 (Redis)];
    E --> H[文件存储 (本地/七牛云)];
    E --> I[消息队列 (RabbitMQ/内存)];

    subgraph "核心项目"
        direction LR
        B; C; D; E;
    end

    subgraph "外部依赖"
        direction LR
        F; G; H; I;
    end

    style B fill:#f9f,stroke:#333,stroke-width:2px;
    style C fill:#ccf,stroke:#333,stroke-width:2px;
    style D fill:#9cf,stroke:#333,stroke-width:2px;
    style E fill:#cfc,stroke:#333,stroke-width:2px;
```

## 各层职责

*   **Presentation Layer (`LinCms.Web`)**:
    *   处理 HTTP 请求和响应。
    *   包含 API 控制器 (Controllers)。
    *   配置和运行 ASP.NET Core 中间件 (Middleware)。
    *   负责用户认证、授权的入口。
    *   依赖应用层处理业务逻辑。

*   **Application Layer (`LinCms.Application`, `LinCms.Application.Contracts`)**:
    *   定义应用服务接口和数据传输对象 (DTOs)。
    *   实现具体的应用服务，编排领域逻辑。
    *   处理数据验证、权限检查、事务管理。
    *   映射 DTOs 和领域实体。
    *   是领域层和表现层之间的桥梁。

*   **Domain Layer (`LinCms.Core`)**:
    *   包含核心业务逻辑和规则。
    *   定义领域实体 (Entities) 和值对象 (Value Objects)。
    *   定义仓储接口 (IRepositories)。
    *   定义领域服务 (Domain Services)。
    *   不依赖其他层（除了 .NET 基础库）。

*   **Infrastructure Layer (`LinCms.Infrastructure`)**:
    *   实现仓储接口，负责与数据库交互 (使用 FreeSql ORM)。
    *   实现与其他基础设施的交互，如文件存储、缓存、消息队列等。
    *   依赖领域层定义的接口。

## 插件与工具


这种分层架构有助于实现关注点分离 (Separation of Concerns)，提高代码的可维护性、可测试性和可扩展性。

