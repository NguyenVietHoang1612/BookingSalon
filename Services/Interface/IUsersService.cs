using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel;
using Microsoft.AspNetCore.Identity;

namespace BookingSalon.Services.Interface
{
    public interface IUsersService
    {
        Task<IEnumerable<UsersModel>> GetAllCustomerAsync();
        Task<IEnumerable<UsersModel>> GetAllStylistAsync();
        Task<IEnumerable<UsersModel>> GetAllSkinnerAsync();
        Task<IEnumerable<UsersModel>> GetAllReceptionAsync();
        Task<WardModel> GetWardById(UsersModel user);
        Task<UsersModel?> GetByIdAsync(string id);
        Task<IdentityResult> CreateUserCustomerAsync(UserCreateViewModel user);
        Task<IdentityResult> UpdateUserCustomerAsync(UserUpdateViewModel user, string id);
        Task<IdentityResult> UpdateUserAsync(UserViewModel user, string id);
        Task<IdentityResult> UpdateCustomerProfileAsync(string userId, ProfileViewModel model);
        Task<ServiceResult<UsersModel>> Delete(string id);
        Task<PaginatedList<CustomerRankVM>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm, string sortOrder);
    }
}
