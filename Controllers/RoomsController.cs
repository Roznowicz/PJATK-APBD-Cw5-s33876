namespace cw5.Controllers;

using Microsoft.AspNetCore.Mvc;
using cw5.Models;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    public static List<Room> rooms = new()
    {
        new Room { Id = 1, Name = "Room A", BuildingCode = "A", Floor = 1, Capacity = 20, HasProjector = true, IsActive = true },
        new Room { Id = 2, Name = "Room B", BuildingCode = "A", Floor = 2, Capacity = 15, HasProjector = false, IsActive = true },
        new Room { Id = 3, Name = "Room C", BuildingCode = "B", Floor = 1, Capacity = 30, HasProjector = true, IsActive = true }
    };

    private static List<Reservation> reservations = ReservationsControllerAccessor.Reservations;

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var room = rooms.FirstOrDefault(r => r.Id == id);

        if (room == null)
            return NotFound();

        return Ok(room);
    }
    [HttpGet]
    public IActionResult Get(
        [FromQuery] int? minCapacity,
        [FromQuery] bool? hasProjector,
        [FromQuery] bool? activeOnly)
    {
        var result = rooms.AsQueryable();

        if (minCapacity.HasValue)
            result = result.Where(r => r.Capacity >= minCapacity.Value);

        if (hasProjector.HasValue)
            result = result.Where(r => r.HasProjector == hasProjector.Value);

        if (activeOnly == true)
            result = result.Where(r => r.IsActive);

        return Ok(result.ToList());
    }
    [HttpGet("building/{buildingCode}")]
    public IActionResult GetByBuilding(string buildingCode)
    {
        var result = rooms
            .Where(r => r.BuildingCode.ToUpper() == buildingCode.ToUpper())
            .ToList();

        return Ok(result);
    }
    [HttpPost]
    public IActionResult Create([FromBody] Room room)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        room.Id = rooms.Max(r => r.Id) + 1;

        rooms.Add(room);

        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Room updatedRoom)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = rooms.FirstOrDefault(r => r.Id == id);

        if (existing == null)
            return NotFound();

        existing.Name = updatedRoom.Name;
        existing.BuildingCode = updatedRoom.BuildingCode;
        existing.Floor = updatedRoom.Floor;
        existing.Capacity = updatedRoom.Capacity;
        existing.HasProjector = updatedRoom.HasProjector;
        existing.IsActive = updatedRoom.IsActive;

        return Ok(existing);
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var room = rooms.FirstOrDefault(r => r.Id == id);

        if (room == null)
            return NotFound();
        if (reservations.Any(r => r.RoomId == id))
            return Conflict("Room has reservations");
        rooms.Remove(room);

        return NoContent();
        
    }
    
}