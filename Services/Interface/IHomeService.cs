using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel;

namespace BookingSalon.Services.Interface
{
    public interface IHomeService
    {
        Task<HomeViewModel> GetAllStaffAsync();
    }
}
