namespace MatrioshkaBookingSystem.Models
{
    public class AdminViewModel
    {
        public List<User> Users { get; set; }
        public List<Hotel> Hotels { get; set; }
        public List<Floor> Floors { get; set; }
        public List<Room> Rooms { get; set; }

        public List<Booking> Bookings { get; set; } 
    }
}
