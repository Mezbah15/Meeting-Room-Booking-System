using System.ComponentModel.DataAnnotations;

namespace meeting_room_booking.ViewModels
{
    public class UserViewModel
    {
        [Required]
        [StringLength(6, ErrorMessage = "The {0} must be at least {2} digit long.", MinimumLength = 4)]
        public string Pin { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string Name { get; set; }
        [Required]
        public int PhoneNumber { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string Department { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public IFormFile? CSVFile { get; set; }
    }
}
