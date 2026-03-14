using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class CustomerRankService : ICustomerRankService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<UsersModel> _userManager;
        public CustomerRankService(IUnitOfWork unitOfWork, UserManager<UsersModel> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<CustomerRankModel> GetByCustomerIdAsync(string customerId)
        {
            return await _unitOfWork.Repository<CustomerRankModel>()
                .Query()
                .Include(cr => cr.Rank)
                .Include(cr => cr.Customer)
                .FirstOrDefaultAsync(cr => cr.Customer_Id == customerId);
        }

        public async Task<ServiceResult<CustomerRankModel>> CreateAsync(CustomerRankModel model)
        {
            try
            {
                var repo = _unitOfWork.Repository<CustomerRankModel>();

                var exists = await repo.ExistsAsync(cr => cr.Customer_Id == model.Customer_Id);
                if (exists) return ServiceResult<CustomerRankModel>.Failed("Khách hàng này đã có thông tin Rank.");

                var appropriateRank = await _unitOfWork.Repository<RankModel>().Query()
                    .Where(r => r.MinPoint <= (model.LifetimePoints >= 0 ? model.LifetimePoints : 0))
                    .OrderByDescending(r => r.MinPoint)
                    .FirstOrDefaultAsync();

                if (appropriateRank == null)
                    return ServiceResult<CustomerRankModel>.Failed("Hệ thống chưa cấu hình bảng Rank.");

                model.RankId = appropriateRank.RankId;
                model.Created_At = DateTime.Now;
                model.Update_At = DateTime.Now;
                model.CurrentPoints = Math.Max(0, model.CurrentPoints);
                model.LifetimePoints = Math.Max(0, model.LifetimePoints);

                await repo.AddAsync(model);

                return ServiceResult<CustomerRankModel>.Success(model);
            }
            catch (Exception ex)
            {
                return ServiceResult<CustomerRankModel>.Failed(ex.Message);
            }
        }

        public async Task<ServiceResult<CustomerRankModel>> UpdateAsync(string customerId, CustomerRankModel model)
        {
            try
            {
                var repo = _unitOfWork.Repository<CustomerRankModel>();

                var existingRank = await repo.Query()
                    .FirstOrDefaultAsync(cr => cr.Customer_Id == customerId);

                if (existingRank == null)
                    return ServiceResult<CustomerRankModel>.Failed("Không tìm thấy thông tin hạng của khách hàng này.");

                existingRank.CurrentPoints = model.CurrentPoints;
                existingRank.LifetimePoints = model.LifetimePoints;
                existingRank.Is_Active = model.Is_Active;
                existingRank.Update_At = DateTime.Now;

                var appropriateRank = await _unitOfWork.Repository<RankModel>().Query()
                    .Where(r => r.MinPoint <= model.LifetimePoints)
                    .OrderByDescending(r => r.MinPoint)
                    .FirstOrDefaultAsync();

                if (appropriateRank != null)
                {
                    existingRank.RankId = appropriateRank.RankId;
                }
                else
                {
                    var minRank = await _unitOfWork.Repository<RankModel>().Query()
                        .OrderBy(r => r.MinPoint)
                        .FirstOrDefaultAsync();
                    if (minRank != null) existingRank.RankId = minRank.RankId;
                }

                repo.Update(existingRank);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<CustomerRankModel>.Success(existingRank);
            }
            catch (Exception ex)
            {
                return ServiceResult<CustomerRankModel>.Failed($"Lỗi khi cập nhật: {ex.Message}");
            }
        }

        public async Task<ServiceResult<CustomerRankModel>> DeleteAsync(string customerId)
        {
            try
            {
                var repo = _unitOfWork.Repository<CustomerRankModel>();
                var item = await repo.Query().FirstOrDefaultAsync(x => x.Customer_Id == customerId);

                if (item == null) return ServiceResult<CustomerRankModel>.Failed("Không tìm thấy dữ liệu.");

                repo.Delete(item);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult<CustomerRankModel>.Success(item);
            }
            catch (Exception ex)
            {
                return ServiceResult<CustomerRankModel>.Failed(ex.Message);
            }
        }

        public async Task<ServiceResult<CustomerRankModel>> UpdatePointsAsync(string customerId, int addedPoints)
        {
            try
            {
                var repo = _unitOfWork.Repository<CustomerRankModel>();
                var customerRank = await repo.Query()
                    .Include(cr => cr.Rank)
                    .FirstOrDefaultAsync(cr => cr.Customer_Id == customerId);

                if (customerRank == null) return ServiceResult<CustomerRankModel>.Failed("Không tìm thấy khách hàng.");

                customerRank.CurrentPoints += addedPoints;
                customerRank.LifetimePoints += addedPoints;
                customerRank.Update_At = DateTime.Now;


                var newRank = await _unitOfWork.Repository<RankModel>().Query()
                    .Where(r => r.MinPoint <= customerRank.LifetimePoints)
                    .OrderByDescending(r => r.MinPoint)
                    .FirstOrDefaultAsync();

                if (newRank != null && newRank.RankId != customerRank.RankId)
                {
                    customerRank.RankId = newRank.RankId;
                }

                repo.Update(customerRank);

                return ServiceResult<CustomerRankModel>.Success(customerRank);
            }
            catch (Exception ex)
            {
                return ServiceResult<CustomerRankModel>.Failed(ex.Message);
            }
        }
    }
}
