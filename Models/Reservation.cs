using System.ComponentModel.DataAnnotations;

namespace cw5.Models;

public class Reservation
{
    public int Id { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Required]
    public string OrganizerName { get; set; }

    [Required]
    public string Topic { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [Required]
    public string Status { get; set; }
}