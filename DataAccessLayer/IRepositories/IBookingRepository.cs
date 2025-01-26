using DataAccessLayer.Entities;

namespace DataAccessLayer.IRepositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        IQueryable<Booking> Load(string userId);
        IQueryable<Booking> Load();
        IQueryable<Booking> LoadApproved();
        Task CreateAsync(Booking booking);
        Task EditAsync(Booking booking);
        Task DeleteAsync(int bookingId);
        Task<bool> IsBookingExists(string userId, int roomId, DateTime start, DateTime end);
        Task<bool> IsBookingExists(int roomId, DateTime start, DateTime end);
        Task ApproveAsync(int bookingId);
        Task<Booking> GetByIdAsync(int id);
    }
}
