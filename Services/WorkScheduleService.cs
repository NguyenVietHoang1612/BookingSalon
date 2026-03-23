using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class WorkScheduleService : IWorkScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public WorkScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IEnumerable<CalendarEventViewModel>> GetWorkSchedule(string staffId)
        {
            var schedules = await _unitOfWork.Repository<StaffScheduleModel>().Query()
                .Include(s => s.TimeSlot)
                .Where(s => s.Staff_Id == staffId && s.Booking_Id != null)
                .ToListAsync();

            var bookingIds = schedules.Select(s => s.Booking_Id).Distinct();

            var bookings = await _unitOfWork.Repository<BookingModel>().Query()
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails).ThenInclude(d => d.Service)
                .Where(b => bookingIds.Contains(b.Booking_Id))
                .ToListAsync();

            var eventList = schedules
                .GroupBy(s => s.Booking_Id) 
                .Select(group => {
                    var bookingId = group.Key;
                    var booking = bookings.FirstOrDefault(b => b.Booking_Id == bookingId);


                    var firstSlot = group.OrderBy(s => s.TimeSlot.TimeLabel).First();
                    var startDateTime = firstSlot.Work_Date.ToDateTime(TimeOnly.FromTimeSpan(firstSlot.TimeSlot.TimeLabel));

                    string serviceNames = booking != null
                        ? string.Join(", ", booking.BookingDetails.Select(d => d.Service.Service_Name))
                        : "Dịch vụ";

                    return new CalendarEventViewModel
                    {
                        Id = firstSlot.ScheduleId, 
                        Title = $"{booking?.Customer?.FullName} | {serviceNames} | Ghi chú: {booking?.Note ?? "Không có ghi chú"}",
                        Start = startDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        End = startDateTime.AddMinutes(booking?.TotalDuration ?? 30).ToString("yyyy-MM-ddTHH:mm:ss"),
                        Color = "#dc3545",
                        ExtendedProps = new ExtendedProps { Status = false }
                    };
                }).ToList();

            return eventList;
        }
    }
}
