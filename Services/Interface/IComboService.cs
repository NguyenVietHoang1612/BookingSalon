using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel.BookingSalonViewModel;

namespace BookingSalon.Services.Interface
{
    public interface IComboService
    {
        Task<IEnumerable<ComboModel>> GetAllComboAsync();
        Task<ServiceResult<ComboViewModel>> Create(ComboViewModel vm);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
