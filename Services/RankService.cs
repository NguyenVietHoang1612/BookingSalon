using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class RankService : IRankService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RankService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<IEnumerable<RankModel>>> GetAllAsync()
        {
            try
            {
                var ranks = await _unitOfWork.Repository<RankModel>()
                    .Query()
                    .OrderBy(r => r.MinPoint)
                    .ToListAsync();

                return ServiceResult<IEnumerable<RankModel>>.Success(ranks);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<RankModel>>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<RankModel>> GetByIdAsync(int id)
        {
            try
            {
                var rank = await _unitOfWork.Repository<RankModel>().GetByIdAsync(id);

                if (rank == null)
                    return ServiceResult<RankModel>.Failed("Không tìm thấy hạng thành viên");

                return ServiceResult<RankModel>.Success(rank);
            }
            catch (Exception ex)
            {
                return ServiceResult<RankModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<RankModel>> CreateAsync(RankModel rank)
        {
            if (string.IsNullOrEmpty(rank.RankName))
                return ServiceResult<RankModel>.Failed("Tên hạng không được để trống");

            try
            {
                var repo = _unitOfWork.Repository<RankModel>();

                var exists = await repo.ExistsAsync(r => r.RankName == rank.RankName || r.MinPoint == rank.MinPoint);
                if (exists)
                    return ServiceResult<RankModel>.Failed("Tên hạng hoặc mốc điểm này đã tồn tại");

                var newRank = new RankModel
                {
                    RankName = rank.RankName,
                    MinPoint = rank.MinPoint,
                    MaxBookingDays = rank.MaxBookingDays,
                    DiscountPercent = rank.DiscountPercent,
                    Description = rank.Description
                };

                await repo.AddAsync(newRank);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<RankModel>.Success(newRank);
            }
            catch (Exception ex)
            {
                return ServiceResult<RankModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<RankModel>> UpdateAsync(int id, RankModel rank)
        {
            try
            {
                var repo = _unitOfWork.Repository<RankModel>();
                var existingRank = await repo.GetByIdAsync(id);

                if (existingRank == null)
                    return ServiceResult<RankModel>.Failed("Không tìm thấy hạng thành viên");

                var pointExists = await repo.ExistsAsync(r => r.MinPoint == rank.MinPoint && r.RankId != id);
                if (pointExists)
                    return ServiceResult<RankModel>.Failed("Mốc điểm này đã được sử dụng bởi hạng khác");

                existingRank.RankName = rank.RankName;
                existingRank.MinPoint = rank.MinPoint;
                existingRank.MaxBookingDays = rank.MaxBookingDays;
                existingRank.DiscountPercent = rank.DiscountPercent;
                existingRank.Description = rank.Description;

                repo.Update(existingRank);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<RankModel>.Success(existingRank);
            }
            catch (Exception ex)
            {
                return ServiceResult<RankModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<PaginatedList<RankModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<RankModel>().Query();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(r => r.RankName.ToLower().Contains(searchTerm));
            }

            query = query.OrderBy(r => r.RankId);

            return await PaginatedList<RankModel>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<ServiceResult<RankModel>> DeleteAsync(int id)
        {
            try
            {
                var rank = await _unitOfWork.Repository<RankModel>().GetByIdAsync(id);
                if (rank == null) return ServiceResult<RankModel>.Failed("Không tìm thấy hạng");


                var hasMembers = await _unitOfWork.Repository<CustomerRankModel>().ExistsAsync(cr => cr.RankId == id);
                if (hasMembers)
                    return ServiceResult<RankModel>.Failed("Không thể xóa hạng này vì đang có khách hàng thuộc hạng này.");

                _unitOfWork.Repository<RankModel>().Delete(rank);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<RankModel>.Success(rank);
            }
            catch (Exception ex)
            {
                return ServiceResult<RankModel>.Failed($"Lỗi: {ex.Message}");
            }
        }
    }
}
