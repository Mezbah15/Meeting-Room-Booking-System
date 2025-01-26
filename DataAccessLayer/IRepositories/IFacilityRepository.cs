using DataAccessLayer.Entities;

namespace DataAccessLayer.IRepositories
{
    public interface IFacilityRepository
    {
        Task<IEnumerable<Facility>> GetAllAsync();
        Task<Facility> GetByIdAsync(int facilityId);
        Task CreateAsync(Facility facility);
        Task UpdateAsync(Facility facility);
        Task DeleteAsync(int facilityId);
        Task<List<Facility>> GetByIdAsync(List<int> facilityIds);
        Task<bool> IsNameExistsAsync(string facilityName);
    }
}
