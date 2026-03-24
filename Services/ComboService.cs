using BookingSalon.Areas.Admin.Models;
using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class ComboService : IComboService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public ComboService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<IEnumerable<ComboModel>> GetAllComboAsync()
        {
            return await _unitOfWork.Repository<ComboModel>()
                .Query()
                .Include(c => c.ComboServices)
                    .ThenInclude(cs => cs.Service)
                .ToListAsync();
        }

        public async Task<ServiceResult<ComboViewModel>> Create(ComboViewModel vm)
        {
            // 1. VALIDATE TRƯỚC (Không cần Transaction ở đây)
            var comboData = vm.ComboModel;
            if (string.IsNullOrEmpty(comboData?.ComboName))
                return ServiceResult<ComboViewModel>.Failed("Tên combo không được để trống");

            if (vm.ComboServiceModel == null || !vm.ComboServiceModel.Any())
                return ServiceResult<ComboViewModel>.Failed("Combo phải có ít nhất một dịch vụ");

            var exists = await _unitOfWork.Repository<ComboModel>()
                .Query().AnyAsync(entity => entity.ComboName.ToLower() == comboData.ComboName.ToLower());

            if (exists)
                return ServiceResult<ComboViewModel>.Failed("Tên combo này đã tồn tại");

            // 2. BẮT ĐẦU TRANSACTION KHI DỮ LIỆU ĐÃ SẴN SÀNG
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var selectedServiceIds = vm.ComboServiceModel.Select(cs => cs.ServiceId).ToList();
                var actualServices = await _unitOfWork.Repository<ServiceModel>()
                    .Query()
                    .Where(s => selectedServiceIds.Contains(s.ServiceId))
                    .ToListAsync();

                if (actualServices.Count != selectedServiceIds.Count)
                {
                    // Trả về failed thì vẫn phải rollback vì đã lỡ Begin rồi
                    await _unitOfWork.RollbackTransactionAsync();
                    return ServiceResult<ComboViewModel>.Failed("Một số dịch vụ không tồn tại");
                }

                int totalDuration = actualServices.Sum(s => s.DurationInMinutes);
                decimal totalBasePrice = actualServices.Sum(s => s.Base_Price);

                var newCombo = new ComboModel
                {
                    ComboName = comboData.ComboName,
                    TotalDuration = totalDuration,
                    BasePrice = totalBasePrice,
                    PromotionPrice = comboData.PromotionPrice,
                    ComboServices = selectedServiceIds.Select(id => new ComboServiceModel
                    {
                        ServiceId = id
                    }).ToList()
                };

                await _unitOfWork.Repository<ComboModel>().AddAsync(newCombo);

                // Lưu và chốt
                await _unitOfWork.CommitTransactionAsync();

                vm.ComboModel = newCombo;
                return ServiceResult<ComboViewModel>.Success(vm);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult<ComboViewModel>.Failed($"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var combo = await _unitOfWork.Repository<ComboModel>()
                    .Query()
                    .Include(c => c.ComboServices)
                    .FirstOrDefaultAsync(c => c.ComboId == id);

                if (combo == null)
                    return ServiceResult<bool>.Failed("Không tìm thấy combo để xóa.");

                _unitOfWork.Repository<ComboModel>().Delete(combo);

                await _unitOfWork.CommitTransactionAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult<bool>.Failed($"Lỗi khi xóa: {ex.Message}");
            }
        }
    }
}