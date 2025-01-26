namespace meeting_room_booking.ViewModels
{
    public class BookingViewModel
    {
        public int id { get; set; }
        public string UserId { get; set; }
        public int resourceId { get; set; }
        public string title { get; set; }
        public string? backgroundColor { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public extendedProps extendedProps { get; set; }
    }
}

public class extendedProps
{
    public string userId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Anything { get; set; }
}