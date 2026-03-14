using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel;
using BookingSalon.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<IEnumerable<CouponModel>>> GetAllAsync()
        {
            try
            {
                var coupons = await _unitOfWork.Repository<CouponModel>()
                    .Query()
                    .OrderByDescending(c => c.Created_At)
                    .ToListAsync();

                return ServiceResult<IEnumerable<CouponModel>>.Success(coupons);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<CouponModel>>
                    .Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<PaginatedList<CouponModel>> GetPagedListAsync(
            int pageNumber,
            int pageSize,
            string searchTerm)
        {
            var query = _unitOfWork.Repository<CouponModel>().Query();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();

                query = query.Where(c =>
                    c.Code.ToLower().Contains(searchTerm));
            }

            query = query.OrderByDescending(c => c.Created_At);

            return await PaginatedList<CouponModel>
                .CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<ServiceResult<CouponModel>> GetByIdAsync(int id)
        {
            try
            {
                var coupon = await _unitOfWork.Repository<CouponModel>()
                    .GetByIdAsync(id);

                if (coupon == null)
                    return ServiceResult<CouponModel>
                        .Failed("Không tìm thấy coupon");

                return ServiceResult<CouponModel>.Success(coupon);
            }
            catch (Exception ex)
            {
                return ServiceResult<CouponModel>
                    .Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<CouponModel>> CreateAsync(CouponModel model)
        {
            try
            {
                var repo = _unitOfWork.Repository<CouponModel>();

                var codeExists = await repo.ExistsAsync(c => c.Code == model.Code);
                if (codeExists)
                    return ServiceResult<CouponModel>
                        .Failed("Mã coupon đã tồn tại");

                model.Created_At = DateTime.Now;
                model.Updated_At = DateTime.Now;
                model.Used_Count = 0;

                await repo.AddAsync(model);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<CouponModel>.Success(model);
            }
            catch (Exception ex)
            {
                return ServiceResult<CouponModel>
                    .Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<CouponModel>> UpdateAsync(int id, CouponModel model)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var repo = _unitOfWork.Repository<CouponModel>();
                var usageRepo = _unitOfWork.Repository<CouponUsage>();

                var existing = await repo.GetByIdAsync(id);

                if (existing == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ServiceResult<CouponModel>.Failed("Không tìm thấy coupon");
                }

                var codeExists = await repo.ExistsAsync(
                    c => c.Code == model.Code && c.Id != id);

                if (codeExists)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ServiceResult<CouponModel>.Failed("Mã coupon đã tồn tại");
                }

                var oldUsages = await usageRepo.Query()
                    .Where(u => u.CouponId == id)
                    .ToListAsync();

                if (oldUsages.Any())
                {
                    foreach (var usage in oldUsages)
                    {
                        usageRepo.Delete(usage);
                    }
                }
                existing.Code = model.Code;
                existing.Discount_Type = model.Discount_Type;
                existing.Discount_value = model.Discount_value;
                existing.Min_Order_Amount = model.Min_Order_Amount;
                existing.Usage_Limit = model.Usage_Limit;
                existing.MaxUsagePerUser = model.MaxUsagePerUser;
                existing.IsActive = model.IsActive;
                existing.Expires_At = model.Expires_At;
                existing.Updated_At = DateTime.Now;

                existing.Used_Count = 0;

                repo.Update(existing);

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return ServiceResult<CouponModel>.Success(existing);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult<CouponModel>.Failed($"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ServiceResult<CouponModel>> DeleteAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<CouponModel>();
                var coupon = await repo.GetByIdAsync(id);

                if (coupon == null)
                    return ServiceResult<CouponModel>
                        .Failed("Không tìm thấy coupon");

                repo.Delete(coupon);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<CouponModel>.Success(coupon);
            }
            catch (Exception ex)
            {
                return ServiceResult<CouponModel>
                    .Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<CouponValidateViewModel>>
            ValidateForBookingAsync(
                string code,
                decimal orderAmount,
                string userId)
        {
            try
            {
                var repo = _unitOfWork.Repository<CouponModel>();
                var usageRepo = _unitOfWork.Repository<CouponUsage>();

                if (string.IsNullOrWhiteSpace(code))
                    return ServiceResult<CouponValidateViewModel>
                        .Failed("Mã coupon không hợp lệ");

                var coupon = await repo.Query()
                    .FirstOrDefaultAsync(c =>
                        c.Code.ToLower() == code.ToLower());

                if (coupon == null)
                    return ServiceResult<CouponValidateViewModel>
                        .Failed("Coupon không tồn tại");

                if (!coupon.IsActive)
                    return ServiceResult<CouponValidateViewModel>
                        .Failed("Coupon chưa được kích hoạt");

                if (coupon.Expires_At <= DateTime.Now)
                    return ServiceResult<CouponValidateViewModel>
                        .Failed("Coupon đã hết hạn");

                if (coupon.Used_Count >= coupon.Usage_Limit)
                    return ServiceResult<CouponValidateViewModel>
                        .Failed("Coupon đã hết lượt sử dụng");

                if (orderAmount < coupon.Min_Order_Amount)
                    return ServiceResult<CouponValidateViewModel>
                        .Failed($"Đơn tối thiểu {coupon.Min_Order_Amount:n0}đ");

                if (coupon.MaxUsagePerUser.HasValue &&
                    coupon.MaxUsagePerUser.Value > 0)
                {
                    var usedByUser = await usageRepo.Query()
                        .CountAsync(u =>
                            u.CouponId == coupon.Id &&
                            u.UserId == userId);

                    if (usedByUser >= coupon.MaxUsagePerUser.Value)
                        return ServiceResult<CouponValidateViewModel>
                            .Failed("Bạn đã dùng hết lượt cho coupon này");
                }

                decimal discount;

                if (coupon.Discount_Type == DiscountType.Percentage)
                {
                    discount = orderAmount * coupon.Discount_value / 100;
                }
                else
                {
                    discount = coupon.Discount_value;
                }

                discount = Math.Min(discount, orderAmount);

                CouponValidateViewModel vm = new CouponValidateViewModel
                {
                    Discount= discount,
                    DiscountType = coupon.Discount_Type,
                    DiscountValue = coupon.Discount_value,
                    Code = coupon.Code,
                    CouponId = coupon.Id
                };

                return ServiceResult<CouponValidateViewModel>
                    .Success(vm);
            }
            catch (Exception ex)
            {
                return ServiceResult<CouponValidateViewModel>
                    .Failed($"Lỗi: {ex.Message}");
            }
        }
    }
}