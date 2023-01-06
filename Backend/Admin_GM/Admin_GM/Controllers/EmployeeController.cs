using Admin_GM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Admin_GM.Data;
//using Admin_GM.Models;


namespace Admin_GM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public EmployeeController( EmployeeDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Employee>> Get()
           => await _context.Employee.ToListAsync();

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var Employee = await _context.Employee.FindAsync(id);
            return Employee == null ? NotFound() : Ok(Employee);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Employee Employee)
        {
            await _context.Employee.AddAsync(Employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = Employee.ID }, Employee);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, Employee employee)
        {
            if (id != employee.ID) return BadRequest(); //checks if ID exists

            _context.Entry(employee).State = EntityState.Modified; // Modify the object in the table
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")] //means that the id could be found in the url
        [ProducesResponseType(StatusCodes.Status204NoContent)] //code sent if operaion is successful
        [ProducesResponseType(StatusCodes.Status404NotFound)]  //code sent if operaion is unsuccessful
        public async Task<IActionResult> Delete(int id)
        {
            var EmployeeToDelete = await _context.Employee.FindAsync(id);
            if (EmployeeToDelete == null) return NotFound(); // checks wether obj exists or not

            _context.Employee.Remove(EmployeeToDelete);
            await _context.SaveChangesAsync(); //delete and save the obj

            return NoContent(); // no return value
        }

    }

}
