using DataAccessLayer.IRepositories;
using DataAccessLayer.Entities;
using ServiceLayer.IServices;

namespace ServiceLayer.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }
        public async Task<IList<Room>> GetAllAsync()
        {
            var roomsWithFacilities = await _roomRepository.GetAllAsync();

            return roomsWithFacilities;
        }

        public async Task<Room> GetByIdAsync(int roomId)
        {
            var roomWithFacility = await _roomRepository.GetByIdAsync(roomId);
            if (roomWithFacility == null)
            {
                throw new Exception("Room doesn't exists");
            }

            return roomWithFacility;
        }

        public async Task CreateAsync(Room room)
        {
            var titleExist = _roomRepository.IsTitleExists(room.Title);

            if (titleExist == true)
            {
                throw new Exception("Room Already Exist");
            }

            await _roomRepository.CreateAsync(room);
        }

        public async Task UpdateAsync(Room room)
        {
            var titleExist = _roomRepository.IsTitleExists(room.Id, room.Title);

            if (titleExist == true)
            {
                throw new Exception("Room Already Exist");
            }

            await _roomRepository.UpdateAsync(room);
        }

        public async Task DeleteAsync(int roomId)
        {
            await _roomRepository.DeleteAsync(roomId);
        }
    }
}
