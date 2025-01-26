using DataAccessLayer.Entities;

namespace ServiceLayer.IServices
{
    public interface IFacilityService
    {
        Task<IEnumerable<Facility>> GetAllAsync();
        Task<Facility> GetByIdAsync(int facilityId);
        Task CreateAsync(Facility facility);
        Task UpdateAsync(Facility facility);
        Task DeleteAsync(int facilityId);
        Task<List<Facility>> GetByIdAsync(List<int> selectedFacilities);
    }
}
