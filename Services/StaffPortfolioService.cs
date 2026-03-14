using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class StaffPortfolioService : IStaffPortfolioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public StaffPortfolioService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<ServiceResult<IEnumerable<StaffPortfolioModel>>> GetAllAsync()
        {
            try
            {
                var portfolios = await _unitOfWork.Repository<StaffPortfolioModel>()
                    .Query()
                    .Include(p => p.StaffProfile)
                    .OrderByDescending(p=>p.ImageId)
                    .ToListAsync();

                return ServiceResult<IEnumerable<StaffPortfolioModel>>.Success(portfolios);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<StaffPortfolioModel>>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<StaffPortfolioModel>> GetByIdAsync(int id)
        {
            try
            {
                var portfolio = await _unitOfWork.Repository<StaffPortfolioModel>()
                    .Query()
                    .Include(p => p.StaffProfile)
                    .FirstOrDefaultAsync(p => p.ImageId == id);

                if (portfolio == null)
                    return ServiceResult<StaffPortfolioModel>.Failed("Không tìm thấy ảnh portfolio");

                return ServiceResult<StaffPortfolioModel>.Success(portfolio);
            }
            catch (Exception ex)
            {
                return ServiceResult<StaffPortfolioModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<PaginatedList<StaffPortfolioModel>> GetStaffPagedListAsync(string staffId, int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<StaffPortfolioModel>()
                .Query()
                .Where(p => p.StaffProfileId == staffId)
                .OrderByDescending(p => p.Create_At)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(p => p.Title.ToLower().Contains(searchTerm));
            }

            return await PaginatedList<StaffPortfolioModel>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<ServiceResult<StaffPortfolioModel>> CreateAsync(StaffPortfolioModel model)
        {
            try
            {
                var repo = _unitOfWork.Repository<StaffPortfolioModel>();

                var newItem = new StaffPortfolioModel
                {
                    StaffProfileId = model.StaffProfileId,
                    Category = model.Category,
                    Title = model.Title,
                    IsFeatured = false,
                    ImageUrl = "",
                    Create_At = DateTime.Now,
                    Update_At = DateTime.Now
                };

                if (model.Staff_Portfolio_Image_Upload != null)
                {
                    newItem.ImageUrl = await _fileService.UploadFileAsync(model.Staff_Portfolio_Image_Upload, "portfolios");
                }
                else
                {
                    return ServiceResult<StaffPortfolioModel>.Failed("Vui lòng tải lên hình ảnh");
                }

                await repo.AddAsync(newItem);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult<StaffPortfolioModel>.Success(newItem);
            }
            catch (Exception ex)
            {
                return ServiceResult<StaffPortfolioModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<StaffPortfolioModel>> UpdateAsync(int id, StaffPortfolioModel model)
        {
            try
            {
                var repo = _unitOfWork.Repository<StaffPortfolioModel>();
                var existingItem = await repo.GetByIdAsync(id);

                if (existingItem == null) return ServiceResult<StaffPortfolioModel>.Failed("Không tìm thấy");

                existingItem.Category = model.Category;
                existingItem.Title = model.Title;
                existingItem.Update_At = DateTime.Now;

                if (model.Staff_Portfolio_Image_Upload != null)
                {
                    if (!string.IsNullOrEmpty(existingItem.ImageUrl))
                        await _fileService.DeleteFileAsync(existingItem.ImageUrl, "portfolios");

                    existingItem.ImageUrl = await _fileService.UploadFileAsync(model.Staff_Portfolio_Image_Upload, "portfolios");
                }

                repo.Update(existingItem);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult<StaffPortfolioModel>.Success(existingItem);
            }
            catch (Exception ex) { return ServiceResult<StaffPortfolioModel>.Failed(ex.Message); }
        }

        public async Task<ServiceResult<StaffPortfolioModel>> UpdateIsFeaturedAsync(int id, bool isFeatured)
        {
            try
            {
                var repo = _unitOfWork.Repository<StaffPortfolioModel>();
                var existingItem = await repo.GetByIdAsync(id);

                if (existingItem == null)
                    return ServiceResult<StaffPortfolioModel>.Failed("Không tìm thấy Portfolio");

                existingItem.IsFeatured = isFeatured;
                existingItem.Update_At = DateTime.Now;

                repo.Update(existingItem);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<StaffPortfolioModel>.Success(existingItem);
            }
            catch (Exception ex)
            {
                return ServiceResult<StaffPortfolioModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<PaginatedList<StaffPortfolioModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<StaffPortfolioModel>()
                .Query()
                .Include(p => p.StaffProfile)
                    .ThenInclude(s => s.Staff)
                 .OrderByDescending(p=>p.Create_At)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(p => p.Title.ToLower().Contains(searchTerm) ||
                                         p.StaffProfile.Staff.FullName.ToLower().Contains(searchTerm));
            }

            return await PaginatedList<StaffPortfolioModel>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<ServiceResult<StaffPortfolioModel>> DeleteAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<StaffPortfolioModel>();
                var item = await repo.GetByIdAsync(id);
                if (item == null) return ServiceResult<StaffPortfolioModel>.Failed("Không tìm thấy portfolio");

                string imageName = item.ImageUrl;

                repo.Delete(item);
                await _unitOfWork.SaveChangesAsync();

                if (!string.IsNullOrEmpty(imageName))
                {
                    await _fileService.DeleteFileAsync(imageName, "portfolios");
                }

                return ServiceResult<StaffPortfolioModel>.Success(item);
            }
            catch (Exception ex)
            {
                return ServiceResult<StaffPortfolioModel>.Failed($"Lỗi: {ex.Message}");
            }
        }
    }
}