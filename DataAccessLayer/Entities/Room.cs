using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;


namespace DataAccessLayer.Entities
{
    public class Room
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public string RoomColor { get; set; }
        public List<Facility>? Facilities { get; set; }
        public List<Booking>? Bookings { get; set; }
        public List<string> RoomImages { get; set; }
    }
}
