using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.IRepositories;
using Microsoft.EntityFrameworkCore;
namespace DataAccessLayer.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _dbContext;
        public BookingRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateAsync(Booking booking)
        {
            await _dbContext.Bookings.AddAsync(booking);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int bookingId)
        {
            var booking = await _dbContext.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                _dbContext.Bookings.Remove(booking);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Booking Not Found!");
            }          
        }

        public async Task EditAsync(Booking booking)
        {
            _dbContext.Bookings.Update(booking);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            var bookings = await _dbContext.Bookings.Include(x => x.Rooms).ToListAsync();

            return bookings;
        }

        public IQueryable<Booking> Load(string userId)
        {
            var bookings =  _dbContext.Bookings.Include(x => x.Rooms).Where(x => x.UserId == userId);

            return bookings;
        }

        public IQueryable<Booking> Load()
        {
            var notApproved = _dbContext.Bookings.Include(x => x.Rooms).Where(x => x.IsApproved == false);

            return notApproved;
        }

        public IQueryable<Booking> LoadApproved()
        {
            var approved =  _dbContext.Bookings.Include(x => x.Rooms).Where(x => x.IsApproved == true);

            return approved;
        }

        public async Task<bool> IsBookingExists(string userId, int roomId, DateTime start, DateTime end)
        {
            var existingBookings = await _dbContext.Bookings.Where(x => (x.UserId == userId && x.EventStart < end && x.EventEnd > start) || (x.RoomId == roomId && x.EventStart < end && x.EventEnd > start)).ToListAsync();

            return existingBookings.Any();
        }

        public async Task<bool> IsBookingExists(int roomId, DateTime start, DateTime end)
        {
            var existingBookings = await _dbContext.Bookings.Where(x => x.RoomId == roomId && x.EventStart < end && x.EventEnd > start).ToListAsync();

            return existingBookings.Any();
        }

        public async Task ApproveAsync(int bookingId)
        {
            var booking = await _dbContext.Bookings.FindAsync(bookingId);

            if (booking != null)
            {
                booking.IsApproved = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Booking> GetByIdAsync(int id)
        {
            var booking = await _dbContext.Bookings.FindAsync(id);

            return booking;
        }
    }
}
