using DataAccessLayer.Entities;
using DataAccessLayer.IRepositories;
using ServiceLayer.IServices;

namespace ServiceLayer.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task CreateAsync(Booking booking)
        {
            var time = (booking.EventEnd - booking.EventStart).TotalHours;

            if (time < 0.30 || time > 4.0 || booking.EventStart < DateTime.Now)
            {
                throw new Exception("Invalid Meeting Time. Max Duration 4 hours");
            }

            var existingBooking = await _bookingRepository.IsBookingExists(booking.UserId, booking.RoomId, booking.EventStart, booking.EventEnd);

            if (existingBooking == true)
            {
                throw new Exception("Already have a Booking. Select Another Date or Time");
            }

            await _bookingRepository.CreateAsync(booking);
        }
        
        public async Task CreatedByAdminAsync(Booking booking)
        {
            var time = (booking.EventEnd - booking.EventStart).TotalHours;

            if (time < 0.30 || time > 4.0 || booking.EventStart < DateTime.Now)
            {
                throw new Exception("Invalid Meeting Time. Max Duration 4 hours");
            }

            var existingBooking = await _bookingRepository.IsBookingExists(booking.RoomId, booking.EventStart, booking.EventEnd);

            if (existingBooking == true)
            {
                throw new Exception("Already have a Booking. Select Another Date or Time");
            }

            await _bookingRepository.CreateAsync(booking);
        }

        public async Task EditAsync(Booking booking)
        {
            var time = (booking.EventEnd - booking.EventStart).TotalHours;

            if (time < 0.30 || time > 4.0 || booking.EventStart < DateTime.Now)
            {
                throw new Exception("Invalid Meeting Time. Max Duration 4 hours");
            }

            await _bookingRepository.EditAsync(booking);
        }
        
        public async Task ApproveAsync(int bookingId)
        {
            await _bookingRepository.ApproveAsync(bookingId);
        }

        public async Task DeleteAsync(int bookingId)
        {
            await _bookingRepository.DeleteAsync(bookingId);
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        public IQueryable<Booking> Load()
        {
            return _bookingRepository.Load();
        }

        public IQueryable<Booking> Load(string userId)
        {
            return _bookingRepository.Load(userId);
        }

        public IQueryable<Booking> LoadApproved()
        {
            return _bookingRepository.LoadApproved();
        }

        public async Task<Booking> GetByIdAsync(int id)
        {
            return await _bookingRepository.GetByIdAsync(id);
        }
    }
}
