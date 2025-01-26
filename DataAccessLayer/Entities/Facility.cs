using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public class Facility
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<Room> Rooms { get; set; }
    }
}
