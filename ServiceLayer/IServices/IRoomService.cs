using DataAccessLayer.Entities;

namespace ServiceLayer.IServices
{
    public interface IRoomService
    {
        Task<IList<Room>> GetAllAsync();
        Task<Room> GetByIdAsync(int roomId);
        Task CreateAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(int roomId);
    }
}
