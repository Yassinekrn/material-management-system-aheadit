# ERM Platform — AHEADIT Group

> **Employee Resource Management** web application built during an internship at AHEADIT Group (January–February 2023).
> Full-stack platform for managing employee records, equipment inventory, material assignments, and HR workflows.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 10 (C#) · Entity Framework Core 9 · SQLite |
| Caching | Redis (via StackExchange.Redis · `IDistributedCache`) |
| Seed data | Bogus |
| Frontend | Angular · TypeScript · Node.js |
| API Docs | Swagger / OpenAPI |

---

## Features

- **Employee directory** — full CRUD for employee records with department classification
- **Inventory management** — track equipment and supplies by category, brand, price, and availability
- **Assignment tracking** — link employees to equipment with transaction types (Purchase / Assignment / Return)
- **HR records** — log leave requests, performance notes, warnings, and promotions per employee
- **Dashboard aggregates** — real-time summary stats, inventory value by category, headcount by department
- **Redis caching** — dashboard endpoints cached with 60-second TTL; cache-aside pattern
- **Performance-optimized queries** — eager loading via EF Core `Include()` eliminates N+1 patterns

---

## Architecture

```
Angular Frontend  ──►  ASP.NET Core REST API  ──►  EF Core  ──►  SQLite
                                │
                                └──►  IDistributedCache  ──►  Redis
```

### Backend structure

```
Admin_GM/
├── Controllers/
│   ├── EmployeeController.cs    # CRUD: employee records
│   ├── MaterialController.cs    # CRUD: inventory items
│   ├── AssignController.cs      # CRUD + eager-loaded detail view
│   ├── DashboardController.cs   # Cached aggregate endpoints
│   └── LoginController.cs
├── Models/
│   ├── Employee.cs              # Department field added
│   ├── Material.cs              # Category field added
│   ├── Assign.cs                # Navigation props + TransactionType
│   └── HRRecord.cs              # Leave requests & performance notes
├── Data/
│   ├── EmployeeDbContext.cs      # EF Core context + FK relationships
│   └── SeedData.cs              # Bogus-generated test dataset
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

# 3. Apply migrations and seed data (runs automatically on startup)
dotnet run
```

The API starts at `http://localhost:5000`. Database is auto-migrated and seeded on first run.

### Frontend

```bash
cd gestionAdmin   # or the frontend directory
npm install
ng serve -o
```

Swagger UI available at `http://localhost:5000/swagger`.

---

## API Endpoints

### Employee

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Employee` | List all employees |
| `GET` | `/api/Employee/{id}` | Get employee by ID |
| `POST` | `/api/Employee` | Create employee |
| `PUT` | `/api/Employee/{id}` | Update employee |
| `DELETE` | `/api/Employee/{id}` | Delete employee |

### Material (Inventory)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Material` | List all inventory items |
| `GET` | `/api/Material/{id}` | Get item by ID |
| `POST` | `/api/Material` | Add inventory item |
| `PUT` | `/api/Material/{id}` | Update item |
| `DELETE` | `/api/Material/{id}` | Remove item |

### Assignments

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Assign` | List all assignments (IDs only) |
| `GET` | `/api/Assign/details` | Assignments with employee and material names (optimized) |
| `POST` | `/api/Assign` | Create assignment |
| `PUT` | `/api/Assign/{id}` | Update assignment |
| `DELETE` | `/api/Assign/{id}` | Delete assignment |

### Dashboard (Redis-cached, 60s TTL)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/Dashboard/stats` | Total employees, inventory value, open HR requests |
| `GET` | `/api/Dashboard/inventory-by-category` | Item count and value per category |
| `GET` | `/api/Dashboard/employees-by-department` | Headcount per department |

---

## Performance Results

### N+1 Query Elimination

The assignment detail view previously issued one database query per record to resolve
employee names and material names — a classic N+1 pattern.

| Endpoint | Queries Before | Queries After | Reduction |
|----------|---------------|---------------|-----------|
| `GET /api/Assign/details` | **313** | **1** | **99.7%** |

**Fix:** replaced sequential `FindAsync` calls with `.Include(a => a.Employee).Include(a => a.Material)`,
letting EF Core generate a single LEFT JOIN query.

### Redis Cache Impact

Measured over 5 cold runs (Redis flushed between each) and 5 warm runs.

| Endpoint | Cold (DB hit) | Warm (cache hit) | Speedup |
|----------|--------------|-----------------|---------|
| `/api/Dashboard/stats` | 15.2 ms | 4.0 ms | **3.8×** |
| `/api/Dashboard/inventory-by-category` | 7.4 ms | 3.2 ms | **2.3×** |
| `/api/Dashboard/employees-by-department` | 10.4 ms | 3.0 ms | **3.5×** |

Full raw data in [`measurements/METRICS.md`](measurements/METRICS.md).

---

## Seed Data

Generated with [Bogus](https://github.com/bchavez/Bogus) — deterministic, reproducible.

| Entity | Count | Distribution |
|--------|-------|-------------|
| Employees | 300 | 5 departments (IT, HR, Finance, Operations, Marketing) |
| Inventory items | 500 | 8 categories (Hardware, Software, Furniture, Electronics…) |
| Assignments | 200 | 3 transaction types (Purchase, Assignment, Return) |
| HR records | 50 | 5 types (Leave, Performance Note, Warning, Promotion, Training) |

---

## License

This project was developed as part of an internship at **AHEADIT Group**.
See [LICENSE](LICENSE) for details.
