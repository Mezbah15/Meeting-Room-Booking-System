using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _dbContext;
        public RoomRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<Room>> GetAllAsync()
        {
            var roomsWithFacilities = await _dbContext.Rooms.Include(x => x.Facilities).ToListAsync();

            return roomsWithFacilities;
        }

        public async Task<Room> GetByIdAsync(int roomId)
        {
            var roomWithFacilities = await _dbContext.Rooms.Include(a => a.Facilities).FirstOrDefaultAsync(a => a.Id == roomId);

            return roomWithFacilities;
        }

        public bool IsTitleExists(string title)
        {
            bool exists = _dbContext.Rooms.Any(x => x.Title.ToLower() == title.ToLower());

            return exists;
        }

        public bool IsTitleExists(int roomId, string title)
        {
            bool exists = _dbContext.Rooms.Any(x => x.Title.ToLower() == title.ToLower() && x.Id != roomId);

            return exists;
        }

        public async Task CreateAsync(Room room)
        {
            await _dbContext.Rooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var room = await _dbContext.Rooms.FindAsync(id);

            _dbContext.Rooms.Remove(room);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Room room)
        {
            _dbContext.Rooms.Update(room);
            await _dbContext.SaveChangesAsync();
        }
    }
}
