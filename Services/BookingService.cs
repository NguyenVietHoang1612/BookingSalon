using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel.BookingSalonViewModel;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BookingSalon.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRankService _customerRankService;
        private readonly IFixedTimeSlotService _fixedTimeSlotService;
        private readonly ICouponService _couponService;
        private readonly IFileService _fileService;
        private readonly IReviewService _reviewService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public BookingService(IUnitOfWork unitOfWork, ICustomerRankService customerRankService, IFixedTimeSlotService fixedTimeSlotService,
            ICouponService couponService, RoleManager<IdentityRole> roleManager, IFileService fileService, IReviewService reviewService)
        {
            _unitOfWork = unitOfWork;
            _customerRankService = customerRankService;
            _fixedTimeSlotService = fixedTimeSlotService;
            _couponService = couponService;
            _roleManager = roleManager;
            _fileService = fileService;
            _reviewService = reviewService;
        }

        public async Task<IEnumerable<BookingProfileDetailsViewModel>> GetAllAsync()
        {
            var bookingData = await _unitOfWork.Repository<BookingModel>()
            .Query()
            .Include(s => s.BookingImages)
            .Select(b => new BookingProfileDetailsViewModel
            {
                BookingId = b.Booking_Id,
                Customer_Id = b.Customer_Id,
                Customer_Name = b.Customer.FullName,
                Customer_Phone = b.Customer.PhoneNumber,
                Customer_Email = b.Customer.Email,
                Customer_Address = b.Customer.Address,
                Id_Stylist = b.Stylist_Id,
                Stylist_Name = b.StylistProfile.Staff.FullName,
                Stylist_Image = b.StylistProfile.Staff.Avatar_Name,
                Id_Skinner = b.Skinner_Id,
                Skinner_Name = b.SkinnerProfile.Staff.FullName,
                Skinner_Image = b.SkinnerProfile.Staff.Avatar_Name,
                Branch_Name = b.Branch.Branch_Name,
                Date_Booking = b.Booking_Date,
                TimeSlot = b.StartSlot.TimeLabel,
                TotalDuration = b.TotalDuration,
                FinalPrice = b.TotalPrice,
                Status = b.Status,
                BookingImages = b.BookingImages,
                IsReview = _unitOfWork.Repository<ReviewModel>().Query().Any(r => r.Booking_Id == b.Booking_Id),
                CreateAt = b.Create_At
            })
            .ToListAsync();

            return bookingData;
        }

        public async Task<ServiceResult<BookingModel>> GetByIdAsync(int id)
        {
            try
            {
                var booking = await _unitOfWork.Repository<BookingModel>().GetByIdAsync(id);

                if (booking == null)
                    return ServiceResult<BookingModel>.Failed("Không tìm thấy branch");

                return ServiceResult<BookingModel>.Success(booking);
            }
            catch (Exception ex)
            {
                return ServiceResult<BookingModel>.Failed($"Lỗi: {ex.Message}");
            }
        }


        public async Task<ServiceResult<BookingViewModel>> BookingAsync(BookingViewModel bookingVM)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var booking = bookingVM.NewBooking;
                booking.Create_At = DateTime.Now;
                booking.Update_At = DateTime.Now;
                booking.Status = BookingStatus.PendingConfirmation;

                decimal subTotal = 0;
                int totalDuration = 0;
                var serviceRepo = _unitOfWork.Repository<ServiceModel>();

                if (bookingVM.BookingDetails == null || !bookingVM.BookingDetails.Any())
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ServiceResult<BookingViewModel>.Failed("Vui lòng chọn ít nhất 1 dịch vụ.");
                }

                foreach (var detail in bookingVM.BookingDetails)
                {
                    var service = await serviceRepo.GetByIdAsync(detail.Service_Id);
                    if (service != null)
                    {
                        decimal actualPrice = (service.Promotion_Price != null && service.Promotion_Price >= 0 && service.IsPromotionActive)
                                      ? service.Promotion_Price.Value
                                      : service.Base_Price;

                        detail.Price = actualPrice;
                        detail.BasePriceSnapshot = service.Base_Price;
                        detail.ServiceNameSnapshot = service.Service_Name;
                        detail.DurationSnapshot = service.DurationInMinutes;

                        subTotal += actualPrice;
                        totalDuration += service.DurationInMinutes;
                    }
                }

                if (string.IsNullOrEmpty(bookingVM.NewBooking.Stylist_Id))
                {
                    return ServiceResult<BookingViewModel>.Failed("Vui lòng chọn Stylist bắt buộc.");
                }

                var isAvailableSlot = GetListSlotIdsInRange(bookingVM.NewBooking.Start_Slot_Id, totalDuration);

                foreach (var slotId in isAvailableSlot)
                {
                    var existingSchedule = await _unitOfWork.Repository<StaffScheduleModel>().Query()
                        .FirstOrDefaultAsync(s =>
                            s.Staff_Id == booking.Stylist_Id &&
                            s.Work_Date == DateOnly.FromDateTime(booking.Booking_Date) &&
                            s.Slot_Id == slotId);

                    if (existingSchedule != null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ServiceResult<BookingViewModel>.Failed("Rất tiếc, khung giờ này trùng 1 khung thời gian với khách khác rồi. Vui lòng chọn khung giờ khác.");
                    }
                }
                var customerRank = await _customerRankService.GetByCustomerIdAsync(booking.Customer_Id);

                decimal discountRank = 0;
                decimal priceAfterRank = subTotal;

                if (customerRank != null)
                {
                    discountRank = subTotal * customerRank.Rank.DiscountPercent / 100;
                    priceAfterRank -= discountRank;
                }

                decimal discountCoupon = 0;

                if (!string.IsNullOrWhiteSpace(bookingVM.CouponCode))
                {
                    var couponResult = await _couponService
                        .ValidateForBookingAsync(
                            bookingVM.CouponCode,
                            priceAfterRank,
                            booking.Customer_Id);

                    if (!couponResult.Succeeded)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ServiceResult<BookingViewModel>
                            .Failed(couponResult.Errors.ToArray());
                    }

                    discountCoupon = couponResult.Data.Discount;

                    booking.Coupon_Id = couponResult.Data.CouponId;
                    booking.CouponCodeSnapshot = couponResult.Data.Code;
                    booking.DiscountValueSnapshot = couponResult.Data.DiscountValue;
                    booking.DiscountTypeSnapshot = couponResult.Data.DiscountType;
                }


                booking.TotalPrice = subTotal;
                booking.DiscountAmount = discountRank + discountCoupon;
                booking.FinalPrice = subTotal - booking.DiscountAmount;

                if (booking.FinalPrice < 0)
                    booking.FinalPrice = 0;

                booking.TotalDuration = totalDuration;

                // Kiểm tra thời gian làm việc
                var allSlots = await _fixedTimeSlotService.GetAllListAsync();
                var startSlot = allSlots.FirstOrDefault(s => s.SlotId == booking.Start_Slot_Id);

                if (startSlot != null)
                {
                    var startTime = startSlot.TimeLabel;
                    var endTime = startTime.Add(TimeSpan.FromMinutes(totalDuration));

                    var endSlot = allSlots
                        .Where(s => s.TimeLabel < endTime)
                        .OrderByDescending(s => s.TimeLabel)
                        .FirstOrDefault();

                    booking.End_Slot_Id = endSlot?.SlotId ?? booking.Start_Slot_Id;
                }

                await _unitOfWork.Repository<BookingModel>().AddAsync(booking);
                await _unitOfWork.SaveChangesAsync();

                foreach (var detail in bookingVM.BookingDetails)
                {
                    detail.Booking_Id = booking.Booking_Id;
                    detail.Created_At = DateTime.Now;
                    detail.Update_At = DateTime.Now;
                    await _unitOfWork.Repository<BookingDetailModel>().AddAsync(detail);
                }
                await _unitOfWork.SaveChangesAsync();

                // Thêm Staff vào schedule
                var lockStylist = await AddStylistTimeWork(booking.Booking_Id);

                if (!lockStylist.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ServiceResult<BookingViewModel>.Failed(lockStylist.Errors.ToArray());
                }

                var lockSkinner = await AddSkinnerTimeWork(booking.Booking_Id);

                if (!lockSkinner.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ServiceResult<BookingViewModel>.Failed(lockStylist.Errors.ToArray());
                }

                if (booking.Coupon_Id.HasValue)
                {
                    var couponRepo = _unitOfWork.Repository<CouponModel>();
                    var usageRepo = _unitOfWork.Repository<CouponUsage>();

                    var coupon = await couponRepo.GetByIdAsync(booking.Coupon_Id.Value);

                    if (coupon != null)
                    {
                        coupon.Used_Count += 1;
                        coupon.Updated_At = DateTime.Now;
                        couponRepo.Update(coupon);

                        await usageRepo.AddAsync(new CouponUsage
                        {
                            CouponId = coupon.Id,
                            UserId = booking.Customer_Id,
                            UsedAt = DateTime.Now
                        });
                    }
                }

                await _unitOfWork.CommitTransactionAsync();
                return ServiceResult<BookingViewModel>.Success(bookingVM);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult<BookingViewModel>.Failed($"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BookingModel>> UpdateStatusAsync(int id, byte statusValue, string updateById)
        {
            try
            {
                if (statusValue == 5)
                {
                    var cancelResult = await UpdateCancelBooking(id);
                    if (!cancelResult.Succeeded)
                    {
                        return ServiceResult<BookingModel>.Failed(cancelResult.Errors.ToArray());
                    }
                    var updatedBooking = await GetByIdAsync(id);
                    return ServiceResult<BookingModel>.Success(updatedBooking.Data);
                }

                await _unitOfWork.BeginTransactionAsync();

                var responseBooking = await GetByIdAsync(id);
                var booking = responseBooking?.Data;
                if (booking == null) return ServiceResult<BookingModel>.Failed("Không tìm thấy booking");

                if (booking.Status == BookingStatus.Canceled)
                {
                    return ServiceResult<BookingModel>.Failed("Không thể thay đổi trạng thái của lịch đã hủy.");
                }

                booking.Status = (BookingStatus)statusValue;
                booking.Update_At = DateTime.Now;
                booking.UpdatedById = updateById;
                _unitOfWork.Repository<BookingModel>().Update(booking);

                if (statusValue == 4)
                {
                    await _customerRankService.UpdatePointsAsync(booking.Customer_Id, (int)(responseBooking.Data.FinalPrice/1000));
                }

                await _unitOfWork.SaveChangesAsync();


                await _unitOfWork.CommitTransactionAsync();
                return ServiceResult<BookingModel>.Success(booking);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult<BookingModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BookingModel>> UpdateImageAsync(int id, List<IFormFile> imageFiles, string updateById)
        {
            List<string> savedFileNames = new List<string>();

            try
            {
                var responseBooking = await GetByIdAsync(id);
                var booking = responseBooking?.Data;

                if (booking == null) return ServiceResult<BookingModel>.Failed("Không tìm thấy lịch đặt.");
                if (booking.Status != BookingStatus.InProgress) return ServiceResult<BookingModel>.Failed("Chỉ có thể hoàn tất lịch đang phục vụ.");
                if (imageFiles == null || !imageFiles.Any()) return ServiceResult<BookingModel>.Failed("Vui lòng cung cấp ít nhất một ảnh.");

                await _unitOfWork.BeginTransactionAsync();

                foreach (var file in imageFiles)
                {
                    var fileName = await _fileService.UploadFileAsync(file, "bookingresult");
                    savedFileNames.Add(fileName);

                    var newImage = new BookingImageModel
                    {
                        Booking_Id = id,
                        ImageUrl = fileName,
                        Created_At = DateTime.Now
                    };
                    await _unitOfWork.Repository<BookingImageModel>().AddAsync(newImage);
                }


                booking.Status = BookingStatus.Completed;
                booking.Update_At = DateTime.Now;
                booking.UpdatedById = updateById;
                _unitOfWork.Repository<BookingModel>().Update(booking);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return ServiceResult<BookingModel>.Success(booking);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                foreach (var fileName in savedFileNames)
                {
                    await _fileService.DeleteFileAsync(fileName, "bookings");
                }

                return ServiceResult<BookingModel>.Failed($"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> AddStylistTimeWork(int bookingId)
        {
            try
            {
                var bookingResponse = await GetByIdAsync(bookingId);
                var booking = bookingResponse?.Data;

                if (booking == null)
                {
                    return ServiceResult<bool>.Failed("Không tìm thấy thông tin lịch đặt.");
                }

                var slotIdsToBook = GetListSlotIdsInRange(booking.Start_Slot_Id, booking.TotalDuration);

                DateOnly bookingDateOnly = DateOnly.FromDateTime(booking.Booking_Date);

                var isExist = await _unitOfWork.Repository<StaffScheduleModel>().ExistsAsync(x =>
                    x.Staff_Id == booking.Stylist_Id &&
                    x.Work_Date == bookingDateOnly &&
                    slotIdsToBook.Contains(x.Slot_Id) &&
                    x.Booking_Id != bookingId);

                if (isExist)
                {
                    return ServiceResult<bool>.Failed("Rất tiếc, Tổng thời gian dự kiến trùng khung giờ 1 khung thời gian với khách khác. Vui lòng load lại trang và chọn khung giờ khác.");
                }

                var currentStylistSlots = _unitOfWork.Repository<StaffScheduleModel>().Query()
                     .Where(x => x.Booking_Id == bookingId && x.Staff_Id == booking.Stylist_Id); 

                _unitOfWork.Repository<StaffScheduleModel>().DeleteRange(currentStylistSlots);
                await _unitOfWork.SaveChangesAsync();
                foreach (var sId in slotIdsToBook)
                {
                    var timeWork = new StaffScheduleModel
                    {
                        Booking_Id = bookingId,
                        Staff_Id = booking.Stylist_Id,
                        Work_Date = bookingDateOnly,
                        Slot_Id = sId,
                        Create_At = DateTime.Now,
                        Update_At = DateTime.Now

                    };
                    await _unitOfWork.Repository<StaffScheduleModel>().AddAsync(timeWork);
                }
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failed($"Hệ thống gặp lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> AddSkinnerTimeWork(int bookingId)
        {
            try
            {
                var bookingResponse = await GetByIdAsync(bookingId);
                var booking = bookingResponse?.Data;

                if (booking == null)
                {
                    return ServiceResult<bool>.Failed("Không tìm thấy thông tin lịch đặt.");
                }

                var slotIdsToBook = GetListSlotIdsInRange(booking.Start_Slot_Id, booking.TotalDuration);

                DateOnly bookingDateOnly = DateOnly.FromDateTime(booking.Booking_Date);

                var isExist = await _unitOfWork.Repository<StaffScheduleModel>().ExistsAsync(x =>
                    x.Staff_Id == booking.Skinner_Id &&
                    x.Work_Date == bookingDateOnly &&
                    slotIdsToBook.Contains(x.Slot_Id) &&
                    x.Booking_Id != bookingId);

                if (isExist)
                {
                    return ServiceResult<bool>.Failed("Rất tiếc, Tổng thời gian dự kiến trùng khung giờ 1 khung thời gian với khách khác. Vui lòng chọn khung giờ khác.");
                }

                var currentSkinnerSlots = _unitOfWork.Repository<StaffScheduleModel>().Query()
                    .Where(x => x.Booking_Id == bookingId && x.Staff_Id == booking.Skinner_Id); 

                _unitOfWork.Repository<StaffScheduleModel>().DeleteRange(currentSkinnerSlots);
                await _unitOfWork.SaveChangesAsync();
                foreach (var sId in slotIdsToBook)
                {
                    var timeWork = new StaffScheduleModel
                    {
                        Booking_Id = bookingId,
                        Staff_Id = booking.Skinner_Id,
                        Work_Date = bookingDateOnly,
                        Slot_Id = sId,
                        Create_At = DateTime.Now,
                        Update_At = DateTime.Now

                    };
                    await _unitOfWork.Repository<StaffScheduleModel>().AddAsync(timeWork);
                }
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failed($"Hệ thống gặp lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> UpdateCancelBooking(int bookingId)
        {
            try
            {
                var response = await GetByIdAsync(bookingId);
                var booking = response?.Data;
                if (booking == null) return ServiceResult<bool>.Failed("Không tìm thấy booking");

                if (booking.Status == BookingStatus.InProgress || booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Paid)
                {
                    return ServiceResult<bool>.Failed("Không thể hủy lịch hẹn đang thực hiện hoặc đã hoàn tất.");
                }

                booking.Status = BookingStatus.Canceled;
                booking.Update_At = DateTime.Now;

                var occupiedSlots = _unitOfWork.Repository<StaffScheduleModel>().Query()
                                              .Where(x => x.Booking_Id == bookingId);
                _unitOfWork.Repository<StaffScheduleModel>().DeleteRange(occupiedSlots);

                await _unitOfWork.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failed(ex.Message);
            }
        }

        private List<int> GetListSlotIdsInRange(int startSlotId, int totalDurationMinutes)
        {
            var allSlots = _unitOfWork.Repository<FixedTimeSlotModel>()
                                      .Query()
                                      .OrderBy(s => s.TimeLabel)
                                      .ToList();

            var startSlot = allSlots.FirstOrDefault(s => s.SlotId == startSlotId);
            if (startSlot == null) return new List<int>();

            var startTime = startSlot.TimeLabel;
            var endTime = startTime.Add(TimeSpan.FromMinutes(totalDurationMinutes));

            var selectedSlotIds = allSlots
                .Where(s => s.TimeLabel >= startTime && s.TimeLabel < endTime)
                .Select(s => s.SlotId)
                .ToList();

            return selectedSlotIds;
        }

        public async Task<ServiceResult<IEnumerable<SlotSelectionViewModel>>> GetSlots(string stylistId, string skinnerId, DateOnly date, int durationMinutes)
        {
            try
            {
                var stylistProfile = await _unitOfWork.Repository<StaffProfileModel>().Query()
                    .FirstOrDefaultAsync(s => s.StaffId == stylistId);

                if (stylistProfile == null)
                    return ServiceResult<IEnumerable<SlotSelectionViewModel>>.Failed("Không tìm thấy thông tin Stylist.");

                var skinnerProfile = string.IsNullOrEmpty(skinnerId) || skinnerId == "RANDOM"
                    ? null
                    : await _unitOfWork.Repository<StaffProfileModel>().Query()
                        .FirstOrDefaultAsync(s => s.StaffId == skinnerId);

                var allFixedSlots = await _unitOfWork.Repository<FixedTimeSlotModel>().Query()
                    .OrderBy(s => s.TimeLabel).ToListAsync();

                var busySchedules = await _unitOfWork.Repository<StaffScheduleModel>().Query()
                    .Where(stw => (stw.Staff_Id == stylistId || (!string.IsNullOrEmpty(skinnerId) && stw.Staff_Id == skinnerId))
                           && stw.Work_Date == date)
                    .ToListAsync();

                var viewModel = new List<SlotSelectionViewModel>();

                foreach (var slot in allFixedSlots)
                {
                    var neededSlotIds = GetListSlotIdsInRange(slot.SlotId, durationMinutes);

                    if (neededSlotIds.Count * 20 < durationMinutes)
                    {
                        viewModel.Add(new SlotSelectionViewModel { SlotId = slot.SlotId, TimeRange = slot.TimeLabel, IsAvailable = false });
                        continue;
                    }

                    bool stylistOk = slot.TimeLabel >= stylistProfile.Start_Work_Time &&
                                     slot.TimeLabel <= stylistProfile.End_Work_Time &&
                                     !busySchedules.Any(s => s.Staff_Id == stylistId && neededSlotIds.Contains(s.Slot_Id));

                    bool skinnerOk = true;
                    if (skinnerProfile != null)
                    {
                        skinnerOk = slot.TimeLabel >= skinnerProfile.Start_Work_Time &&
                                    slot.TimeLabel <= skinnerProfile.End_Work_Time &&
                                    !busySchedules.Any(s => s.Staff_Id == skinnerId && neededSlotIds.Contains(s.Slot_Id));
                    }

                    viewModel.Add(new SlotSelectionViewModel
                    {
                        SlotId = slot.SlotId,
                        TimeRange = slot.TimeLabel,
                        IsAvailable = stylistOk && skinnerOk
                    });
                }

                return ServiceResult<IEnumerable<SlotSelectionViewModel>>.Success(viewModel);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<SlotSelectionViewModel>>.Failed(ex.Message);
            }
        }

        public async Task<ServiceResult<BookingDetailViewModel>> GetAllBookingDetailsAsync(int id)
        {
            try
            {
                var paymentMethod = _unitOfWork.Repository<PaymentModel>().Query()
                    .Where(p => p.BookingId == id)
                    .Select(p => p.PaymentMethod).FirstOrDefault();         
                
                var bookingImages = await _unitOfWork.Repository<BookingImageModel>().Query()
                    .Where(bi => bi.Booking_Id == id)
                    .Select(bi => bi.ImageUrl)
                    .ToListAsync();

                var booking = await _unitOfWork.Repository<BookingModel>()
                .Query()
                .Include(bd => bd.BookingDetails)
                    .ThenInclude(d => d.Service)
                .Include(bd => bd.Branch)
                    .ThenInclude(b => b.Ward)
                        .ThenInclude(b => b.District)
                            .ThenInclude(b => b.Province)
                .Select(bd => new BookingDetailViewModel
                {
                    BookingId = bd.Booking_Id,
                    Customer_Name = bd.Customer.FullName,
                    Customer_Phone = bd.Customer.PhoneNumber,
                    Customer_Email = bd.Customer.Email,
                    Customer_Address = bd.Customer.Address,
                    Id_Stylist = bd.Stylist_Id,
                    Stylist_Name = bd.StylistProfile.Staff.FullName,
                    Id_Skinner = bd.Skinner_Id,
                    Skinner_Name = bd.SkinnerProfile.Staff.FullName,
                    Branch_Name = bd.Branch.Branch_Name,
                    Branch_Address = $"{bd.Branch.Address} {bd.Branch.Ward.Name} {bd.Branch.Ward.District.Name} {bd.Branch.Ward.District.Province.Name}",
                    Branch_Phone = bd.Branch.PhoneNumber,
                    Date_Booking = bd.Booking_Date,
                    TimeSlot = bd.StartSlot.TimeLabel,
                    TotalDuration = bd.TotalDuration,
                    DiscountAmount = bd.DiscountAmount,
                    DiscoutValueSnapshot = bd.DiscountValueSnapshot,
                    DiscountTypeSnapshot = bd.DiscountTypeSnapshot,
                    CouponCodeSnapshot = bd.CouponCodeSnapshot,
                    FinalPrice = bd.FinalPrice,
                    Status = bd.Status,
                    TotalPrice = bd.TotalPrice,
                    bookingDetails = bd.BookingDetails,
                    PaymentMethod = paymentMethod,
                    Create_At = bd.Create_At,
                    ResultImage = bookingImages
                })
                .FirstOrDefaultAsync(bd => bd.BookingId == id);



                if (booking == null)
                    return ServiceResult<BookingDetailViewModel>.Failed("Không tìm thấy booking");

                return ServiceResult<BookingDetailViewModel>.Success(booking);
            }
            catch (Exception ex)
            {
                return ServiceResult<BookingDetailViewModel>.Failed($"Lỗi: {ex.Message}");
            }


        }

        public async Task<PaginatedList<BookingProfileDetailsViewModel>> GetPagedListAsync(int pageNumber, int pageSize,
            string searchTerm, BookingStatus? status, int? branchId)
        {
            var query = _unitOfWork.Repository<BookingModel>().Query();

            query = query.Include(b => b.Customer);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(s => s.Customer.FullName.ToLower().Contains(searchTerm) || s.Customer.PhoneNumber.ToLower().Contains(searchTerm));
            }

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            if (branchId.HasValue)
            {
                query = query.Where(b => b.Branch_Id == branchId.Value);
            }


            var projectedQuery = query.Select(b => new BookingProfileDetailsViewModel
            {
                BookingId = b.Booking_Id,
                Customer_Name = b.Customer.FullName,
                Customer_Phone = b.Customer.PhoneNumber,
                Customer_Email = b.Customer.Email,
                Customer_Address = b.Customer.Address,
                Id_Stylist = b.Stylist_Id,
                Stylist_Name = b.StylistProfile.Staff.FullName,
                Stylist_Image = b.StylistProfile.Staff.Avatar_Name,
                Id_Skinner = b.Skinner_Id,
                Skinner_Name = b.SkinnerProfile.Staff.FullName,
                Skinner_Image = b.SkinnerProfile.Staff.Avatar_Name,
                Branch_Name = b.Branch.Branch_Name,
                Date_Booking = b.Booking_Date,
                TimeSlot = b.StartSlot.TimeLabel,
                Status = b.Status,
                FinalPrice = b.FinalPrice,
                TotalDuration = b.TotalDuration,
                UpdateById = b.UpdatedById,
                UpdateByName = b.UpdatedBy != null ? b.UpdatedBy.FullName : null,
                CreateAt = b.Create_At

            });

            var bookingPaginate = projectedQuery.OrderByDescending(b => b.BookingId);

            return await PaginatedList<BookingProfileDetailsViewModel>.CreateAsync(bookingPaginate, pageNumber, pageSize);
        }

        public async Task<PaginatedList<BookingProfileDetailsViewModel>> GetPagedListBranchAsync(int branchID, int pageNumber, int pageSize,
            string searchTerm, BookingStatus? status)
        {
            var query = _unitOfWork.Repository<BookingModel>().Query();

            query = query.Include(b => b.Customer);

            query = query.Where(b => b.Branch_Id == branchID);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(s => s.Customer.FullName.ToLower().Contains(searchTerm) || s.Customer.PhoneNumber.ToLower().Contains(searchTerm));
            }

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            var projectedQuery = query.Select(b => new BookingProfileDetailsViewModel
            {
                BookingId = b.Booking_Id,
                Customer_Name = b.Customer.FullName,
                Customer_Phone = b.Customer.PhoneNumber,
                Customer_Email = b.Customer.Email,
                Customer_Address = b.Customer.Address,
                Id_Stylist = b.Stylist_Id,
                Stylist_Name = b.StylistProfile.Staff.FullName,
                Stylist_Image = b.StylistProfile.Staff.Avatar_Name,
                Id_Skinner = b.Skinner_Id,
                Skinner_Name = b.SkinnerProfile.Staff.FullName,
                Skinner_Image = b.SkinnerProfile.Staff.Avatar_Name,
                Branch_Name = b.Branch.Branch_Name,
                Date_Booking = b.Booking_Date,
                TimeSlot = b.StartSlot.TimeLabel,
                Status = b.Status,
                FinalPrice = b.TotalPrice,
                TotalDuration = b.TotalDuration,
                UpdateById = b.UpdatedById,
                UpdateByName = b.UpdatedBy != null ? b.UpdatedBy.FullName : null

            });

            var bookingPaginate = projectedQuery.OrderByDescending(b => b.BookingId);
            return await PaginatedList<BookingProfileDetailsViewModel>.CreateAsync(bookingPaginate, pageNumber, pageSize);
        }

        public async Task<PaginatedList<BookingProfileDetailsViewModel>> GetPagedListStaffIdAsync(string StaffId, int pageNumber, int pageSize,
            string searchTerm, BookingStatus? status)
        {
            var query = _unitOfWork.Repository<BookingModel>().Query();

            query = query.Include(b => b.Customer);

            query = query.Where(b => b.Stylist_Id == StaffId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(s => s.Customer.FullName.ToLower().Contains(searchTerm) || s.Customer.PhoneNumber.ToLower().Contains(searchTerm));
            }

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            var projectedQuery = query.Select(b => new BookingProfileDetailsViewModel
            {
                BookingId = b.Booking_Id,
                Customer_Name = b.Customer.FullName,
                Customer_Phone = b.Customer.PhoneNumber,
                Customer_Email = b.Customer.Email,
                Customer_Address = b.Customer.Address,
                Id_Stylist = b.Stylist_Id,
                Stylist_Name = b.StylistProfile.Staff.FullName,
                Stylist_Image = b.StylistProfile.Staff.Avatar_Name,
                Id_Skinner = b.Skinner_Id,
                Skinner_Name = b.SkinnerProfile.Staff.FullName,
                Skinner_Image = b.SkinnerProfile.Staff.Avatar_Name,
                Branch_Name = b.Branch.Branch_Name,
                Date_Booking = b.Booking_Date,
                TimeSlot = b.StartSlot.TimeLabel,
                Status = b.Status,
                FinalPrice = b.TotalPrice,
                TotalDuration = b.TotalDuration,
                UpdateById = b.UpdatedById,
                UpdateByName = b.UpdatedBy != null ? b.UpdatedBy.FullName : null

            });

            var bookingPaginate = projectedQuery.OrderByDescending(b => b.BookingId);
            return await PaginatedList<BookingProfileDetailsViewModel>.CreateAsync(bookingPaginate, pageNumber, pageSize);
        }

        public async Task<ServiceResult<string?>> GetAvailableStylistIdAsync(int branchId, DateTime date, int startSlotId, int durationMinutes)
        {
            var neededSlots = GetListSlotIdsInRange(startSlotId, durationMinutes);
            var bookingDateOnly = DateOnly.FromDateTime(date);
            string roleStylist = _roleManager.FindByNameAsync("Stylist").Result.Id;

            if (roleStylist == null) return ServiceResult<string?>.Failed("Không tìm thấy Role Stylist");

            var allStylists = await _unitOfWork.Repository<StaffProfileModel>().Query()
                .Include(s => s.Staff)
                .Where(s => s.Branch_Id == branchId && s.Staff.Status == true && s.Staff.RoleId == roleStylist)
                .ToListAsync();

            foreach (var stylist in allStylists)
            {
                var isBusy = await _unitOfWork.Repository<StaffScheduleModel>().ExistsAsync(x =>
                    x.Staff_Id == stylist.StaffId &&
                    x.Work_Date == bookingDateOnly &&
                    neededSlots.Contains(x.Slot_Id));

                if (!isBusy)
                    return ServiceResult<string>.Success(stylist.StaffId);
            }

            return ServiceResult<string>.Failed("Không còn stylist nào rảnh trong khung giờ này cả");
        }

        public async Task<ServiceResult<string?>> GetAvailableSkinnerIdAsync(int branchId, DateTime date, int startSlotId, int durationMinutes, string? excludeStaffId = null)
        {
            var neededSlots = GetListSlotIdsInRange(startSlotId, durationMinutes);
            var bookingDateOnly = DateOnly.FromDateTime(date);
            var role = await _roleManager.FindByNameAsync("Skinner");

            if (role == null) return ServiceResult<string?>.Failed("Không tìm thấy Role Skinner");

            var allSkinner = await _unitOfWork.Repository<StaffProfileModel>().Query()
                .Include(s => s.Staff)
                .Where(s => s.Branch_Id == branchId && s.Staff.Status == true && s.Staff.RoleId == role.Id)
                .Where(s => s.StaffId != excludeStaffId) 
                .ToListAsync();

            foreach (var skinner in allSkinner)
            {
                var isBusy = await _unitOfWork.Repository<StaffScheduleModel>().ExistsAsync(x =>
                    x.Staff_Id == skinner.StaffId &&
                    x.Work_Date == bookingDateOnly &&
                    neededSlots.Contains(x.Slot_Id));

                if (!isBusy) return ServiceResult<string?>.Success(skinner.StaffId);
            }
            return ServiceResult<string?>.Failed("Không còn Skinner nào rảnh.");
        }

        public async Task<ServiceResult<IEnumerable<SlotSelectionViewModel>>> GetSlotsAvailableForBranchRandom(int branchId, string? stylistId, string? skinnerId, DateOnly date, int durationMinutes)
        {
            var roleStylist = await _roleManager.FindByNameAsync("Stylist");
            var roleSkinner = await _roleManager.FindByNameAsync("Skinner");

            if (roleStylist == null || roleSkinner == null)
                return ServiceResult<IEnumerable<SlotSelectionViewModel>>.Failed("Cấu hình Role thợ không tồn tại.");

            var allStaffInBranch = await _unitOfWork.Repository<StaffProfileModel>().Query()
                .Include(s => s.Staff)
                .Where(s => s.Branch_Id == branchId && s.Staff.Status == true)
                .ToListAsync();

            var stylists = allStaffInBranch.Where(s => s.Staff.RoleId == roleStylist.Id).ToList();

            var skinners = allStaffInBranch.Where(s => s.Staff.RoleId == roleSkinner.Id).ToList();

            var allFixedSlots = await _unitOfWork.Repository<FixedTimeSlotModel>().Query()
                .OrderBy(s => s.TimeLabel).ToListAsync();

            var busySchedules = await _unitOfWork.Repository<StaffScheduleModel>().Query()
                .Where(x => x.Work_Date == date)
                .ToListAsync();

            var viewModel = new List<SlotSelectionViewModel>();

            foreach (var slot in allFixedSlots)
            {
                var neededSlotIds = GetListSlotIdsInRange(slot.SlotId, durationMinutes);
                if (neededSlotIds.Count * 20 < durationMinutes)
                {
                    viewModel.Add(new SlotSelectionViewModel { SlotId = slot.SlotId, TimeRange = slot.TimeLabel, IsAvailable = false });
                    continue;
                }

                bool isSlotAvailable = false;

                var availableStylists = stylists.Where(st =>
                    (stylistId != "RANDOM" ? st.StaffId == stylistId : true) &&
                    slot.TimeLabel >= st.Start_Work_Time && slot.TimeLabel <= st.End_Work_Time &&
                    !busySchedules.Any(busy => busy.Staff_Id == st.StaffId && neededSlotIds.Contains(busy.Slot_Id))
                ).ToList();

                var availableSkinners = skinners.Where(sk =>
                    (skinnerId != "RANDOM" ? sk.StaffId == skinnerId : true) && 
                    slot.TimeLabel >= sk.Start_Work_Time && slot.TimeLabel <= sk.End_Work_Time &&
                    !busySchedules.Any(busy => busy.Staff_Id == sk.StaffId && neededSlotIds.Contains(busy.Slot_Id))
                ).ToList();

                foreach (var st in availableStylists)
                {
                    if (availableSkinners.Any(sk => sk.StaffId != st.StaffId))
                    {
                        isSlotAvailable = true;
                        break;
                    }
                }

                viewModel.Add(new SlotSelectionViewModel
                {
                    SlotId = slot.SlotId,
                    TimeRange = slot.TimeLabel,
                    IsAvailable = isSlotAvailable
                });
            }

            return ServiceResult<IEnumerable<SlotSelectionViewModel>>.Success(viewModel);
        }
    }
}
