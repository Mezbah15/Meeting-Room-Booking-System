using DataAccessLayer.Entities;
using meeting_room_booking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using Serilog;
using ServiceLayer.IServices;
using System.Drawing.Imaging;

namespace meeting_room_booking.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IFacilityService _facilityService;

        private IWebHostEnvironment _env;

        public RoomController(IRoomService roomService, IFacilityService facilityService, IWebHostEnvironment env)
        {
            _roomService = roomService;
            _facilityService = facilityService;
            _env = env;
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            Log.Information("Room Index get started!");

            var roomsWithFacilities = await _roomService.GetAllAsync();
            var roomVMList = new List<RoomViewModel>();

            foreach (var rooms in roomsWithFacilities)
            {
                var facilityList = new List<string>();

                foreach (var facility in rooms.Facilities)
                {
                    facilityList.Add(facility.Name);
                }

                var imageList = rooms.RoomImages;

                var roomVM = new RoomViewModel
                {
                    Id = rooms.Id,
                    Title = rooms.Title,
                    Capacity = rooms.Capacity,
                    RoomColor = rooms.RoomColor,
                    Description = rooms.Description,
                    Facilities = facilityList,
                    ImagePaths = imageList
                };

                roomVMList.Add(roomVM);
            }

            return View(roomVMList);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Details(int Id)
        {
            try
            {
                var room = await _roomService.GetByIdAsync(Id);
                var facilityList = new List<string>();

                foreach (var facility in room.Facilities)
                {
                    facilityList.Add(facility.Name);
                }

                var imageList = room.RoomImages;

                var roomVM = new RoomViewModel
                {
                    Id = room.Id,
                    Title = room.Title,
                    Capacity = room.Capacity,
                    RoomColor = room.RoomColor,
                    Description = room.Description,
                    Facilities = facilityList,
                    ImagePaths = imageList
                };
                Log.Information($"Room Details for Room Id: {room.Id} Opened!");

                return View(roomVM);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Log.Error("Room Not Found, Invalid Room Id!");

                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create()
        {
            var facilities = await _facilityService.GetAllAsync();
            var FacilityList = new List<RoomFacilitiesVM>();

            foreach (var facility in facilities)
            {
                FacilityList.Add(new RoomFacilitiesVM
                {
                    Id = facility.Id,
                    Name = facility.Name,
                    IsSelected = false
                });
            }

            var roomVM = new RoomViewModel
            {
                FacilitiesForRoom = FacilityList
            };

            return View(roomVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(RoomViewModel roomVM)
        {
            var facilities = await _facilityService.GetAllAsync();
            var FacilityList = new List<RoomFacilitiesVM>();

            foreach (var facility in facilities)
            {
                FacilityList.Add(new RoomFacilitiesVM
                {
                    Id = facility.Id,
                    Name = facility.Name,
                    IsSelected = false
                });
            }

            var roomVMs = new RoomViewModel
            {
                FacilitiesForRoom = FacilityList
            };

            try
            {
                if (ModelState.IsValid)
                {
                    List<Facility> facilityList = await _facilityService.GetByIdAsync(roomVM.SelectedFacilities);
                    List<string> ImagePathList = new List<string>();

                    foreach (var image in roomVM.RoomImages)
                    {
                        var extension = Path.GetExtension(image.FileName);
                        var size = image.Length;
                        if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                        {
                            if (size < 100000)
                            {
                                string folder = Path.Combine(_env.WebRootPath, "Images");
                                string fileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                                string filepath = Path.Combine(folder, fileName);

                                await image.CopyToAsync(new FileStream(filepath, FileMode.Create));

                                ImagePathList.Add(fileName);
                            }
                            else
                            {
                                TempData["SizeError"] = "Size must be less than 100kb";

                                return View(roomVMs);
                            }
                        }
                        else
                        {
                            TempData["ExtensionError"] = "Must be a png, jpg or jpeg";

                            return View(roomVMs);
                        }
                    }
                    var room = new Room
                    {
                        Title = roomVM.Title,
                        Capacity = roomVM.Capacity,
                        Description = roomVM.Description,
                        RoomColor = roomVM.RoomColor,
                        Facilities = facilityList,
                        RoomImages = ImagePathList
                    };
                    using (MemoryStream ms = new MemoryStream())
                    {
                        var qrCodeGenerator = new QRCodeGenerator();
                        var qrCodeData = qrCodeGenerator.CreateQrCode($"https://localhost:7167/Room/Details/{0}", QRCodeGenerator.ECCLevel.Q);
                        var qrCode = new QRCode(qrCodeData);

                        using (var oBitmap = qrCode.GetGraphic(20))
                        {
                            oBitmap.Save(ms, ImageFormat.Png);
                            ViewBag.QRCode = "data: image/png;base64," + Convert.ToBase64String(ms.ToArray());
                        }
                    }

                    await _roomService.CreateAsync(room);
                    Log.Information("Room Created!");

                    return View(roomVMs);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Room not created : {ex.Message}!");
                ViewBag.ErrorMessages = ex.Message;
            }

            return View(roomVMs);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int Id)
        {
            try
            {
                var room = await _roomService.GetByIdAsync(Id);

                List<RoomFacilitiesVM>? roomFacilityList = new List<RoomFacilitiesVM>();

                foreach (var facility in room.Facilities)
                {
                    var roomFacilities = new RoomFacilitiesVM
                    {
                        Id = facility.Id,
                        Name = facility.Name,
                        IsSelected = true
                    };
                    roomFacilityList.Add(roomFacilities);
                }

                var allFacility = await _facilityService.GetAllAsync();

                foreach (var facility in allFacility)
                {
                    if (roomFacilityList.Any(x => x.Id == facility.Id))
                    {
                        continue;
                    }

                    var roomWithFacility = new RoomFacilitiesVM
                    {
                        Id = facility.Id,
                        Name = facility.Name,
                        IsSelected = false
                    };
                    roomFacilityList.Add(roomWithFacility);
                }

                var roomVM = new RoomViewModel
                {
                    Id = room.Id,
                    Title = room.Title,
                    Capacity = room.Capacity,
                    RoomColor = room.RoomColor,
                    Description = room.Description,
                    FacilitiesForRoom = roomFacilityList,
                    ImagePaths = room.RoomImages
                };
                Log.Information("Room Update get started!");

                return View(roomVM);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                Log.Error($"{e.Message}");

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(RoomViewModel roomVM, int id)
        {
            var allfacilities = await _facilityService.GetAllAsync();

            var FacilityList = new List<RoomFacilitiesVM>();

            foreach (var facility in allfacilities)
            {
                FacilityList.Add(new RoomFacilitiesVM
                {
                    Id = facility.Id,
                    Name = facility.Name,
                    IsSelected = false
                });
            }
            var roomVMs = new RoomViewModel
            {
                FacilitiesForRoom = FacilityList
            };

            try
            {
                if (ModelState.IsValid)
                {
                    var facilities = await _facilityService.GetByIdAsync(roomVM.SelectedFacilities);
                    var existingRoom = await _roomService.GetByIdAsync(id);

                    List<string> ImagePathList = new List<string>();

                    if (roomVM.RoomImages != null)
                    {
                        foreach (var image in roomVM.RoomImages)
                        {
                            var extension = Path.GetExtension(image.FileName);
                            var size = image.Length;
                            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                            {
                                if (size < 100000)
                                {
                                    string folder = Path.Combine(_env.WebRootPath, "Images");
                                    string fileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                                    string filepath = Path.Combine(folder, fileName);

                                    using (var stream = new FileStream(filepath, FileMode.Create))
                                    {
                                        image.CopyTo(stream);
                                    };

                                    ImagePathList.Add(fileName);

                                    foreach (var roomImage in existingRoom.RoomImages)
                                    {
                                        var file = Path.Combine(folder, roomImage);
                                        System.IO.File.Delete(file);
                                    }
                                }
                                else
                                {
                                    TempData["SizeError"] = "Size must be less than 100kb";

                                    return View(roomVMs);
                                }
                            }
                            else
                            {
                                TempData["ExtensionError"] = "Must be a png, jpg or jpeg";

                                return View(roomVMs);
                            }
                        }
                    }
                    else
                    {
                        ImagePathList = existingRoom.RoomImages;
                    }

                    existingRoom.Title = roomVM.Title;
                    existingRoom.Capacity = roomVM.Capacity;
                    existingRoom.RoomColor = roomVM.RoomColor;
                    existingRoom.Description = roomVM.Description;
                    existingRoom.Facilities = facilities;
                    existingRoom.RoomImages = ImagePathList;

                    await _roomService.UpdateAsync(existingRoom);

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {

                ViewBag.error = ex.Message;
            }

            return View(roomVMs);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int Id)
        {
            try
            {
                var room = await _roomService.GetByIdAsync(Id);

                var facilityList = new List<RoomFacilitiesVM>();

                foreach (var facility in room.Facilities)
                {
                    var roomWithFacility = new RoomFacilitiesVM
                    {
                        Name = facility.Name,
                    };
                    facilityList.Add(roomWithFacility);
                }

                var roomVM = new RoomViewModel
                {
                    Title = room.Title,
                    Capacity = room.Capacity,
                    RoomColor = room.RoomColor,
                    Description = room.Description,
                    FacilitiesForRoom = facilityList
                };
                Log.Information("Attempt to delete a room");

                return View(roomVM);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                Log.Error($"Tried to Delete a Room : {e.Message}");

                return View();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(int Id)
        {
            var existingRoom = await _roomService.GetByIdAsync(Id);
            string folder = Path.Combine(_env.WebRootPath, "Images");

            foreach (var roomImage in existingRoom.RoomImages)
            {
                var file = Path.Combine(folder, roomImage);
                System.IO.File.Delete(file);
            }

            await _roomService.DeleteAsync(Id);
            Log.Information($"Delete Confirmed Room Id: {Id}");

            return RedirectToAction(nameof(Index));
        }
    }
}
