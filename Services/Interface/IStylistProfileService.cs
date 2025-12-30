using BookingSalon.Areas.Admin.Models;
using BookingSalon.Models.Entities;

namespace BookingSalon.Services.Interface
{
    public interface IStylistProfileService
    {
        Task<IEnumerable<StylistProfile>> GetAllAsync();
        Task<IEnumerable<Branch>> GetAllBranchAsync();
        Task<ServiceResult<StylistProfile>> GetByIdAsync(string id);
        Task<ServiceResult<CreateStylistProfileVM>> CreateAsync(CreateStylistProfileVM stylistProfile);
        Task<ServiceResult<UpdateStylistProfileVM>> UpdateAsync(string id, UpdateStylistProfileVM stylistProfile);
        Task<ServiceResult<StylistProfile>> DeleteAsync(string id);
    }
}
