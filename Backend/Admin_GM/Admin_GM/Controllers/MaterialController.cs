using Admin_GM.Data;
using Admin_GM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin_GM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase

    {
        private readonly EmployeeDbContext _context;
        public MaterialController(EmployeeDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Material>> Get()
           => await _context.Material.ToListAsync();

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Material), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var Material = await _context.Material.FindAsync(id);
            return  Material == null ? NotFound() : Ok(Material);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Material Material)
        {
            await _context.Material.AddAsync(Material);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = Material.ID }, Material);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, Material Material)
        {
            if (id != Material.ID) return BadRequest(); //checks if ID exists

            _context.Entry(Material).State = EntityState.Modified; // Modify the object in the table
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")] //means that the id could be found in the url
        [ProducesResponseType(StatusCodes.Status204NoContent)] //code sent if operaion is successful
        [ProducesResponseType(StatusCodes.Status404NotFound)]  //code sent if operaion is unsuccessful
        public async Task<IActionResult> Delete(int id)
        {
            var MaterialToDelete = await _context.Material.FindAsync(id);
            if (MaterialToDelete == null) return NotFound(); // checks wether obj exists or not

            _context.Material.Remove(MaterialToDelete);
            await _context.SaveChangesAsync(); //delete and save the obj

            return NoContent(); // no return value
        }
    }
}
