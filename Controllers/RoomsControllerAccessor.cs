using cw5.Models;

namespace cw5.Controllers;

public class RoomsControllerAccessor
{
    public static List<Room> Rooms => RoomsController.rooms;
}