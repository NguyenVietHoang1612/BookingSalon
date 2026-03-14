using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel.BookingSalonViewModel;

namespace BookingSalon.Services.Interface
{
    public interface ICustomerRankService
    {
        Task<CustomerRankModel> GetByCustomerIdAsync(string customerId);
        Task<ServiceResult<CustomerRankModel>> CreateAsync(CustomerRankModel model);
        Task<ServiceResult<CustomerRankModel>> UpdateAsync(string customerId, CustomerRankModel model);
        Task<ServiceResult<CustomerRankModel>> UpdatePointsAsync(string customerId, int addedPoints);
        Task<ServiceResult<CustomerRankModel>> DeleteAsync(string customerId);
    }
}
