using DataAccessLayer.IRepositories;
using DataAccessLayer.Entities;
using ServiceLayer.IServices;

namespace ServiceLayer.Services
{
    public class FacilityService : IFacilityService
    {
        private readonly IFacilityRepository _facilityRepository;
        public FacilityService(IFacilityRepository facilityRepository)
        {
            _facilityRepository = facilityRepository;
        }

        public async Task<IEnumerable<Facility>> GetAllAsync()
        {
            return await _facilityRepository.GetAllAsync();
        }

        public async Task<Facility> GetByIdAsync(int facilityId)
        {
            var facility = await _facilityRepository.GetByIdAsync(facilityId);

            if (facility == null)
            {
                throw new Exception("Facility Doesn't Exists");
            }

            return facility;
        }

        public async Task<List<Facility>> GetByIdAsync(List<int> selectedFacilities)
        {
            var facilityList = await _facilityRepository.GetByIdAsync(selectedFacilities);

            return facilityList;
        }

        public async Task CreateAsync(Facility facility)
        {
            var facilityExists = await _facilityRepository.IsNameExistsAsync(facility.Name);

            if (facilityExists == true)
            {
                throw new Exception($"{facility.Name} Already Exists");
            }

            await _facilityRepository.CreateAsync(facility);
        }

        public async Task UpdateAsync(Facility facility)
        {
            var facilityExists = await _facilityRepository.IsNameExistsAsync(facility.Name);

            if (facilityExists == true)
            {
                throw new Exception($"{facility.Name} Already Exists");
            }

            await _facilityRepository.UpdateAsync(facility);
        }

        public async Task DeleteAsync(int facilityId)
        {
            await _facilityRepository.DeleteAsync(facilityId);
        }
    }
}
