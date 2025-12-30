using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;

namespace BookingSalon.Services
{
    public class TypeOfServiceService : ITypeOfServiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TypeOfServiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<TypeOfService>> CreateAsync(TypeOfService typeOfService)
        {
            if (string.IsNullOrEmpty(typeOfService.Type_Service_Name))
            {
                return ServiceResult<TypeOfService>.Failed("Loại dịch vụ không được để trống");
            }

            try
            {
                var repo = _unitOfWork.Repository<TypeOfService>();

                var exists = await repo.ExistsAsync(s => s.Type_Service_Name == typeOfService.Type_Service_Name);

                if (exists)
                {
                    return ServiceResult<TypeOfService>.Failed("Loại dịch vụ đã tồn tại");
                }

                typeOfService.Created_At = DateTime.Now;

                await repo.AddAsync(typeOfService);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<TypeOfService>.Success(typeOfService);

            }
            catch (Exception ex)
            {
                return ServiceResult<TypeOfService>.Failed($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<TypeOfService>> DeleteAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<TypeOfService>();
                var existingTypeOfService = await repo.GetByIdAsync(id);

                if (existingTypeOfService == null)
                    return ServiceResult<TypeOfService>.Failed("Loại dịch vụ không tìm thấy");

                repo.Delete(existingTypeOfService);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult<TypeOfService>.Success(existingTypeOfService);
            }
            catch (Exception ex)
            {
                return ServiceResult<TypeOfService>.Failed($"Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<TypeOfService>> GetAllServiceAsync()
        {
            var datas = await _unitOfWork.Repository<TypeOfService>().GetAllAsync();
            return datas;

        }

        public async Task<ServiceResult<TypeOfService>> GetByIdAsync(int id)
        {
            try
            {
                var data = await _unitOfWork.Repository<TypeOfService>().GetByIdAsync(id);

                if (data == null)
                {
                    return await Task.FromResult(ServiceResult<TypeOfService>.Failed("Loại dịch vụ not found"));
                }

                return await Task.FromResult(ServiceResult<TypeOfService>.Success(data));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(ServiceResult<TypeOfService>.Failed($"Error: {ex.Message}"));
            }
        }

        public async Task<ServiceResult<TypeOfService>> UpdateAsync(int id, TypeOfService fixedTime)
        {
            try
            {
                var repo = _unitOfWork.Repository<TypeOfService>();
                var existingTypeOfService = await repo.GetByIdAsync(id);

                if (existingTypeOfService == null)
                    return ServiceResult<TypeOfService>.Failed("Loại dịch vụ không được tìm thấy");

                existingTypeOfService.Type_Service_Name = fixedTime.Type_Service_Name;
                existingTypeOfService.Update_At = DateTime.Now;

                repo.Update(existingTypeOfService);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<TypeOfService>.Success(existingTypeOfService);


            }
            catch (Exception ex)
            {
                return ServiceResult<TypeOfService>.Failed($"Error: {ex.Message}");
            }
        }
    }
}
