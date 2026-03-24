using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Areas.Admin.Models.ViewModel.Filter;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel.BookingSalonViewModel;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using System.Security.Claims;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin ,Reception ,Stylist")]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IBranchService _branchService;
        private readonly IUsersService _userService;
        private readonly IServicesSalonService _salonService;
        private readonly IStaffProfileService _staffProfileService;
        private readonly IFixedTimeSlotService _fixedTimeSlotService;
        private readonly ITypeOfServiceService _typeOfServiceService;
        private readonly ICustomerRankService _customerRankService;
        private readonly IVnPayService _vnPayService;
        private readonly IPaymentService _paymentService;


        public BookingController(IBookingService bookingService, IBranchService branchService, IUsersService userService,
            IServicesSalonService salonService, IStaffProfileService staffProfileService, IFixedTimeSlotService fixedTimeSlotService,
            ICustomerRankService customerRankService, ITypeOfServiceService typeOfServiceService, IVnPayService vnPayService, IPaymentService paymentService)
        {
            _bookingService = bookingService;
            _branchService = branchService;
            _userService = userService;
            _salonService = salonService;
            _staffProfileService = staffProfileService;
            _fixedTimeSlotService = fixedTimeSlotService;
            _customerRankService = customerRankService;
            _typeOfServiceService = typeOfServiceService;
            _vnPayService = vnPayService;
            _paymentService = paymentService;
        }
        [Authorize(Roles = "Reception")]
        public async Task<IActionResult> ReceptionIndex(int? pageNumber, string term, BookingStatus? status = null)
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var receptionprofile = await _staffProfileService.GetByIdAsync(userLogin);
            var receptionData = receptionprofile.Data;
            int pageSize = 10;
            var pagedData = await _bookingService.GetPagedListBranchAsync(receptionData.Branch_Id, pageNumber ?? 1, pageSize, term, status);

            var branches = await _branchService.GetAllAsync();

            var vm = new BookingListFilterViewModel
            {
                Bookings = pagedData,
                SearchTerm = term,
                Status = status,
                Branches = branches.Data
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Stylist")]
        public async Task<IActionResult> StylistIndex(int? pageNumber, string term, BookingStatus? status = null)
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int pageSize = 10;
            var pagedData = await _bookingService.GetPagedListStaffIdAsync(userLogin, pageNumber ?? 1, pageSize, term, status);

            var branches = await _branchService.GetAllAsync();

            var vm = new BookingListFilterViewModel
            {
                Bookings = pagedData,
                SearchTerm = term,
                Status = status,
                Branches = branches.Data
            };

            return View(vm);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int? branchId, int? pageNumber,  string term, BookingStatus? status = null)
        {
            int pageSize = 10;
            var pagedData = await _bookingService.GetPagedListAsync(pageNumber ?? 1, pageSize, term, status, branchId);

            var branches = await _branchService.GetAllAsync();

            var vm = new BookingListFilterViewModel
            {
                Bookings = pagedData,
                SearchTerm = term,
                Status = status,
                BranchId = branchId,
                Branches = branches.Data
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Admin ,Reception")]
        public async Task<IActionResult> Create()
        {
            var bookingVM = new BookingViewModel
            {
                NewBooking = new BookingModel { Booking_Date = DateTime.Now }
            };

            await PopulatedBookingLists(bookingVM);

            ViewBag.CustomerList = await _userService.GetAllCustomerAsync();

            return View(bookingVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Reception")]
        public async Task<IActionResult> Create(BookingViewModel bookingVM)
        {
            if (string.IsNullOrEmpty(bookingVM.NewBooking.Customer_Id))
            {
                ModelState.AddModelError("NewBooking.Customer_Id", "Vui lòng chọn khách hàng.");
            }

            if (!ModelState.IsValid)
            {
                await HandleBookingError(bookingVM, "Lỗi dữ liệu đầu vào.");
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
                    await HandleBookingError(bookingVM, "Lỗi Stylist: " + resultS.Errors.First());
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
                    await HandleBookingError(bookingVM, "Lỗi Skinner: " + resultK.Errors.First());
                    return View(bookingVM);
                }
            }

            var result = await _bookingService.BookingAsync(bookingVM);

            if (!result.Succeeded)
            {
                await HandleBookingError(bookingVM, "Lỗi khi đặt lịch: " + result.Errors.First());
                return View(bookingVM);
            }

            TempData["Success"] = "Đặt lịch thành công!";
            return User.IsInRole("Admin") ? RedirectToAction("Index") : RedirectToAction("ReceptionIndex");
        }

        private async Task HandleBookingError(BookingViewModel bookingVM, string message)
        {
            TempData["Error"] = message;
            await PopulatedBookingLists(bookingVM);
            ViewBag.CustomerList = await _userService.GetAllCustomerAsync();

            var branchId = bookingVM.NewBooking.Branch_Id;
            var allStylists = await _staffProfileService.GetAllStylistActiveAsync();
            bookingVM.Stylists = allStylists.Where(s => s.Branch_Id == branchId).ToList();

            var allSkinners = await _staffProfileService.GetAllSkinnerActiveAsync();
            bookingVM.Skinners = allSkinners.Where(s => s.Branch_Id == branchId).ToList();
        }

        private async Task PopulatedBookingLists(BookingViewModel vm)
        {
            vm.Branches = await _branchService.GetAllBranchActiveAsync() ?? new List<BranchModel>();
            vm.Services = await _salonService.GetAllServiceActiveAsync() ?? new List<ServiceModel>();
            vm.TimeSlots = await _fixedTimeSlotService.GetAllListAsync() ?? new List<FixedTimeSlotModel>();
            vm.TypeOfServices = await _typeOfServiceService.GetAllTypeServiceAsync() ?? new List<TypeOfServiceModel>();
            vm.Stylists = await _staffProfileService.GetAllStylistActiveAsync() ?? new List<StaffProfileModel>();
            vm.Skinners = await _staffProfileService.GetAllSkinnerActiveAsync() ?? new List<StaffProfileModel>();
            vm.Customers = await _userService.GetAllCustomerAsync() ?? new List<UsersModel>();
        }

        [HttpGet]
        [Authorize(Roles = "Admin ,Reception")]
        public async Task<IActionResult> GetStaffStylist(int branchId)
        {
            var allStaffStylist = await _staffProfileService.GetAllStylistActiveAsync();
            var result = allStaffStylist
                .Where(s => s.Branch_Id == branchId)
                .Select(s => new
                {
                    id = s.StaffId,
                    name = s.Staff.FullName,
                    img = s.Staff.Avatar_Name
                }).ToList();
            return Json(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin ,Reception")]
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

        [HttpGet]
        [Authorize(Roles = "Admin ,Reception")]
        public async Task<IActionResult> GetCustomerRank(string customerId)
        {
            var rank = await _customerRankService.GetByCustomerIdAsync(customerId);
            if (rank == null) return Json(new { succeeded = false });

            return Json(new { succeeded = true, data = rank });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Reception")]
        public async Task<IActionResult> Update(int id, byte status)
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userLogin == null)
            {
                return Unauthorized();
            }

            if (!Enum.IsDefined(typeof(BookingStatus), status))
            {
                return BadRequest("Trạng thái không hợp lệ.");
            }

            var result = await _bookingService.UpdateStatusAsync(id, status, userLogin);

            if (!result.Succeeded)
            {
                return Json(new { succeeded = false, errors = result.Errors });
            }

            return Json(new { succeeded = true, data = result.Data });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateImage(int id, List<IFormFile> imageFiles)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _bookingService.UpdateImageAsync(id, imageFiles, userId);

            if (result.Succeeded)
                TempData["Success"] = "Đã hoàn thành dịch vụ và lưu ảnh thành công!";
            else
                TempData["Error"] = result.Errors;

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            else if (User.IsInRole("Reception"))
            {
                return RedirectToAction("ReceptionIndex");
            }
            else
            {
                return RedirectToAction("StylistIndex");
            } 
        }

        [HttpGet]
        public async Task<IActionResult> BookingDetails(int id)
        {
            var result = await _bookingService.GetAllBookingDetailsAsync(id);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return NotFound();
            }
            return View(result.Data);
        }

        [HttpGet]
        [Authorize(Roles = "Admin ,Reception")]
        public async Task<IActionResult> GetAvailableSlots(string stylistId, string skinnerId, string date, int duration)
        {
            if (string.IsNullOrEmpty(stylistId) || string.IsNullOrEmpty(skinnerId) || string.IsNullOrEmpty(date))
            {
                return BadRequest("Vui lòng chọn đầy đủ Stylist, Skinner và Ngày.");
            }

            if (duration <= 0)
            {
                return BadRequest("Thời lượng dịch vụ không hợp lệ.");
            }

            if (!DateOnly.TryParse(date, out var dateOnly))
            {
                return BadRequest("Định dạng ngày không hợp lệ (YYYY-MM-DD).");
            }

            var result = await _bookingService.GetSlots(stylistId, skinnerId, dateOnly, duration);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var data = result.Data.Select(x => new
            {
                slotId = x.SlotId,
                timeRange = x.TimeRange.ToString(),
                isAvailable = x.IsAvailable
            });

            return Json(data);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Reception")]
        public async Task<IActionResult> GetSlotsAvailableForBranch(int branchId, string? stylistId, string? skinnerId, string date, int duration)
        {

            if (branchId <= 0 || string.IsNullOrEmpty(date) || duration <= 0)
                return Json(new List<object>());

            if (!DateOnly.TryParse(date, out var bookingDate))
            {
                return BadRequest("Định dạng ngày không hợp lệ. Mong muốn: yyyy-MM-dd");
            }

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

        [Authorize(Roles = "Admin ,Reception")]
        public async Task<IActionResult> ExportInvoicePdf(int id)
        {
            var result = await _bookingService.GetAllBookingDetailsAsync(id);
            if (!result.Succeeded || result.Data == null) return NotFound();

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var document = new InvoiceDocument(result.Data);

            byte[] pdfBytes = document.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"HoaDon_{id}.pdf");
        }

        [HttpGet]
        [Authorize(Roles = "Admin ,Reception")]
        public async Task<IActionResult> PaymentDetails(int id)
        {
            var result = await _bookingService.GetAllBookingDetailsAsync(id);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return NotFound();
            }
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            string bookingIdStr = Request.Query["vnp_TxnRef"];
            string userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(bookingIdStr, out int bookingId) && response.VnPayResponseCode == "00")
            {
                await _bookingService.UpdateStatusAsync(bookingId, 4, userLogin);

                var payment = new PaymentModel
                {
                    PaymentId = Guid.NewGuid(),
                    BookingId = bookingId,
                    Amount = decimal.Parse(Request.Query["vnp_Amount"]) / 100,
                    PaymentMethod = "VNPay",
                    OrderInfo = "VNPay Transaction: " + Request.Query["vnp_TransactionNo"],
                    PaymentDate = DateTime.Now,
                    ProcessedBy = userLogin

                };
                await _paymentService.CreatePaymentAsync(payment);

                TempData["Success"] = "Thanh toán qua VNPay thành công!";
                return RedirectToAction("BookingDetails", new { id = bookingId });
            }

            TempData["Error"] = "Thanh toán không thành công.";
            return RedirectToAction("Index");
        }


    }
}
