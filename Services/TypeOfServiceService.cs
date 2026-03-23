using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class TypeOfServiceService : ITypeOfServiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TypeOfServiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<TypeOfServiceModel>> CreateAsync(TypeOfServiceModel typeOfService)
        {
            if (string.IsNullOrEmpty(typeOfService.Type_Service_Name))
            {
                return ServiceResult<TypeOfServiceModel>.Failed("Loại dịch vụ không được để trống");
            }

            try
            {
                var repo = _unitOfWork.Repository<TypeOfServiceModel>();

                var exists = await repo.ExistsAsync(s => s.Type_Service_Name == typeOfService.Type_Service_Name);

                if (exists)
                {
                    return ServiceResult<TypeOfServiceModel>.Failed("Loại dịch vụ đã tồn tại");
                }

                typeOfService.Created_At = DateTime.Now;

                await repo.AddAsync(typeOfService);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<TypeOfServiceModel>.Success(typeOfService);

            }
            catch (Exception ex)
            {
                return ServiceResult<TypeOfServiceModel>.Failed($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<TypeOfServiceModel>> DeleteAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<TypeOfServiceModel>();
                var existingTypeOfService = await repo.GetByIdAsync(id);

                if (existingTypeOfService == null)
                    return ServiceResult<TypeOfServiceModel>.Failed("Loại dịch vụ không tìm thấy");

                repo.Delete(existingTypeOfService);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult<TypeOfServiceModel>.Success(existingTypeOfService);
            }
            catch (Exception ex)
            {
                return ServiceResult<TypeOfServiceModel>.Failed($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<TypeOfServiceModel>> GetByIdAsync(int id)
        {
            try
            {
                var data = await _unitOfWork.Repository<TypeOfServiceModel>().GetByIdAsync(id);

                if (data == null)
                {
                    return await Task.FromResult(ServiceResult<TypeOfServiceModel>.Failed("Loại dịch vụ not found"));
                }

                return await Task.FromResult(ServiceResult<TypeOfServiceModel>.Success(data));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(ServiceResult<TypeOfServiceModel>.Failed($"Error: {ex.Message}"));
            }
        }

        public async Task<ServiceResult<TypeOfServiceModel>> UpdateAsync(int id, TypeOfServiceModel fixedTime)
        {
            try
            {
                var repo = _unitOfWork.Repository<TypeOfServiceModel>();
                var existingTypeOfService = await repo.GetByIdAsync(id);

                if (existingTypeOfService == null)
                    return ServiceResult<TypeOfServiceModel>.Failed("Loại dịch vụ không được tìm thấy");

                existingTypeOfService.Type_Service_Name = fixedTime.Type_Service_Name;
                existingTypeOfService.AppliedStaffRoleId = fixedTime.AppliedStaffRoleId;
                existingTypeOfService.Update_At = DateTime.Now;

                repo.Update(existingTypeOfService);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<TypeOfServiceModel>.Success(existingTypeOfService);


            }
            catch (Exception ex)
            {
                return ServiceResult<TypeOfServiceModel>.Failed($"Error: {ex.Message}");
            }
        }

        public async Task<PaginatedList<TypeOfServiceModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<TypeOfServiceModel>().Query();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(r => r.Type_Service_Name.ToLower().Contains(searchTerm));
            }

            return await PaginatedList<TypeOfServiceModel>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<IEnumerable<TypeOfServiceModel>> GetAllTypeServiceAsync()
        {
            var datas = await _unitOfWork.Repository<TypeOfServiceModel>().Query()
                .Include(t => t.Services).ToListAsync();

            return datas;
        }
    }
}
