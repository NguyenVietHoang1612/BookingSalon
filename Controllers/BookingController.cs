using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel.BookingSalonViewModel;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookingSalon.Controllers
{
    [Authorize(Roles = "Customer")]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<Users> _userManager;
        public BookingController(IBookingService bookingService, UserManager<Users> userManager)
        {
            _bookingService = bookingService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var branches = await _bookingService.GetAllBranchesAsync();
            var services = await _bookingService.GetAllServicesAsync();
            var fixTimeSlots = await _bookingService.GetAllFixedTimeSlotsAsync();
            var typeOfService = await _bookingService.GetAllTypeOfServiceAsync();
            var stylists = await _bookingService.GetAllStylistAsync();

            var viewModel = new BookingViewModel
            {
                Branches = branches,
                Services = services,
                TimeSlots = fixTimeSlots,
                TypeOfServices = typeOfService,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(BookingViewModel bookingVM)
        {
            if (!ModelState.IsValid)
            {
                // QUAN TRỌNG: Phải nạp lại dữ liệu để các SelectList ở View không bị NULL
                bookingVM.Branches = await _bookingService.GetAllBranchesAsync();
                bookingVM.Services = await _bookingService.GetAllServicesAsync();
                bookingVM.TimeSlots = await _bookingService.GetAllFixedTimeSlotsAsync();
                bookingVM.TypeOfServices = await _bookingService.GetAllTypeOfServiceAsync();

                // Có thể gộp lỗi để hiển thị nếu cần
                return View(bookingVM);
            }

            var result = await _bookingService.CreateAsync(bookingVM);

            if (!result.Succeeded)
            {
                // Nạp lại dữ liệu tương tự ở đây nếu Create thất bại
                bookingVM.Branches = await _bookingService.GetAllBranchesAsync();
                bookingVM.Services = await _bookingService.GetAllServicesAsync();
                // ... nạp lại các list khác ...

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(bookingVM);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSlots(string stylistId, string dateStr)
        {
            var result = await _bookingService.GetSlots(stylistId, dateStr);
            if (result.Succeeded)
            {
                DateOnly selectedDate = DateOnly.Parse(dateStr);
                var now = DateTime.Now;
                var isToday = selectedDate == DateOnly.FromDateTime(now);
                var currentTime = now.TimeOfDay;

                var newSlotWork = result.Data.Select(sw => new {
                    slotId = sw.Slot_Id,
                    time = sw.TimeSlot.TimeLabel.ToString(@"hh\:mm"),

                    isAvailable = sw.Is_Available && (!isToday || sw.TimeSlot.TimeLabel > currentTime)
                }).ToList();

                return Json(newSlotWork);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View();
            }
        }

        public async Task<IActionResult> GetStylists(int branchId)
        {

            var allStylists = await _bookingService.GetAllStylistAsync();


            var result = allStylists
                .Where(s => s.Branch_Id == branchId && s.Stylist.Status == true)
                .Select(s => new
                {
                    id = s.StylistId,
                    name = s.Stylist.FullName,
                    img = s.Stylist.Avatar_Name
                }).ToList();

            return Json(result); 
        }
    }
}
