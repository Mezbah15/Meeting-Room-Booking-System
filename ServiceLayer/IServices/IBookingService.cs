using DataAccessLayer.Entities;

namespace ServiceLayer.IServices
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAllAsync();
        IQueryable<Booking> Load(string userId);
        IQueryable<Booking> Load();
        IQueryable<Booking> LoadApproved();       
        Task CreateAsync(Booking booking);
        Task CreatedByAdminAsync(Booking booking);
        Task EditAsync(Booking booking);
        Task DeleteAsync(int bookingId);
        Task ApproveAsync(int bookingId);
        Task<Booking> GetByIdAsync(int id);
    }
}
