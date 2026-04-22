using Microsoft.AspNetCore.Mvc;
using cw5.Models;

namespace cw5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private static List<Reservation> reservations = new()
    {
        new Reservation
        {
            Id = 1,
            RoomId = 1,
            OrganizerName = "Jan Kowalski",
            Topic = "C# Basics",
            Date = DateTime.Today,
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            Status = "confirmed"
        }
    };

    private static List<Room> rooms = RoomsControllerAccessor.Rooms;
    static ReservationsController()
    {
        ReservationsControllerAccessor.Reservations = reservations;
    }
    // GET by id
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var res = reservations.FirstOrDefault(r => r.Id == id);

        if (res == null)
            return NotFound();

        return Ok(res);
    }
    [HttpGet]
    public IActionResult Get(
        [FromQuery] DateTime? date,
        [FromQuery] string? status,
        [FromQuery] int? roomId)
    {
        var result = reservations.AsQueryable();

        if (date.HasValue)
            result = result.Where(r => r.Date.Date == date.Value.Date);

        if (!string.IsNullOrEmpty(status))
            result = result.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

        if (roomId.HasValue)
            result = result.Where(r => r.RoomId == roomId.Value);

        return Ok(result.ToList());
    }
    // POST
    [HttpPost]
    public IActionResult Create([FromBody] Reservation reservation)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var room = rooms.FirstOrDefault(r => r.Id == reservation.RoomId);
        if (room == null)
            return NotFound("Room does not exist");

        
        if (!room.IsActive)
            return BadRequest("Room is not active");
        
        if (reservation.EndTime <= reservation.StartTime)
            return BadRequest("EndTime must be after StartTime");
        
        bool conflict = reservations.Any(r =>
            r.RoomId == reservation.RoomId &&
            r.Date.Date == reservation.Date.Date &&
            reservation.StartTime < r.EndTime &&
            reservation.EndTime > r.StartTime
        );
        
        if (conflict)
            return Conflict("Reservation time conflict");

        reservation.Id = reservations.Any() ? reservations.Max(r => r.Id) + 1 : 1;

        reservations.Add(reservation);

        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
        
    }

    // PUT
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Reservation updated)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = reservations.FirstOrDefault(r => r.Id == id);
        if (existing == null)
            return NotFound();

        var room = rooms.FirstOrDefault(r => r.Id == updated.RoomId);
        if (room == null)
            return NotFound("Room does not exist");

        if (!room.IsActive)
            return BadRequest("Room is not active");

        if (updated.EndTime <= updated.StartTime)
            return BadRequest("EndTime must be after StartTime");

        bool conflict = reservations.Any(r =>
            r.Id != id && 
            r.RoomId == updated.RoomId &&
            r.Date.Date == updated.Date.Date &&
            updated.StartTime < r.EndTime &&
            updated.EndTime > r.StartTime
        );

        if (conflict)
            return Conflict("Reservation time conflict");

        existing.RoomId = updated.RoomId;
        existing.OrganizerName = updated.OrganizerName;
        existing.Topic = updated.Topic;
        existing.Date = updated.Date;
        existing.StartTime = updated.StartTime;
        existing.EndTime = updated.EndTime;
        existing.Status = updated.Status;

        return Ok(existing);
    }

    // DELETE
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var res = reservations.FirstOrDefault(r => r.Id == id);

        if (res == null)
            return NotFound();

        reservations.Remove(res);

        return NoContent();
    }
}