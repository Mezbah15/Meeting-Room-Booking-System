using DataAccessLayer.Entities;
using DataAccessLayer.HelperClass;
using Elfie.Serialization;
using meeting_room_booking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ServiceLayer.IServices;
using System.Drawing.Printing;
using System.Security.Claims;

namespace meeting_room_booking.Controllers
{
    public class BookingController : Controller
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;

        public BookingController(ILogger<BookingController> logger, IRoomService roomService, IBookingService bookingService)
        {
            _logger = logger;
            _roomService = roomService;
            _bookingService = bookingService;
        }

        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Index()
        {
            var rooms = await _roomService.GetAllAsync();
            var roomVMList = new List<RoomViewModel>();

            foreach (var room in rooms)
            {
                var FacilityList = new List<string>();
                foreach (var facility in room.Facilities)
                {
                    FacilityList.Add(facility.Name);
                }

                var roomList = new RoomViewModel
                {
                    Id = room.Id,
                    Title = room.Title,
                    Capacity = room.Capacity,
                    Facilities = FacilityList
                };
                roomVMList.Add(roomList);
            }

            var bookings = await _bookingService.GetAllAsync();
            var bookingVMList = new List<BookingViewModel>();

            foreach (var booking in bookings)
            {
                var roomColor = "#8B0000";
                if (booking.IsApproved)
                {
                    roomColor = booking.Rooms.RoomColor;
                }
                var bookingVM = new BookingViewModel
                {
                    id = booking.Id,
                    title = booking.EventTitle,
                    resourceId = booking.RoomId,
                    start = booking.EventStart,
                    end = booking.EventEnd,
                    backgroundColor = roomColor,
                    extendedProps = new extendedProps
                    {
                        userId = booking.UserId
                    }

                };
                bookingVMList.Add(bookingVM);
            }

            ViewBag.Bookings = bookingVMList;

            Log.Information("Booking Index Opened Successfully!");
            _logger.LogInformation("Booking Index Opened Successfully!");

            return View(roomVMList);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Create(BookingViewModel bookingVM)
        {
            bool isSuccess = true;
            string message;
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                if (User.IsInRole("Admin"))
                {
                    var booking = new Booking
                    {
                        RoomId = bookingVM.resourceId,
                        EventTitle = bookingVM.title,
                        EventStart = bookingVM.start,
                        EventEnd = bookingVM.end,
                        CreatedAt = DateTime.Now,
                        UserId = currentUserId,
                        IsApproved = true
                    };
                    message = "Booking Created Successfully";
                    await _bookingService.CreatedByAdminAsync(booking);

                    Log.Information($"An Admin with Id: {currentUserId} Created a Booking!");
                    _logger.LogInformation($"An Admin with Id: {currentUserId} Created a Booking!");
                }
                else
                {
                    var booking = new Booking
                    {
                        RoomId = bookingVM.resourceId,
                        EventTitle = bookingVM.title,
                        EventStart = bookingVM.start,
                        EventEnd = bookingVM.end,
                        CreatedAt = DateTime.Now,
                        UserId = currentUserId,
                        IsApproved = false
                    };

                    message = "Booking Created. Wait For Admin Approval!";
                    await _bookingService.CreateAsync(booking);

                    Log.Information($"A User with Id: {currentUserId} Created a Booking!");
                    _logger.LogInformation($"A User with Id: {currentUserId} Created a Booking!");
                }
            }

            catch (Exception ex)
            {
                message = ex.Message;
                isSuccess = false;

                _logger.LogError("Booking not created");
                Log.Error("Booking not created");
            }

            return Ok(new { isSuccess = isSuccess, message = message});
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Edit(BookingViewModel bookingVM)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isSuccess = true;
            string message;
            try
            {
                if (User.IsInRole("Admin"))
                {
                    var booking = new Booking
                    {
                        Id = bookingVM.id,
                        RoomId = bookingVM.resourceId,
                        UserId = currentUserId,
                        EventTitle = bookingVM.title,
                        EventStart = bookingVM.start,
                        EventEnd = bookingVM.end,
                        ModifiedAt = DateTime.Now,
                        IsApproved = true
                    };

                    message = "Booking Updated Successfully";
                    await _bookingService.EditAsync(booking);

                    Log.Information($"An Admin with Id: {currentUserId} Updated a Booking!");
                    _logger.LogInformation($"An Admin with Id: {currentUserId} Updated a Booking!");
                }
                else
                {

                    var booking = new Booking
                    {
                        Id = bookingVM.id,
                        RoomId = bookingVM.resourceId,
                        UserId = currentUserId,
                        EventTitle = bookingVM.title,
                        EventStart = bookingVM.start,
                        EventEnd = bookingVM.end,
                        ModifiedAt = DateTime.Now
                    };

                    message = "Booking Updated. Wait For Admin Approval";
                    await _bookingService.EditAsync(booking);

                    Log.Information($"A User with Id: {currentUserId} Updated a Booking!");
                    _logger.LogInformation($"A User with Id: {currentUserId} Updated a Booking!");
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                isSuccess = false;

                Log.Error("Booking not updated!");
                _logger.LogError("Booking not updated!");
            }

            return Ok(new
            {
                isSuccess = isSuccess,
                message = message
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = true;
            var message = "Booking Deleted Successfully";
            try
            {
                if (User.IsInRole("Admin"))
                {
                    await _bookingService.DeleteAsync(id);
                }
                else
                {
                    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    await _bookingService.DeleteAsync(id);
                }
                Log.Information("User deleted a Booking!");
                _logger.LogInformation("User deleted a Booking!");
            }
            catch (Exception ex)
            {
                message = ex.Message;
                isSuccess = false;

                _logger.LogError($"Booking not deleted! {message}");
                Log.Error($"Booking not deleted! {message}");
            }

            return Ok(new{ isSuccess = isSuccess, message = message});
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IsApproved(string searchString, string sortOrder, string currentFilter, int? pageIndex)
        {
            var notApproved =  _bookingService.Load();

            if (string.IsNullOrEmpty(searchString) == false)
            {
                notApproved = notApproved.Where(n => n.EventTitle.Contains(searchString) || n.Rooms.Title.Contains(searchString));
            }

            ViewData["TimeSortOrder"] = string.IsNullOrEmpty(sortOrder) ? "start_time_desc" : "";

            switch (sortOrder)
            {
                case "start_time_desc":
                    notApproved = notApproved.OrderByDescending(e => e.EventStart);
                    break;
                default:
                    notApproved = notApproved.OrderBy(e => e.EventStart);
                    break;
            }

            int pageSize = 3;
            ViewData["CurrentFilter"] = searchString;

            var paginatedBookingList = await PaginatedList<Booking>.CreateAsync(notApproved, pageIndex ?? 1, pageSize);

            Log.Information("Requested booking List in the Admin dashboard opened!");
            _logger.LogInformation("Requested booking List in the Admin dashboard opened!");

            return View(paginatedBookingList);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            await _bookingService.DeleteAsync(id);

            Log.Information("Requested booking in the Admin dashboard is Rejected!");
            _logger.LogInformation("Requested booking in the Admin dashboard is Rejected!");

            return RedirectToAction(nameof(IsApproved)); 
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Accept(int id)
        {
            await _bookingService.ApproveAsync(id);

            Log.Information("Requested booking in the Admin dashboard is Accepted!");
            _logger.LogInformation("Requested booking in the Admin dashboard is Accepted!");

            return RedirectToAction(nameof(IsApproved));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ScheduledBooking(string searchString, string sortOrder, string currentFilter, int? pageIndex)
        {
            var approvedBookings = _bookingService.LoadApproved();

            if (string.IsNullOrEmpty(searchString) == false)
            {
                approvedBookings = approvedBookings.Where(n => n.EventTitle.Contains(searchString)|| n.Rooms.Title.Contains(searchString));
            }

            ViewData["TimeSortOrder"] = string.IsNullOrEmpty(sortOrder) ? "time_desc" : "";

            switch (sortOrder)
            {
                case "time_desc":
                    approvedBookings =  approvedBookings.OrderByDescending(e => e.EventStart);
                    break;
                default:
                    approvedBookings = approvedBookings.OrderBy(e => e.EventStart);
                    break;
            }

            int pageSize = 5;
            ViewData["CurrentFilter"] = searchString;

            var paginatedBookingList = await PaginatedList<Booking>.CreateAsync(approvedBookings, pageIndex ?? 1, pageSize);

            Log.Information("Scheduled booking List (ScheduledBooking) in the Admin dashboard Opened!");
            _logger.LogInformation("Scheduled booking List (ScheduledBooking) in the Admin dashboard Opened!");

            return View(paginatedBookingList);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyBooking(string userId, string searchString, string sortOrder, string currentFilter, int? pageIndex)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var myBookings = _bookingService.Load(currentUserId);

            if (string.IsNullOrEmpty(searchString) == false)
            {
                myBookings = myBookings.Where(e => e.EventTitle.Contains(searchString) || e.Rooms.Title.Contains(searchString));
            }

            ViewData["TimeSortOrder"] = string.IsNullOrEmpty(sortOrder) ? "time_desc" : "";

            switch (sortOrder)
            {
                case "time_desc":
                    myBookings = myBookings.OrderByDescending(e => e.EventStart);
                    break;
                default:
                    myBookings = myBookings.OrderBy(e => e.EventStart);
                    break;
            }

            int pageSize = 5;
            ViewData["CurrentFilter"] = searchString;

            var paginatedBookingList = await PaginatedList<Booking>.CreateAsync(myBookings, pageIndex ?? 1, pageSize);

            Log.Information("Booking List(My Booking) in the user dashboard Opened!");
            _logger.LogInformation("Booking List(My Booking) in the user dashboard Opened!");

            return View(paginatedBookingList);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateByAdmin(int id)
        {
            var rooms = await _roomService.GetAllAsync();
            var roomVMList = new List<RoomViewModel>();

            foreach (var room in rooms)
            {
                var FacilityList = new List<string>();
                foreach (var facility in room.Facilities)
                {
                    FacilityList.Add(facility.Name);
                }

                var roomVM = new RoomViewModel
                {
                    Id = room.Id,
                    Title = room.Title,
                    Capacity = room.Capacity,
                    Facilities = FacilityList
                };
                roomVMList.Add(roomVM);
            }
            ViewBag.Rooms = roomVMList;

            var booking = await _bookingService.GetByIdAsync(id);

                var bookingVM = new BookingViewModel
                {
                    id = booking.Id,
                    title = booking.EventTitle,
                    resourceId = booking.RoomId,
                    start = booking.EventStart,
                    end = booking.EventEnd
                };

            return View(bookingVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateByAdmin(int id, BookingViewModel bookingVM)
        {
            var message = "Success";
            try
            {
                var booking = new Booking
                {
                    Id = bookingVM.id,
                    UserId = "bookingVM.UserId",
                    RoomId = bookingVM.resourceId,
                    EventTitle = bookingVM.title,
                    EventStart = bookingVM.start,
                    EventEnd = bookingVM.end,
                    ModifiedAt = DateTime.Now,
                    IsApproved = true
                };
                Log.Information("Requested booking in the Admin dashboard is Updated!");
                _logger.LogInformation("Requested booking in the Admin dashboard is Updated!");

                await _bookingService.EditAsync(booking);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                Log.Error("Requested booking in the Admin dashboard is not Updated!");
                _logger.LogError("Requested booking in the Admin dashboard is not Updated!");
            }
            ViewBag.Message = message;

            return RedirectToAction("IsApproved");
        }
    }
}
