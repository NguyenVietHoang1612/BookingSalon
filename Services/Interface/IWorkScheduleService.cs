using BookingSalon.Areas.Admin.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace BookingSalon.Services.Interface
{
    public interface IWorkScheduleService
    {
        Task<IEnumerable<CalendarEventViewModel>> GetWorkSchedule(string stylistId);
    }
}
