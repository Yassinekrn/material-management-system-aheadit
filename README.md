<p align="center">
  <img src="docs/assets/banner.png" alt="ERM Platform" width="100%">
</p>

# ERM Platform — AHEADIT Group

> **Employee Resource Management** web application built during an internship at AHEADIT Group (January–February 2023).
> Full-stack platform for managing employee records, equipment inventory, material assignments, and HR workflows.

---

## Tech Stack

| Layer     | Technology                                              |
| --------- | ------------------------------------------------------- |
| Backend   | ASP.NET Core 10 (C#) · Entity Framework Core 9 · SQLite |
| Caching   | Redis · StackExchange.Redis · `IDistributedCache`       |
| Seed data | Bogus                                                   |
| Frontend  | Angular · TypeScript · Node.js                          |
| API Docs  | Swagger / OpenAPI                                       |

---

## Features

- **Employee directory** — full CRUD for employee records with department classification
- **Inventory management** — track equipment and supplies by category, brand, price, and availability
- **Assignment tracking** — link employees to equipment with transaction types (Purchase / Assignment / Return)
- **HR records** — log leave requests, performance notes, warnings, and promotions per employee
- **Dashboard aggregates** — real-time summary stats, inventory value by category, headcount by department
- **Redis caching** — dashboard endpoints cached with 60-second TTL using cache-aside pattern
- **Optimized queries** — eager loading via EF Core `Include()` eliminates N+1 patterns

---

## Architecture

```mermaid
graph LR
    FE["🖥️ Angular\nFrontend"]

    subgraph API["ASP.NET Core REST API"]
        EC[EmployeeController]
        MC[MaterialController]
        AC[AssignController]
        DC[DashboardController]
    end

    subgraph Data["Data Layer"]
        CTX["EF Core 9\nDbContext"]
        DB[(SQLite)]
    end

    subgraph Cache["Cache Layer"]
        RC["IDistributedCache\nStackExchange.Redis"]
        RD[(Redis\n:6379)]
    end

    FE --> EC & MC & AC & DC
    EC & MC & AC & DC --> CTX
    DC --> RC
    CTX --> DB
    RC --> RD
```

---

## Data Model

### Entity-Relationship Diagram

```mermaid
erDiagram
    EMPLOYEE {
        int ID PK
        string Name
        string Prename
        string CIN
        string Email
        string Phone_Number
        string Home_Address
        string Emp_Function
        string Department
    }
    MATERIAL {
        int ID PK
        string Name
        double Price
        datetime Purchase_Date
        string Brand
        string Reference
        int IsTaken
        string Category
    }
    ASSIGN {
        int ID PK
        int ID_Employee FK
        int ID_Material FK
        datetime Start_Date
        datetime End_Date
        string TransactionType
    }
    HRRECORD {
        int ID PK
        int ID_Employee FK
        string Type
        string Description
        datetime Date
        string Status
    }

    EMPLOYEE ||--o{ ASSIGN : "is assigned"
    MATERIAL ||--o{ ASSIGN : "assigned in"
    EMPLOYEE ||--o{ HRRECORD : "has"
```

### Class Diagram

```mermaid
classDiagram
    class Employee {
        +int ID
        +string Name
        +string Prename
        +string CIN
        +string Email
        +string Phone_Number
        +string Home_Address
        +string Emp_Function
        +string Department
    }

    class Material {
        +int ID
        +string Name
        +double Price
        +DateTime Purchase_Date
        +string Brand
        +string Reference
        +int IsTaken
        +string Category
    }

    class Assign {
        +int ID
        +int ID_Employee
        +int ID_Material
        +DateTime Start_Date
        +DateTime End_Date
        +string TransactionType
        +Employee? Employee
        +Material? Material
    }

    class HRRecord {
        +int ID
        +int ID_Employee
        +string Type
        +string Description
        +DateTime Date
        +string Status
        +Employee? Employee
    }

    Employee "1" --> "0..*" Assign : assigned
    Material "1" --> "0..*" Assign : assigned to
    Employee "1" --> "0..*" HRRecord : has
```

---

## Backend Structure

```
Admin_GM/
├── Controllers/
│   ├── EmployeeController.cs    # CRUD: employee records
│   ├── MaterialController.cs    # CRUD: inventory items
│   ├── AssignController.cs      # CRUD + eager-loaded detail view
│   ├── DashboardController.cs   # Cached aggregate endpoints
│   └── LoginController.cs
├── Models/
│   ├── Employee.cs              # + Department field
│   ├── Material.cs              # + Category field
│   ├── Assign.cs                # + Navigation properties + TransactionType
│   └── HRRecord.cs              # Leave requests & performance notes
├── Data/
│   ├── EmployeeDbContext.cs     # EF Core context + FK relationships
│   └── SeedData.cs             # Bogus-generated test dataset
├── Migrations/                  # EF Core SQLite migration
└── Program.cs                   # DI: SQLite, Redis, EF logging
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for Redis)
- [Node.js](https://nodejs.org/) + Angular CLI (for frontend)

### Backend

```bash
# 1. Start Redis
docker run -d -p 6379:6379 --name redis-erm redis:alpine

# 2. Restore dependencies
cd Backend/Admin_GM/Admin_GM
dotnet restore

# 3. Run — auto-migrates and seeds on first start
dotnet run
```

API runs at `http://localhost:5000`. Swagger UI at `http://localhost:5000/swagger`.

### Frontend

```bash
cd gestionAdmin
npm install
ng serve -o
```

---

## API Reference

### Employee

| Method   | Endpoint             | Description        |
| -------- | -------------------- | ------------------ |
| `GET`    | `/api/Employee`      | List all employees |
| `GET`    | `/api/Employee/{id}` | Get employee by ID |
| `POST`   | `/api/Employee`      | Create employee    |
| `PUT`    | `/api/Employee/{id}` | Update employee    |
| `DELETE` | `/api/Employee/{id}` | Delete employee    |

### Material (Inventory)

| Method   | Endpoint             | Description              |
| -------- | -------------------- | ------------------------ |
| `GET`    | `/api/Material`      | List all inventory items |
| `GET`    | `/api/Material/{id}` | Get item by ID           |
| `POST`   | `/api/Material`      | Add inventory item       |
| `PUT`    | `/api/Material/{id}` | Update item              |
| `DELETE` | `/api/Material/{id}` | Remove item              |

### Assignments

| Method   | Endpoint              | Description                                |
| -------- | --------------------- | ------------------------------------------ |
| `GET`    | `/api/Assign`         | All assignments (raw IDs)                  |
| `GET`    | `/api/Assign/details` | Assignments with employee & material names |
| `POST`   | `/api/Assign`         | Create assignment                          |
| `PUT`    | `/api/Assign/{id}`    | Update assignment                          |
| `DELETE` | `/api/Assign/{id}`    | Delete assignment                          |

### Dashboard _(Redis-cached, 60s TTL)_

| Method | Endpoint                                 | Description                                        |
| ------ | ---------------------------------------- | -------------------------------------------------- |
| `GET`  | `/api/Dashboard/stats`                   | Total employees, inventory value, open HR requests |
| `GET`  | `/api/Dashboard/inventory-by-category`   | Item count and value per category                  |
| `GET`  | `/api/Dashboard/employees-by-department` | Headcount per department                           |

---

## Performance

### Cache-Aside Flow

```mermaid
sequenceDiagram
    participant Client
    participant API as ASP.NET Core API
    participant Redis
    participant DB as SQLite (EF Core)

    Client->>API: GET /api/Dashboard/stats

    API->>Redis: GetStringAsync("dashboard:stats")

    alt Cache HIT (avg 4ms)
        Redis-->>API: cached JSON
        API-->>Client: 200 OK ⚡
    else Cache MISS (avg 15ms)
        Redis-->>API: null
        API->>DB: COUNT employees
        API->>DB: SUM inventory value
        API->>DB: COUNT open HR requests
        DB-->>API: aggregated results
        API->>Redis: SetStringAsync(key, data, TTL=60s)
        API-->>Client: 200 OK
    end
```

### N+1 Query Elimination

Replacing sequential `FindAsync` calls with `.Include()` in the assignment detail endpoint:

| Endpoint                  | Before          | After       | Reduction |
| ------------------------- | --------------- | ----------- | --------- |
| `GET /api/Assign/details` | **313 queries** | **1 query** | **99.7%** |

### Redis Cache Impact

_5 cold runs (Redis flushed between each) · 5 warm runs_

| Endpoint                                 | Cold (DB) | Warm (Cache) | Speedup  |
| ---------------------------------------- | --------- | ------------ | -------- |
| `/api/Dashboard/stats`                   | 15.2 ms   | 4.0 ms       | **3.8×** |
| `/api/Dashboard/inventory-by-category`   | 7.4 ms    | 3.2 ms       | **2.3×** |
| `/api/Dashboard/employees-by-department` | 10.4 ms   | 3.0 ms       | **3.5×** |

Full raw data → [`measurements/METRICS.md`](measurements/METRICS.md)

---

## Frontend Preview

<p align="center">
  <img src="docs/assets/frontend-preview.png" alt="ERM Platform Frontend Preview" width="90%">
</p>

---

## Seed Data

Generated with [Bogus](https://github.com/bchavez/Bogus) — deterministic, reproducible (seed = 42).

| Entity          | Count | Distribution                                                                                           |
| --------------- | ----- | ------------------------------------------------------------------------------------------------------ |
| Employees       | 300   | 5 departments: IT, HR, Finance, Operations, Marketing                                                  |
| Inventory items | 500   | 8 categories: Hardware, Software, Furniture, Office Supplies, Electronics, Networking, Vehicles, Tools |
| Assignments     | 200   | 3 types: Purchase, Assignment, Return                                                                  |
| HR records      | 50    | 5 types: Leave Request, Performance Note, Warning, Promotion, Training                                 |

---

## UML Diagrams (PlantUML)

Source files for report-quality diagrams are in [`docs/diagrams/`](docs/diagrams/):

| File                                                       | Description                  |
| ---------------------------------------------------------- | ---------------------------- |
| [`er-diagram.puml`](docs/diagrams/er-diagram.puml)         | Entity-relationship diagram  |
| [`class-diagram.puml`](docs/diagrams/class-diagram.puml)   | Domain model class diagram   |
| [`sequence-cache.puml`](docs/diagrams/sequence-cache.puml) | Cache-aside sequence diagram |
| [`architecture.puml`](docs/diagrams/architecture.puml)     | System component diagram     |

Render with [PlantUML](https://plantuml.com/) or the VS Code PlantUML extension.

---

## License

Developed during an internship at **AHEADIT Group**.
See [LICENSE](LICENSE) for details.
