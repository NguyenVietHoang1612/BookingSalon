using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using System.Numerics;

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

        public async Task<ServiceResult<IEnumerable<Branch>>> GetAllAsync()
        {
            try
            {
                var branches = await _unitOfWork.Repository<Branch>().GetAllAsync();
                return ServiceResult<IEnumerable<Branch>>.Success(branches);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Branch>>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Branch>> GetByIdAsync(int id)
        {
            try
            {
                var branch = await _unitOfWork.Repository<Branch>().GetByIdAsync(id);

                if (branch == null)
                    return ServiceResult<Branch>.Failed("Không tìm thấy branch");

                return ServiceResult<Branch>.Success(branch);
            }
            catch (Exception ex)
            {
                return ServiceResult<Branch>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Branch>> CreateAsync(Branch branch)
        {
            if (string.IsNullOrEmpty(branch.Branch_Name))
                return ServiceResult<Branch>.Failed("Tên không được để trống");

            try
            {
                var repo = _unitOfWork.Repository<Branch>();

                var exists = await repo.ExistsAsync(s => s.Branch_Name == branch.Branch_Name);
                if (exists)
                    return ServiceResult<Branch>.Failed("Tên đã tồn tại");

                var newBranch = new Branch
                {
                    Branch_Name = branch.Branch_Name,
                    Address = branch.Address,
                    Phone = branch.Phone,
                    Status = branch.Status,
                    Created_At = DateTime.Now
                };

                await repo.AddAsync(newBranch);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<Branch>.Success(newBranch);
            }
            catch (Exception ex)
            {
                return ServiceResult<Branch>.Failed($"Lỗi: {ex.Message}");
            }
        }
        public async Task<ServiceResult<Branch>> UpdateAsync(int id, Branch branch)
        {
            try
            {
                var repo = _unitOfWork.Repository<Branch>();
                var existingBranch = await repo.GetByIdAsync(id);

                if (existingBranch == null)
                    return ServiceResult<Branch>.Failed("Không tìm thấy Chi nhánh");

                existingBranch.Branch_Name = branch.Branch_Name;
                existingBranch.Address = branch.Address;
                existingBranch.Phone = branch.Phone;
                existingBranch.Status = branch.Status;
                existingBranch.Update_At = DateTime.Now;

                repo.Update(existingBranch);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<Branch>.Success(existingBranch);
            }
            catch (Exception ex)
            {
                return ServiceResult<Branch>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Branch>> DeleteAsync(int id)
        {
            try
            {
                var branch = await _unitOfWork.Repository<Branch>().GetByIdAsync(id);

                if (branch == null)
                    return ServiceResult<Branch>.Failed("Không tìm thấy branch");

                _unitOfWork.Repository<Branch>().Delete(branch);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<Branch>.Success(branch);
            }
            catch (Exception ex)
            {
                return ServiceResult<Branch>.Failed($"Lỗi: {ex.Message}");
            }
        }

    }
}
