using DataAccessLayer.Entities;

namespace DataAccessLayer.IRepositories
{
    public interface IRoomRepository
    {
        Task<IList<Room>> GetAllAsync();
        Task<Room> GetByIdAsync(int roomId);
        Task CreateAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(int roomId);
        bool IsTitleExists(string roomTitle);
        bool IsTitleExists(int roomId, string roomTitle);
    }
}
