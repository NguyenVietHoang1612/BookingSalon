using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel.BookingSalonViewModel;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSalon.Controllers
{
    [Authorize(Roles = "Customer")]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IStaffProfileService _staffProfileService;
        private readonly IFixedTimeSlotService _fixedTimeSlotService;
        private readonly IServicesSalonService _servicesSalonService;
        private readonly IBranchService _branchService;
        private readonly ITypeOfServiceService _typeOfServiceService;
        private readonly ICustomerRankService _customerRankService;
        private readonly UserManager<UsersModel> _userManager;
        private readonly IReviewService _reviewService;
        private readonly IComboService _comboService;

        public BookingController(IBookingService bookingService, IStaffProfileService staffProfileService, IFixedTimeSlotService fixedTimeSlotService,
            IServicesSalonService servicesSalonService, IBranchService branchService, ITypeOfServiceService typeOfServiceService, 
            UserManager<UsersModel> userManager, ICustomerRankService customerRankService, IReviewService reviewService, IComboService comboService)
        {
            _bookingService = bookingService;
            _staffProfileService = staffProfileService;
            _fixedTimeSlotService = fixedTimeSlotService;
            _servicesSalonService = servicesSalonService;
            _branchService = branchService;
            _typeOfServiceService = typeOfServiceService;
            _userManager = userManager;
            _customerRankService = customerRankService;
            _reviewService = reviewService;
            _comboService = comboService;
        }

        public async Task<IActionResult> Booking()
        {
            var branches = await _branchService.GetAllBranchActiveAsync();
            var services = await _servicesSalonService.GetAllServiceActiveAsync();
            var fixTimeSlots = await _fixedTimeSlotService.GetAllListAsync();
            var typeOfService = await _typeOfServiceService.GetAllTypeServiceAsync();
            var stylists = await _staffProfileService.GetAllStylistActiveAsync();
            var skinners = await _staffProfileService.GetAllSkinnerActiveAsync();
            var combos = await _comboService.GetAllComboAsync();

            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customerRank = await _customerRankService.GetByCustomerIdAsync(user);

            var viewModel = new BookingViewModel
            {
                Branches = branches,
                Services = services,
                TimeSlots = fixTimeSlots,
                TypeOfServices = typeOfService,
                RankPercent = customerRank?.Rank.DiscountPercent ?? 0,
                Stylists = stylists,
                Skinners = skinners,
                Combos = combos,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Booking(BookingViewModel bookingVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            bookingVM.NewBooking.Customer_Id = userId;

            if (!ModelState.IsValid)
            {
                await PopulatedBookingLists(bookingVM);
                return View(bookingVM);
            }

            DateTime bookingDate = bookingVM.NewBooking.Booking_Date;

            if (bookingVM.NewBooking.Stylist_Id == "RANDOM")
            {
                var resultS = await _bookingService.GetAvailableStylistIdAsync(
                    bookingVM.NewBooking.Branch_Id,
                    bookingDate,
                    bookingVM.NewBooking.Start_Slot_Id,
                    bookingVM.NewBooking.TotalDuration);

                if (resultS.Succeeded)
                    bookingVM.NewBooking.Stylist_Id = resultS.Data;
                else
                {
                    TempData["Error"] = "Không tìm thấy Stylist trống: " + resultS.Errors.First();
                    await PopulatedBookingLists(bookingVM);
                    return View(bookingVM);
                }
            }

            if (bookingVM.NewBooking.Skinner_Id == "RANDOM")
            {
                var resultK = await _bookingService.GetAvailableSkinnerIdAsync(
                    bookingVM.NewBooking.Branch_Id,
                    bookingDate,
                    bookingVM.NewBooking.Start_Slot_Id,
                    bookingVM.NewBooking.TotalDuration,
                    excludeStaffId: bookingVM.NewBooking.Stylist_Id);

                if (resultK.Succeeded)
                    bookingVM.NewBooking.Skinner_Id = resultK.Data;
                else
                {
                    TempData["Error"] = "Không tìm thấy Skinner trống: " + resultK.Errors.First();
                    await PopulatedBookingLists(bookingVM);
                    return View(bookingVM);
                }
            }

            var result = await _bookingService.BookingAsync(bookingVM);

            if (!result.Succeeded)
            {
                await PopulatedBookingLists(bookingVM);
                TempData["Error"] = "Lỗi khi đặt lịch: " + string.Join(", ", result.Errors);
                return View(bookingVM);
            }

            TempData["Success"] = "Đặt lịch thành công!";
            return RedirectToAction("Details", new { id = bookingVM.NewBooking.Booking_Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(string stylistId,  string skinnerId, string date, int durationMinus)
        {
            if (string.IsNullOrEmpty(stylistId) || string.IsNullOrEmpty(date))
            {
                return BadRequest("Thiếu thông tin thợ hoặc ngày.");
            }

            if (!DateOnly.TryParse(date, out var dateOnly))
            {
                return BadRequest("Định dạng ngày không hợp lệ (YYYY-MM-DD).");
            }

            var result = await _bookingService.GetSlots(stylistId, skinnerId, dateOnly, durationMinus);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Json(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetSlotsAvailableForBranch(int branchId, string? stylistId, string? skinnerId, string date, int duration)
        {
            if (branchId <= 0 || string.IsNullOrEmpty(date) || duration <= 0)
                return Json(new List<object>());

            if (!DateOnly.TryParse(date, out var bookingDate))
                return BadRequest("Ngày không hợp lệ.");

            var result = await _bookingService.GetSlotsAvailableForBranchRandom(branchId, stylistId, skinnerId, bookingDate, duration);

            if (result.Succeeded)
            {
                var data = result.Data.Select(x => new
                {
                    slotId = x.SlotId,
                    timeRange = x.TimeRange.ToString().Substring(0, 5),
                    isAvailable = x.IsAvailable
                }).ToList();

                return Json(data);
            }

            return Json(new { succeeded = false, message = result.Errors });
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerRank(string customerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _customerRankService.GetByCustomerIdAsync(userId);

            if (result != null)
            {
                return Json(new
                {
                    succeeded = true,
                    data = new
                    {
                        rank = new
                        {
                            rankName = result.Rank.RankName,
                            discountPercent = result.Rank.DiscountPercent
                        }
                    }
                });
            }
            return Json(new { succeeded = false });
        }
        private async Task PopulatedBookingLists(BookingViewModel vm)
        {
            vm.Branches = await _branchService.GetAllBranchActiveAsync();
            vm.Services = await _servicesSalonService.GetAllServiceActiveAsync();
            vm.TimeSlots = await _fixedTimeSlotService.GetAllListAsync();
            vm.TypeOfServices = await _typeOfServiceService.GetAllTypeServiceAsync();
            vm.Stylists = await _staffProfileService.GetAllStylistActiveAsync();
            vm.Skinners = await _staffProfileService.GetAllSkinnerActiveAsync();
        }

        public async Task<IActionResult> GetStaffStylist(int branchId)
        {

            var allStaffStylist = await _staffProfileService.GetAllStylistActiveAsync();
            var allReviews = await _reviewService.GetAllAsync();

            var result = allStaffStylist
                .Where(s => s.Branch_Id == branchId)
                .Select(s => new
                {
                    id = s.StaffId,
                    name = s.Staff.FullName,
                    img = s.Staff.Avatar_Name,
                    rating = allReviews.Where(r => r.Staff_Id == s.StaffId).Any()
                             ? Math.Round(allReviews.Where(r => r.Staff_Id == s.StaffId).Average(r => r.Rating), 1)
                             : 5.0 
                }).ToList();

            return Json(result);
        }

        public async Task<IActionResult> GetStaffSkinner(int branchId)
        {

            var allStaffSkinner = await _staffProfileService.GetAllSkinnerActiveAsync();

            var result = allStaffSkinner
                .Where(s => s.Branch_Id == branchId)
                .Select(s => new
                {
                    id = s.StaffId,
                    name = s.Staff.FullName,
                    img = s.Staff.Avatar_Name
                }).ToList();

            return Json(result);
        }

        public async Task<IActionResult> History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Challenge();

            var allBookings = await _bookingService.GetAllAsync();

    
            var myBookings = allBookings
                .Where(mb=>mb.Customer_Id == userId)
                .OrderByDescending(b => b.CreateAt)
                .ToList();

            return View(myBookings);
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _bookingService.GetAllBookingDetailsAsync(id);
            if (!result.Succeeded) return NotFound();

            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var bookingResponse = await _bookingService.GetByIdAsync(id);
            if (bookingResponse.Data == null) return NotFound();
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userLogin == null)
            {
                return Unauthorized();
            }

            if (bookingResponse.Data.Status == BookingStatus.PendingConfirmation)
            {
                var result = await _bookingService.UpdateStatusAsync(id, (int)BookingStatus.Canceled, userLogin);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Đã hủy lịch đặt thành công.";
                }
                else
                {
                    TempData["Error"] = "Không thể hủy lịch vào lúc này.";
                }
            }
            else
            {
                TempData["Error"] = "Lịch đã được xác nhận hoặc đang thực hiện, không thể tự hủy.";
            }

            return RedirectToAction(nameof(History));
        }
    }
}
