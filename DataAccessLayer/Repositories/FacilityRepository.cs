using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class FacilityRepository : IFacilityRepository
    {
        private readonly AppDbContext _dbContext;
        public FacilityRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Facility>> GetAllAsync()
        {
            return await _dbContext.Facilities.ToListAsync();
        }

        public async Task<Facility> GetByIdAsync(int facilityId)
        {
            return await _dbContext.Facilities.FindAsync(facilityId);
        }

        //This method overloading is to get selected facilities.
        public async Task<List<Facility>> GetByIdAsync(List<int> facilityIds)
        {
            var facilityList = await _dbContext.Facilities.Where(x => facilityIds.Contains(x.Id)).ToListAsync();

            return facilityList;
        }

        //This method is to prevent duplicate facility create.
        public async Task<bool> IsNameExistsAsync(string facilityName)
        {
            var isNameExists = await _dbContext.Facilities.AnyAsync(x => x.Name == facilityName);

            return isNameExists;
        }

        public async Task CreateAsync(Facility facility)
        {
            await _dbContext.Facilities.AddAsync(facility);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Facility facility)
        {
            _dbContext.Facilities.Update(facility);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int facilityId)
        {
            var facility = await _dbContext.Facilities.FindAsync(facilityId);

            _dbContext.Facilities.Remove(facility);
            await _dbContext.SaveChangesAsync();
        }
    }
}
