using DataAccessLayer.Entities;
using meeting_room_booking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ServiceLayer.IServices;

namespace meeting_room_booking.Controllers
{
    public class FacilityController : Controller
    {
        private readonly IFacilityService _facilityService;
        public FacilityController(IFacilityService facilityService)
        {
            _facilityService = facilityService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            var facilities = await _facilityService.GetAllAsync();
            var facilityVMList = new List<FacilityViewModel>();

            foreach (var facility in facilities)
            {
                var facilityVM = new FacilityViewModel
                {
                    Id = facility.Id,
                    Name = facility.Name
                };
                facilityVMList.Add(facilityVM);
            }
            Log.Information("Facility Index get started!");

            return View(facilityVMList);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Details(int Id)
        {
            try
            {
                var facility = await _facilityService.GetByIdAsync(Id);

                var model = new FacilityViewModel
                {
                    Id = facility.Id,
                    Name = facility.Name
                };
                Log.Information("Facility Index get started!");

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Log.Error("Facility Not Found, Invalid Fcility Id!");

                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(FacilityViewModel facilityVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var facility = new Facility
                    {
                        Name = facilityVM.Name
                    };

                    await _facilityService.CreateAsync(facility);
                    Log.Information("Facility Created Successfully!");

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {

                ViewBag.ErrorMessage = ex.Message;
                Log.Error($"Facility Not Created: {ex.Message}!");
            }

            return View(facilityVM);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int Id)
        {
            try
            {
                var facility = await _facilityService.GetByIdAsync(Id);

                var facilityVM = new FacilityViewModel
                {
                    Name = facility.Name
                };
                Log.Information("Edit a Facility get started!");

                return View(facilityVM);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                Log.Error("Facility does not exits!");

                return View();
            }           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int Id, FacilityViewModel facilityVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var facility = await _facilityService.GetByIdAsync(Id);

                    facility.Name = facilityVM.Name;
                    await _facilityService.UpdateAsync(facility);
                    Log.Information("Facility updated!");

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                Log.Error("Facility not updated!");
            }

            return View(facilityVM);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var facility = await _facilityService.GetByIdAsync(id);

                var facilityVM = new FacilityViewModel
                {
                    Name = facility.Name
                };

                return View(facilityVM);
            }
            catch (Exception e)
            {
                ViewBag.Error = e.Message;
                Log.Error("Facility does not exits!");

                return View();
            }            
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(int Id)
        {
            await _facilityService.DeleteAsync(Id);
            Log.Information("Facility Deleted!");

            return RedirectToAction(nameof(Index));
        }
    }
}
