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

        [HttpGet] //get all data from the table 
        public async Task<IEnumerable<Assign>> Get()
           => await _context.Assign.ToListAsync();

        [HttpGet("{id}")] //same shit but with id, so only one row
        [ProducesResponseType(typeof(Assign), StatusCodes.Status200OK)] //good
        [ProducesResponseType(StatusCodes.Status404NotFound)] //bad
        public async Task<IActionResult> GetById(int id)
        {
            var Assign = await _context.Assign.FindAsync(id);
            return Assign == null ? NotFound() : Ok(Assign);
        }

        [HttpPost] // add new row with data
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Assign Assign)
        {
            await _context.Assign.AddAsync(Assign);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = Assign.ID }, Assign);
        }

        [HttpPut("{id}")] //delete
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, Assign Assign)
        {
            if (id != Assign.ID) return BadRequest(); //checks if ID exists

            _context.Entry(Assign).State = EntityState.Modified; // Modify the object in the table
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")] //means that the id could be found in the url
        [ProducesResponseType(StatusCodes.Status204NoContent)] //code sent if operaion is successful
        [ProducesResponseType(StatusCodes.Status404NotFound)]  //code sent if operaion is unsuccessful
        public async Task<IActionResult> Delete(int id)
        {
            var AssignToDelete = await _context.Assign.FindAsync(id);
            if (AssignToDelete == null) return NotFound(); // checks wether obj exists or not

            _context.Assign.Remove(AssignToDelete);
            await _context.SaveChangesAsync(); //delete and save the obj

            return NoContent(); // no return value
        }
    }
}
