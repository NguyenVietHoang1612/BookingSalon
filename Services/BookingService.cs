using BookingSalon.Data.Repository;
using BookingSalon.Migrations;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel.BookingSalonViewModel;
using BookingSalon.Services.Interface;
using Microsoft.EntityFrameworkCore;


namespace BookingSalon.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<BookingViewModel>> CreateAsync(BookingViewModel bookingVM)
        {
            try
            {
                var booking = bookingVM.NewBooking;
                booking.Create_At = DateTime.Now;

                booking.Status = "pending";
                decimal calculatedTotal = 0;
                int totalDuration = 0;
                var serviceRepo = _unitOfWork.Repository<Service>();

                foreach (var detail in bookingVM.BookingDetails)
                {
                    var service = await serviceRepo.GetByIdAsync(detail.Service_Id);
                    if (service != null)
                    {
                        calculatedTotal += service.Price;
                        totalDuration += service.DurationInMinutes;
                    }
                }
                booking.TotalPrice = calculatedTotal;
                booking.TotalDuration = totalDuration;


                await _unitOfWork.Repository<Booking>().AddAsync(booking);
                await _unitOfWork.SaveChangesAsync();


                foreach (var detail in bookingVM.BookingDetails)
                {
                    detail.Booking_Id = booking.Booking_Id;
                    detail.Created_At = DateTime.Now;
                    await _unitOfWork.Repository<BookingDetail>().AddAsync(detail);
                }

               
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<BookingViewModel>.Success(bookingVM);
            }
            catch (Exception ex)
            {
                return ServiceResult<BookingViewModel>.Failed($"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Booking>> UpdateAsync(int id, string status)
        {
            if (status == null) return ServiceResult<Booking>.Failed("Status không được để trống");

            var bookingRepo = _unitOfWork.Repository<Booking>();
            var booking = await GetByIdAsync(id);

            if (booking == null)
            {
                return ServiceResult<Booking>.Failed("Không tìm thấy booking");
            }

            booking.Data.Status = status;
            booking.Data.Update_At = DateTime.Now;
            bookingRepo.Update(booking.Data);

            if (booking.Data.Status == "confirm")
            {
                UpdateStylistTimeWork(booking.Data.TotalDuration, booking.Data.Stylist_Id, booking.Data.Booking_Date, booking.Data.Slot_Id);
            }

            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<Booking>.Success(booking.Data);
        }

        public async Task<ServiceResult<Booking>> DeleteAsync(int id)
        {
            try
            {
                var booking = await GetByIdAsync(id);

                if (booking == null)
                    return ServiceResult<Booking>.Failed("Không tìm thấy dịch vụ");

                _unitOfWork.Repository<Booking>().Delete(booking.Data);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<Booking>.Success(booking.Data);
            }
            catch (Exception ex)
            {
                return ServiceResult<Booking>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<IEnumerable<BookingProfileDetailsViewModel>> GetAllAsync()
        {
            var bookingData = await _unitOfWork.Repository<Booking>()
            .Query()
            .Select(b => new BookingProfileDetailsViewModel
            {
                BookingId = b.Booking_Id,
                Customer_Name = b.Customer.FullName,
                Customer_Phone = b.Customer.PhoneNumber,
                Customer_Email = b.Customer.Email,
                Customer_Address = b.Customer.Address,
                Id_Stylist = b.Stylist_Id,
                Stylist_Name = b.Stylist.FullName,
                Branch_Name = b.Branch.Branch_Name,
                Date_Booking = b.Booking_Date,
                TimeSlot = b.StartSlot.TimeLabel,
                Status = b.Status,
                TotalPrice = b.TotalPrice
            })
            .ToListAsync();

            return bookingData;
        }

        public async Task<IEnumerable<Branch>> GetAllBranchesAsync()
        {
            var branches = await _unitOfWork.Repository<Branch>().GetAllAsync();
            return branches;
        }

        public async Task<IEnumerable<FixedTimeSlot>> GetAllFixedTimeSlotsAsync()
        {
            var fixedTimeSlots = await _unitOfWork.Repository<FixedTimeSlot>().GetAllAsync();
            return fixedTimeSlots;
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            var services = await _unitOfWork.Repository<Service>().GetAllAsync();
            return services;
        }

        public async Task<IEnumerable<TypeOfService>> GetAllTypeOfServiceAsync()
        {
            var typeOfServices = await _unitOfWork.Repository<TypeOfService>().GetAllAsync();
            return typeOfServices;
        }

        public async Task<ServiceResult<Booking>> GetByIdAsync(int id)
        {
            try
            {
                var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(id);

                if (booking == null)
                    return ServiceResult<Booking>.Failed("Không tìm thấy branch");

                return ServiceResult<Booking>.Success(booking);
            }
            catch (Exception ex)
            {
                return ServiceResult<Booking>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task UpdateStylistTimeWork(int totalDuration, string stylist_Id, DateOnly date, int slot_Id)
        {
            const int SLOT_DURATION = 20;
            int numberOfSlotsToLock = (int)Math.Ceiling((double)totalDuration / SLOT_DURATION);

            var stylistTimeWorkRepo = _unitOfWork.Repository<StylistTimeWork>();

            var slotStylist = await stylistTimeWorkRepo
                .Query()
                .Where(sw => sw.Stylist_Id == stylist_Id
                          && sw.Work_Date == date)
                .OrderBy(sw => sw.Stylist_Time_Work_Id)
                .ToListAsync();


            var startSlotIndex = slotStylist.FindIndex(s => s.Slot_Id == slot_Id);

            if (startSlotIndex != -1)
            {

                for (int i = 0; i < numberOfSlotsToLock; i++)
                {
                    int currentIndex = startSlotIndex + i;


                    if (currentIndex < slotStylist.Count)
                    {
                        slotStylist[currentIndex].Is_Available = false;
                        stylistTimeWorkRepo.Update(slotStylist[currentIndex]);   
                    }
                }
            }

        } 

        public async Task GenerateWorkSlots(string stylistId, DateOnly date)
        {

            var profile = await _unitOfWork.Repository<StylistProfile>().GetByIdAsync(stylistId);
            if (profile == null) return;


            var slots = await _unitOfWork.Repository<FixedTimeSlot>()
                .Query()
                .Where(s => s.TimeLabel >= profile.Start_Work_Time && s.TimeLabel < profile.End_Work_Time)
                .ToListAsync();



            if (slots.Count > 0)
            {
                TimeSpan lunchStart = new TimeSpan(12, 0, 0);
                TimeSpan lunchEnd = new TimeSpan(13, 0, 0);

                foreach (var slot in slots)
                {

                    if (slot.TimeLabel >= lunchStart && slot.TimeLabel < lunchEnd)
                    {
                        continue;
                    }

                    var exists = await _unitOfWork.Repository<StylistTimeWork>()
                        .Query()
                        .AnyAsync(sw => sw.Stylist_Id == stylistId && sw.Work_Date == date && sw.Slot_Id == slot.SlotId);

                    if (!exists)
                    {
                        await _unitOfWork.Repository<StylistTimeWork>().AddAsync(new StylistTimeWork
                        {
                            Stylist_Id = stylistId,
                            Work_Date = date,
                            Slot_Id = slot.SlotId,
                            Is_Available = true,
                            Create_At = DateTime.Now
                        });
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<StylistProfile>> GetAllStylistAsync()
        {
            //var stylists = await _userManager.GetUsersInRoleAsync("Stylist");

            var profileStylist = _unitOfWork.Repository<StylistProfile>()
                .Query()
                .Include(s => s.Stylist);

            return profileStylist;
        }

        public async Task<ServiceResult<IEnumerable<StylistTimeWork>>> GetSlots(string stylistId, string dateStr)
        {
            if (string.IsNullOrEmpty(stylistId) || string.IsNullOrEmpty(dateStr)) return ServiceResult<IEnumerable<StylistTimeWork>>.Failed("Không được để trống ngày/sylist đặt");

            var stylistTimeWorksRepo = _unitOfWork.Repository<StylistTimeWork>();

            DateOnly selectedDate = DateOnly.Parse(dateStr);
            bool hasData = await stylistTimeWorksRepo
                .Query()
                .AnyAsync(sw => sw.Stylist_Id == stylistId && sw.Work_Date == selectedDate);

            if (!hasData)
            {

                await GenerateWorkSlots(stylistId, selectedDate);
            }

            var workSlots = await stylistTimeWorksRepo
                .Query()
                .Include(sw => sw.TimeSlot)
                .Where(sw => sw.Stylist_Id == stylistId && sw.Work_Date == selectedDate)
                .OrderBy(sw => sw.TimeSlot.Sort_Order).ToListAsync();

            return ServiceResult<IEnumerable<StylistTimeWork>>.Success(workSlots);
        }

        public async Task<ServiceResult<BookingDetailViewModel>> GetAllBookingDetailsAsync(int id)
        {
            try
            {
                var booking = await _unitOfWork.Repository<Booking>()
                .Query()
                .Include(bd => bd.BookingDetails)
                    .ThenInclude(d => d.Service)
                .Select(bd => new BookingDetailViewModel
                {
                    BookingId = bd.Booking_Id,
                    Customer_Name = bd.Customer.FullName,
                    Customer_Phone = bd.Customer.PhoneNumber,
                    Customer_Email = bd.Customer.Email,
                    Customer_Address = bd.Customer.Address,
                    Id_Stylist = bd.Stylist_Id,
                    Stylist_Name = bd.Stylist.FullName,
                    Branch_Name = bd.Branch.Branch_Name,
                    Branch_Address = bd.Branch.Address,
                    Date_Booking = bd.Booking_Date,
                    TimeSlot = bd.StartSlot.TimeLabel,
                    TotalDuration = bd.TotalDuration,
                    Status = bd.Status,
                    TotalPrice = bd.TotalPrice,
                    bookingDetails = bd.BookingDetails,
                    Create_At = bd.Create_At
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

        public async Task<PaginatedList<BookingProfileDetailsViewModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<Booking>().Query();

            query = query.Include(b => b.Customer);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(s => s.Customer.FullName.ToLower().Contains(searchTerm));
            }

            var projectedQuery = query.Select(b => new BookingProfileDetailsViewModel
            {
                BookingId = b.Booking_Id,
                Customer_Name = b.Customer.FullName,
                Customer_Phone = b.Customer.PhoneNumber,
                Customer_Email = b.Customer.Email,
                Customer_Address = b.Customer.Address,
                Id_Stylist = b.Stylist_Id,
                Stylist_Name = b.Stylist.FullName,
                Branch_Name = b.Branch.Branch_Name,
                Date_Booking = b.Booking_Date,
                TimeSlot = b.StartSlot.TimeLabel,
                Status = b.Status,
                TotalPrice = b.TotalPrice
            });

            return await PaginatedList<BookingProfileDetailsViewModel>.CreateAsync(projectedQuery, pageNumber, pageSize);
        }
    }
}
