using DataAccessLayer.Entities;

namespace meeting_room_booking.ViewModels
{
    public class RoomViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public string RoomColor { get; set; }
        public List<string>? Facilities { get; set; }
        public List<int>? SelectedFacilities { get; set; }
        public List<RoomFacilitiesVM>? FacilitiesForRoom{ get; set; }
        public List<IFormFile>? RoomImages { get; set; }
        public List<string>? ImagePaths { get; set; }
    }
}
