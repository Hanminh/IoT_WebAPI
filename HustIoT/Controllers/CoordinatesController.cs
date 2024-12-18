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
        [HttpGet("iot/get-all")]
        public async Task<IActionResult> GetAll()
        {
            var coordinates = await _context.Coordinates.ToListAsync();
            return Ok(coordinates);
        }
        // Get a coordinate by id
        [HttpGet("iot/by-id")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var coordinate = await _context.Coordinates.FindAsync(id);
            if (coordinate == null) return NotFound();
            return Ok(coordinate);
        }
        // get coordinates by time (day)
        [HttpGet("iot/by-date")]
        public async Task<IActionResult> GetByDay([FromQuery] DateTime time)
        {
            var coordinates = await _context.Coordinates.Where(c => c.Time.Date == time.Date).ToListAsync();
            if (coordinates == null) return NotFound();
            return Ok(coordinates);
        }
        // Get the latest coordinate with route
        [HttpGet("iot/latest")]
        public async Task<IActionResult> GetLatest()
        {
            var coordinate = await _context.Coordinates.OrderByDescending(c => c.Id).FirstOrDefaultAsync();
            if (coordinate == null) return NotFound();
            return Ok(coordinate);
        }

        // Add a new coordinate
        [HttpPost("iot/single-coordinate")]
        //[HttpPost("iot/single-coordinate")]
        public async Task<ActionResult<Coordinate>> Create([FromBody] Coordinate coordinate)
        {
            if (coordinate == null)
                return BadRequest("Invalid coordinate data");

            if (!IsValidCoordinate(coordinate))
                return BadRequest("Longitude and latitude must be between -90 and 90");

            _context.Coordinates.Add(coordinate);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = coordinate.Id }, coordinate);
        }

        [HttpPost("iot/multi-coordinates")]
        public async Task<ActionResult<IEnumerable<Coordinate>>> CreateMultiple([FromBody] IEnumerable<Coordinate> coordinates)
        {
            if (coordinates == null || !coordinates.Any())
                return BadRequest("Invalid or empty coordinates data");

            // Validate each coordinate
            foreach (var coord in coordinates)
            {
                if (!IsValidCoordinate(coord))
                    return BadRequest($"Invalid coordinate data: Longitude and latitude must be between -90 and 90 for ID {coord.Id}");
            }

            _context.Coordinates.AddRange(coordinates);
            await _context.SaveChangesAsync();

            return Ok(coordinates);
        }

        // Utility method to validate longitude and latitude
        private bool IsValidCoordinate(Coordinate coordinate)
        {
            return coordinate.Latitude is >= -90 and <= 90;
        }



        // Update an existing coordinate
        [HttpPut]
        [Microsoft.AspNetCore.Mvc.Route("iot/put-id")]
        public async Task<ActionResult<Coordinate>> Update([FromBody] Coordinate coordinate)
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
        [HttpDelete("iot/delete-by-id")]
        //[Microsoft.AspNetCore.Mvc.Route("iot/delete-id")]
        public async Task<ActionResult<Coordinate>> Delete([FromBody] int id)
        {
            var coordinate = await _context.Coordinates.FindAsync(id);
            if (coordinate == null) return NotFound();

            _context.Coordinates.Remove(coordinate);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("iot/delete-all")]
        public async Task<ActionResult> DeleteAll()
        {
            _context.Coordinates.RemoveRange(_context.Coordinates);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
