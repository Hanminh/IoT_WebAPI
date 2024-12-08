using HustIoT.Data;
using HustIoT.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HustIoT.Controllers
{
    public class CoordinatesController : Controller
    {
        private readonly AppDbContext _context;
        public IActionResult Index()
        {
            return View();
        }
        public CoordinatesController(AppDbContext context)
        {
            _context = context;
        }

        // Get all the coordinates
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var coordinates = await _context.Coordinates.ToListAsync();
            return Ok(coordinates);
        }
        // Get a coordinate by id
        [HttpGet("by-id")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var coordinate = await _context.Coordinates.FindAsync(id);
            if (coordinate == null) return NotFound();
            return Ok(coordinate);
        }
        // get coordinates by time (day)
        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDay([FromQuery] DateTime time)
        {
            var coordinates = await _context.Coordinates.Where(c => c.Time.Date == time.Date).ToListAsync();
            if (coordinates == null) return NotFound();
            return Ok(coordinates);
        }
        // Get the latest coordinate
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            var coordinate = await _context.Coordinates.OrderByDescending(c => c.Time).FirstOrDefaultAsync();
            if (coordinate == null) return NotFound();
            return Ok(coordinate);
        }

        // Add a new coordinate
        [HttpPost] 
        public async Task<IActionResult> Create([FromBody] Coordinate coordinate)
        {
            if (coordinate == null)
                return BadRequest("Invalid coordinate data");

            _context.Coordinates.Add(coordinate);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new {id = coordinate.Id}, coordinate);
        }
        // Update an existing coordinate
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Coordinate coordinate)
        {
            if(coordinate == null || coordinate.Id <= 0)
                return BadRequest("Invalid coordinate data");
            
            var existinggCoordinate = await _context.Coordinates.FindAsync(coordinate.Id);
            if (existinggCoordinate == null)
                return NotFound("Coordinate not found");

            existinggCoordinate.Longitude = coordinate.Longitude;
            existinggCoordinate.Latitude = coordinate.Latitude;
            existinggCoordinate.Time = coordinate.Time;

            await _context.SaveChangesAsync();

            // return update successfull message
            return Ok("Coordinate updated successfully");
        }

        // Delete a coordinate
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            var coordinate = await _context.Coordinates.FindAsync(id);
            if (coordinate == null) return NotFound();

            _context.Coordinates.Remove(coordinate);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
