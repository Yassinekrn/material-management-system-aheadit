using Admin_GM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Admin_GM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly EmployeeDbContext _context;
        private readonly IDistributedCache _cache;

        public DashboardController(EmployeeDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            const string cacheKey = "dashboard:stats";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return Ok(JsonSerializer.Deserialize<object>(cached));

            var stats = new
            {
                TotalEmployees = await _context.Employee.CountAsync(),
                TotalInventoryValue = await _context.Material.SumAsync(m => m.Price),
                OpenRequests = await _context.HRRecord.CountAsync(h => h.Status == "Pending")
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(stats),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                });

            return Ok(stats);
        }

        [HttpGet("inventory-by-category")]
        public async Task<IActionResult> GetInventoryByCategory()
        {
            const string cacheKey = "dashboard:inventory-by-category";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return Ok(JsonSerializer.Deserialize<object>(cached));

            var summary = await _context.Material
                .GroupBy(m => m.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Count = g.Count(),
                    TotalValue = g.Sum(m => m.Price)
                })
                .ToListAsync();

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(summary),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                });

            return Ok(summary);
        }

        [HttpGet("employees-by-department")]
        public async Task<IActionResult> GetEmployeesByDepartment()
        {
            const string cacheKey = "dashboard:employees-by-department";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return Ok(JsonSerializer.Deserialize<object>(cached));

            var summary = await _context.Employee
                .GroupBy(e => e.Department)
                .Select(g => new
                {
                    Department = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(summary),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
                });

            return Ok(summary);
        }
    }
}
