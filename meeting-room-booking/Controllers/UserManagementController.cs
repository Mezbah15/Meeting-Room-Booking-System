using CsvHelper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using meeting_room_booking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;

namespace meeting_room_booking.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _dbContext;

        public UserManagementController(UserManager<ApplicationUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUserAsync([FromForm] UserViewModel model)
        {
            string message = "";
            bool isSuccess = true;
            try
            {
                if (model.CSVFile != null && model.CSVFile.Length > 0)
                {
                    var reader = new StreamReader(model.CSVFile.OpenReadStream());
                    var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    var records = csv.GetRecords<CSVParserViewModel>();
                    foreach (var record in records)
                    {
                        var user = new ApplicationUser()
                        {
                            Name = record.Name,
                            UserName = record.Pin,
                            Pin = record.Pin,
                            Email = record.Email,
                            PhoneNumber = record.PhoneNumber,
                            Department = record.Department
                        };

                        var result = await _userManager.CreateAsync(user, record.Password);

                        if (result.Succeeded == true)
                        {
                            await _userManager.AddToRoleAsync(user, "User");

                            isSuccess = true;
                            message = "User Registration Complete!";
                            Log.Information("Admin created a new User");
                        }
                        else
                        {
                            isSuccess = false;
                            message = $"User {user.Pin} already exists!";
                            Log.Error($"User {user.Pin} already exists!");
                        }
                    }
                }
                else if (ModelState.IsValid == true)
                {
                    var user = new ApplicationUser()
                    {
                        Name = model.Name,
                        UserName = model.Pin,
                        Pin = model.Pin,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Department = model.Department
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded == true)
                    {
                        await _userManager.AddToRoleAsync(user, "User");

                        isSuccess = true;
                        message = "User Registration Complete!";
                        Log.Information("Admin created a new User");
                    }
                    else
                    {
                        isSuccess = false;
                        message = $"User {user.Pin} already exists!";
                        Log.Error($"User {user.Pin} already exists!");
                    }
                }
            }
            catch (Exception)
            {
                isSuccess = false;
                message = "Something went worng";
            }

            return Ok(new { isSuccess = isSuccess, message = message});  
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            List<ApplicationUser>? users = await _dbContext.Users.ToListAsync();

            return View(users);
        }
    }
}
