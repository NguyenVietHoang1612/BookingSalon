using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookingSalon.Services.Interface
{
    public interface IStaffProfileService
    {
        Task<IEnumerable<StaffProfileModel>> GetAllAsync();
        Task<IEnumerable<StaffProfileModel>> GetAllStylistActiveAsync();
        Task<IEnumerable<StaffProfileModel>> GetAllSkinnerActiveAsync();
        Task<IEnumerable<IdentityRole>> GetRoleStaff();

        Task<ServiceResult<StaffProfileModel>> GetByIdAsync(string id);
        Task<ServiceResult<StaffCombinedCreateVM>> CreateAsync(StaffCombinedCreateVM stylistProfile);
        Task<ServiceResult<StaffCombinedUpdateVM>> UpdateAsync(string id, StaffCombinedUpdateVM stylistProfile);
        Task<ServiceResult<bool>> StaffSelfUpdateAsync(string staffId, StaffSelfUpdateVM model);
        Task<ServiceResult<bool>> DeleteAsync(string id);
        Task<PaginatedList<StaffProfileModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm);
    }
}
