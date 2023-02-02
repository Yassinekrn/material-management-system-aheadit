# ERM Platform (AHEADIT Group) — Measured Metrics

## N+1 Query Fix

| View | Queries Before | Queries After | Reduction |
|------|---------------|---------------|-----------|
| Employee list (`GET /api/Employee`) | 1 | 1 | — |
| Material list (`GET /api/Material`) | 1 | 1 | — |
| Assign details (`GET /api/Assign/details`) | **313** | **1** | **99.7%** |
| Dashboard stats (`GET /api/Dashboard/stats`) | 3 | 3 | — |
| Inventory by category | 1 | 1 | — |
| Employees by department | 1 | 1 | — |

**Assign details breakdown (before):** 1 query to load all 200 assignments + up to 312 individual
`FindAsync` calls for related Employee and Material records (N+1 pattern). EF Core identity map
deduplicates repeated IDs within a single request, so 313 total instead of 401 worst-case.

**After fix:** Single query using `.Include(a => a.Employee).Include(a => a.Material)` — EF Core
translates to one LEFT JOIN SQL statement.

## Redis Cache Impact

All endpoints measured over 5 cold runs (Redis flushed between each) and 5 warm runs.

| Endpoint | Avg Cold (DB) | Avg Warm (Cache) | Speedup |
|----------|--------------|-----------------|---------|
| `GET /api/Dashboard/stats` | 15.2ms | 4.0ms | **3.8×** |
| `GET /api/Dashboard/inventory-by-category` | 7.4ms | 3.2ms | **2.3×** |
| `GET /api/Dashboard/employees-by-department` | 10.4ms | 3.0ms | **3.5×** |

Cache TTL: 60 seconds. Cache-aside pattern. Redis running locally via Docker (redis:alpine).

### Raw timing data

**Dashboard stats (cold runs):** 36ms, 16ms, 9ms, 8ms, 7ms  
**Dashboard stats (warm runs):** 5ms, 4ms, 4ms, 4ms, 3ms

**Inventory by category (cold runs):** 9ms, 7ms, 7ms, 8ms, 6ms  
**Inventory by category (warm runs):** 3ms, 4ms, 4ms, 3ms, 2ms

**Employees by department (cold runs):** 8ms, 7ms, 19ms, 8ms, 10ms  
**Employees by department (warm runs):** 3ms, 3ms, 3ms, 3ms, 3ms

## Seed Data (Step 0)

Database migrated from MySQL/SqlServer to SQLite for local development.

| Entity | Count |
|--------|-------|
| Employees | 300 (across 5 departments: IT, HR, Finance, Operations, Marketing) |
| Inventory items (Material) | 500 (across 8 categories) |
| Transactions (Assign) | 200 (Purchase / Assignment / Return) |
| HR Records | 50 (Leave Request, Performance Note, Warning, Promotion, Training) |
