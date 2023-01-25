using Admin_GM.Data;
using Admin_GM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin_GM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignController : ControllerBase
    {
        private readonly EmployeeDbContext _context;
        public AssignController(EmployeeDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Assign>> Get()
           => await _context.Assign.ToListAsync();

        // N+1 pattern — baseline measurement before fix
        [HttpGet("details-naive")]
        public async Task<IActionResult> GetDetailsNaive()
        {
            var assigns = await _context.Assign.ToListAsync();
            var results = new List<object>();
            foreach (var a in assigns)
            {
                var emp = await _context.Employee.FindAsync(a.ID_Employee);
                var mat = await _context.Material.FindAsync(a.ID_Material);
                results.Add(new
                {
                    a.ID,
                    EmployeeName = $"{emp?.Prename} {emp?.Name}",
                    MaterialName = mat?.Name,
                    a.Start_Date,
                    a.End_Date,
                    a.TransactionType
                });
            }
            return Ok(results);
        }

        // Fixed — single query with JOINs via Include
        [HttpGet("details")]
        public async Task<IActionResult> GetDetails()
        {
            var assigns = await _context.Assign
                .Include(a => a.Employee)
                .Include(a => a.Material)
                .Select(a => new
                {
                    a.ID,
                    EmployeeName = a.Employee!.Prename + " " + a.Employee.Name,
                    MaterialName = a.Material!.Name,
                    a.Start_Date,
                    a.End_Date,
                    a.TransactionType
                })
                .ToListAsync();
            return Ok(assigns);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Assign), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var assign = await _context.Assign.FindAsync(id);
            return assign == null ? NotFound() : Ok(assign);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Assign assign)
        {
            await _context.Assign.AddAsync(assign);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = assign.ID }, assign);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, Assign assign)
        {
            if (id != assign.ID) return BadRequest();
            _context.Entry(assign).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var assignToDelete = await _context.Assign.FindAsync(id);
            if (assignToDelete == null) return NotFound();
            _context.Assign.Remove(assignToDelete);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
