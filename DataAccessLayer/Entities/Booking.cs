using DataAccessLayer.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string UserId { get; set; }
        public string EventTitle { get; set; }
        public bool IsApproved { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime EventEnd { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        [ForeignKey("RoomId")]
        public Room Rooms { get; set; }
    }
}
