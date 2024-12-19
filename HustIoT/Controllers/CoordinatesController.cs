using HustIoT.Data;
using HustIoT.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;

namespace HustIoT.Controllers
{
    public class CoordinatesController : Controller
    {
        private readonly AppDbContext _context;
        string privateKey = "MIIEowIBAAKCAQEAuqvwyKfMPcEkSElMM59pBNFLJLIAqJYWdHe6w7oaHf9sPNTQ3g+/E9dUuZH8TWqimPr5Wq/2pD" +
            "mD8D4wnXeNe09ldsPFxGMrLxdHEscin56+SAVoX1O0bumSUIKiODHLTNkxAIibZkUbPSJZDySRLAoQ+21e9JL6/ocRMN21W37CF/HVPBB5JPLIO" +
            "go2zqg3VX9DUIKQG72Wh8b6TGMwDE4FIQQXcsTA1UuCVEC41B0FQnygA6IdK11TTart5WMFRhWufcI/yZL7MF+/4myob5m5ESa4oQWHT7twHOjpf" +
            "o7uJRF9PaB7lRMWQH5sEnQqBdjNUicFpTPR0D7XxKmLDQIDAQABAoIBAFAKi8suD+hm2azZKQB1mO1E5MiOPrQK7wPvlrh4Ity7+eg3hHvDGrZiPW" +
            "T4kMuNLr0T2Dmne8U9GDK6J9RNP/Agwivjk/g6YXjmrUYC9vikc4ikMPW3CYYJRwCYkwAvcbG460cETEXU2fIjuTZCArF0e4WjhvSt1UuwFJH0buA" +
            "UxKdrvenpLh054UaPhR4NzpElggqtq0Xpi5Yp80+M7iMcLh1JzuOdouLFd0B92HhvC0aAAIPi7ikvfLDpJPn6hhaXFtkmsaBqwUiSSiuoShNKelAP2" +
            "+a9ATXFqUqVAYRsKFePyTvVHs7yGzxi+BfiuhIEgiT6IGmXplOYgN7PSw0CgYEAxVsLJ62qKLmYO3c9+Y32dso1ih39hoSWEDIxnetjaB/dEPq/j4k" +
            "XKqefMb2WGZXXtfcDd7FWXsLvnDEQy+sjr5rYp4BCIIjAnEn6Qi3a8rD1bszmuJIL9BEpIwtBJYyFTMB1n88axJd5YpWKrl3Yw/fqzHO6d0G7MQu4qQ" +
            "wn7B8CgYEA8iQoxuvwtDA2L/G7/8OdYrh9uLjQEV12Yqvb/IAwiAjqivO8Obxu3BgK8wB/FfGmpxeNiVNikFsYQWqDI98MrGB2QJcuYU8MUCE0kpz/V" +
            "5HCyO1UeIFjYQ86Xuwek7tAyNUuDhB6AwD7Ia9JXuGAsplTWcJ/21M1lEQ6yK+YY1MCgYBg/vgijjX9QgpR3680AdPKWmOp+EdsX6mpWCIOrWvz9wUd" +
            "nT+c+hHKwwt41Ob6uCyGoFqx7xS2CjTdnTfWIUEuw3oMCPt3Jf8UUT+QWx0q/lICHO6gdBcv42dGc/eWztOM/2JQIufTC2d7TmgUfsdUuXpK9e4FQuc" +
            "PmeUuIG1WnQKBgA2gS0sma903+VSpXdL+xxSPHUQP4mWXxNm4oiCLdi+xkMFRBf6ZxANOtw8FsCEkACTXBnf74UgOWEcWH1sdajEpHH52A34mXKMFu1" +
            "ekzhm3ciasdFxzq4wCt14wG2hk2Th0Bqtz8enJXFiA7LgSKJPkXPRoJZnKDPHTBRvWBdVRAoGBAJd7evuesoSfjHQP84lqZvIz6vx2BSaxVp5fzrd8gB" +
            "bTfMTaVND8XPEiijePx2qvdByIhEAUwzSCWJVuTf1L5c1WZVN35la+lS8VbJdFXs5vh7e3YSXxzAnD9WbLXZYzffScysw4s/3FA4qgHTHKfZKp87fbXQXxyt65iyrun4gF";

        string publicKey = "MIIBCgKCAQEAuqvwyKfMPcEkSElMM59pBNFLJLIAqJYWdHe6w7oaHf9sPNTQ3g+/E9dUuZH8TWqimPr5Wq/2pDmD8D4wnXeNe0" +
            "9ldsPFxGMrLxdHEscin56+SAVoX1O0bumSUIKiODHLTNkxAIibZkUbPSJZDySRLAoQ+21e9JL6/ocRMN21W37CF/HVPBB5JPLIOgo2zqg3VX9DUIKQG72Wh8b" +
            "6TGMwDE4FIQQXcsTA1UuCVEC41B0FQnygA6IdK11TTart5WMFRhWufcI/yZL7MF+/4myob5m5ESa4oQWHT7twHOjpfo7uJRF9PaB7lRMWQH5sEnQqBdjNUicF" +
            "pTPR0D7XxKmLDQIDAQAB";

        public IActionResult Index()
        {
            return View();
        }
        public CoordinatesController(AppDbContext context)
        {
            _context = context;
        }

        private string SignData(string data, string privateKey)
        {
            byte[] privateKeyBytes = Convert.FromBase64String(privateKey);
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] signedBytes = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(signedBytes);
            }
        }

        // Get all the coordinates
        [HttpGet("iot/get-all")]
        public async Task<IActionResult> GetAll()
        {
            var coordinates = await _context.Coordinates.ToListAsync();
            var dataToSign = Newtonsoft.Json.JsonConvert.SerializeObject(coordinates);
            string signature = SignData(dataToSign, privateKey);
            var result = new
            {
                Data = coordinates,
                Signature = signature,
                //RetrievedAt = DateTime.UtcNow
            };
            return Ok(result);
        }
        // Get a coordinate by id
        [HttpGet("iot/by-id")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var coordinate = await _context.Coordinates.FindAsync(id);
            if (coordinate == null) return NotFound("Not exist");
            var dataToSign = Newtonsoft.Json.JsonConvert.SerializeObject(coordinate);
            string signature = SignData(dataToSign, privateKey);
            var result = new
            {
                Data = coordinate,
                Signature = signature,
                //RetrievedAt = DateTime.UtcNow
            };
            return Ok(result);
        }
        // get coordinates by time (day)
        [HttpGet("iot/by-date")]
        public async Task<IActionResult> GetByDay([FromQuery] DateTime time)
        {
            var coordinates = await _context.Coordinates.Where(c => c.Time.Date == time.Date).ToListAsync();
            if (coordinates == null) return NotFound("Not exist");
            var dataToSign = Newtonsoft.Json.JsonConvert.SerializeObject(coordinates);
            string signature = SignData(dataToSign, privateKey);
            var result = new
            {
                Data = coordinates,
                Signature = signature,
                //RetrievedAt = DateTime.UtcNow
            };
            return Ok(result);
        }
        // Get the latest coordinate with route
        [HttpGet("iot/latest")]
        public async Task<IActionResult> GetLatest()
        {
            var coordinate = await _context.Coordinates.OrderByDescending(c => c.Id).FirstOrDefaultAsync();
            if (coordinate == null) return NotFound("Not exist");
            var dataToSign = Newtonsoft.Json.JsonConvert.SerializeObject(coordinate);
            string signature = SignData(dataToSign, privateKey);
            var result = new
            {
                Data = coordinate,
                Signature = signature,
                //RetrievedAt = DateTime.UtcNow
            };
            return Ok(result);
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
