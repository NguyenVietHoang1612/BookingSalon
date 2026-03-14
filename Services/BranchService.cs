using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class BranchService : IBranchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public BranchService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<ServiceResult<IEnumerable<BranchModel>>> GetAllAsync()
        {
            try
            {
                var branches = await _unitOfWork.Repository<BranchModel>()
                    .Query()
                    .Include(b => b.Ward)
                        .ThenInclude(w => w.District)
                            .ThenInclude(d => d.Province)
                    .AsNoTracking()
                    .OrderBy(b => b.Branch_Name)
                    .ToListAsync();


                return ServiceResult<IEnumerable<BranchModel>>.Success(branches);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<BranchModel>>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BranchModel>> GetByIdAsync(int id)
        {
            try
            {
                var branch = await _unitOfWork.Repository<BranchModel>()
                    .Query()
                    .Include(b => b.Ward)
                        .ThenInclude(w => w.District)
                            .ThenInclude(d => d.Province)
                    .FirstOrDefaultAsync(b => b.BranchId == id);

                if (branch == null)
                    return ServiceResult<BranchModel>.Failed("Không tìm thấy chi nhánh");

                return ServiceResult<BranchModel>.Success(branch);
            }
            catch (Exception ex)
            {
                return ServiceResult<BranchModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BranchModel>> CreateAsync(BranchModel branch)
        {
            if (string.IsNullOrEmpty(branch.Branch_Name))
                return ServiceResult<BranchModel>.Failed("Tên không được để trống");

            try
            {
                var repo = _unitOfWork.Repository<BranchModel>();

                var exists = await repo.ExistsAsync(s => s.Branch_Name == branch.Branch_Name);
                if (exists)
                    return ServiceResult<BranchModel>.Failed("Tên chi nhánh đã tồn tại");

                var newBranch = new BranchModel
                {
                    Branch_Name = branch.Branch_Name,
                    Address = branch.Address,
                    WardId = branch.WardId, 
                    PhoneNumber = branch.PhoneNumber,
                    Status = branch.Status,
                    Is_Main_Branch = branch.Is_Main_Branch,
                    Branch_Image = branch.Branch_Image,
                    Created_At = DateTime.Now
                };

                if (branch.Branch_Image_Upload != null)
                {
                    newBranch.Branch_Image = await _fileService.UploadFileAsync(branch.Branch_Image_Upload, "branches");
                }

                await repo.AddAsync(newBranch);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<BranchModel>.Success(newBranch);
            }
            catch (Exception ex)
            {
                return ServiceResult<BranchModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<BranchModel>> UpdateAsync(int id, BranchModel branch)
        {
            try
            {
                var repo = _unitOfWork.Repository<BranchModel>();
                var existingBranch = await repo.GetByIdAsync(id);

                if (existingBranch == null)
                    return ServiceResult<BranchModel>.Failed("Không tìm thấy Chi nhánh");

                string oldImageName = existingBranch.Branch_Image;

                existingBranch.Branch_Name = branch.Branch_Name;
                existingBranch.Address = branch.Address;
                existingBranch.WardId = branch.WardId;
                existingBranch.PhoneNumber = branch.PhoneNumber;
                existingBranch.Status = branch.Status;
                existingBranch.Is_Main_Branch = branch.Is_Main_Branch;
                existingBranch.Update_At = DateTime.Now;

                if (branch.Branch_Image_Upload != null)
                {
                    if (!string.IsNullOrEmpty(oldImageName))
                    {
                        await _fileService.DeleteFileAsync(oldImageName, "branches");
                    }

                    existingBranch.Branch_Image = await _fileService.UploadFileAsync(branch.Branch_Image_Upload, "branches");
                }

                repo.Update(existingBranch);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<BranchModel>.Success(existingBranch);
            }
            catch (Exception ex)
            {
                return ServiceResult<BranchModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<PaginatedList<BranchModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<BranchModel>()
                .Query();
                

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(s => s.Branch_Name.ToLower().Contains(searchTerm)
                                     || s.PhoneNumber.ToLower().Contains(searchTerm));
            }

            query = query.Include(b => b.Ward)
                    .ThenInclude(w => w.District)
                        .ThenInclude(d => d.Province);

            return await PaginatedList<BranchModel>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<ServiceResult<BranchModel>> DeleteAsync(int id)
        {
            try
            {
                var branch = await _unitOfWork.Repository<BranchModel>().GetByIdAsync(id);
                if (branch == null) return ServiceResult<BranchModel>.Failed("Không tìm thấy branch");

                string imageName = branch.Branch_Image;

                _unitOfWork.Repository<BranchModel>().Delete(branch);
                await _unitOfWork.SaveChangesAsync();

                if (branch.Branch_Image != null)
                {
                    await _fileService.DeleteFileAsync(imageName, "branches");
                }

                return ServiceResult<BranchModel>.Success(branch);
            }
            catch (Exception ex)
            {
                return ServiceResult<BranchModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<WardModel> GetWardById(BranchModel branch)
        {
            var ward = await _unitOfWork.Repository<WardModel>().Query()
                    .Include(w => w.District)
                    .FirstOrDefaultAsync(w => w.Id == branch.WardId);

            return ward;
        }

        public async Task<IEnumerable<BranchModel>> GetAllBranchActiveAsync()
        {
            var branchesActive = await _unitOfWork.Repository<BranchModel>()
                .Query()
                .Where(b => b.Status == true)
                .ToListAsync();
            return branchesActive;
        }
    }
}