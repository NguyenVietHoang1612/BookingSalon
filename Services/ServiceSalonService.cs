using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Areas.Admin.Models;
using BookingSalon.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class ServicesSalonService : IServicesSalonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public ServicesSalonService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<IEnumerable<ServiceModel>> GetAllServiceAsync()
        {       
            return await _unitOfWork.Repository<ServiceModel>()
                .Query()
                .Include(s => s.TypeOfService)
                .ToListAsync();
        }

        public async Task<IEnumerable<TypeOfServiceModel>> GetAllTypeOfServiceAsync()
        {
            var typeOfServices = await _unitOfWork.Repository<TypeOfServiceModel>().GetAllAsync();
            return typeOfServices;
        }

        public async Task<ServiceResult<ServiceModel>> GetByIdAsync(int id)
        {
            try
            {
                var service = _unitOfWork.Repository<ServiceModel>().Query()
                    .Include(s => s.TypeOfService)
                    .Where(s => s.ServiceId == id)
                    .FirstOrDefault();


                if (service == null)
                    return ServiceResult<ServiceModel>.Failed("Không tìm thấy dịch vụ");

                return ServiceResult<ServiceModel>.Success(service);
            }
            catch (Exception ex)
            {
                return ServiceResult<ServiceModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ServiceModel>> CreateAsync(ServiceModel service)
        {
            if (string.IsNullOrEmpty(service.Service_Name))
                return ServiceResult<ServiceModel>.Failed("Tên dịch vụ không được để trống");

            try
            {
                var repo = _unitOfWork.Repository<ServiceModel>();

                var exists = await repo.ExistsAsync(s => s.Service_Name == service.Service_Name);
                if (exists)
                    return ServiceResult<ServiceModel>.Failed("Tên dịch vụ đã tồn tại");

                var newService = new ServiceModel
                {
                    Service_Name = service.Service_Name,
                    Type_Service_Id = service.Type_Service_Id,
                    Base_Price = service.Base_Price,
                    Status = service.Status,
                    DurationInMinutes = service.DurationInMinutes,
                    Promotion_Price = service.Promotion_Price ?? 0,
                    Promotion_Start = service.Promotion_Start,
                    Promotion_End = service.Promotion_End,
                    description = service.description,
                    Create_At = DateTime.Now
                };

                if (service.Service_Image_Upload != null)
                {
                    newService.ImageName = await _fileService.UploadFileAsync(service.Service_Image_Upload, "services");
                }

                await repo.AddAsync(newService);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<ServiceModel>.Success(newService);     
            }
            catch (Exception ex)
            {
                return ServiceResult<ServiceModel>.Failed($"Lỗi: {ex.Message}");
            }
        }
        public async Task<ServiceResult<ServiceModel>> UpdateAsync(int id, ServiceModel service)
        {
            try
            {
                var repo = _unitOfWork.Repository<ServiceModel>();
                var existingService = await repo.GetByIdAsync(id);

                if (existingService == null)
                    return ServiceResult<ServiceModel>.Failed("Không tìm thấy dịch vụ");

                string oldImageName = existingService.ImageName;

                existingService.Service_Name = service.Service_Name;
                existingService.Type_Service_Id = service.Type_Service_Id;
                existingService.Base_Price = service.Base_Price;
                existingService.Promotion_Price = service.Promotion_Price;
                existingService.Promotion_Start = service.Promotion_Start;
                existingService.Promotion_End = service.Promotion_End;
                existingService.DurationInMinutes = service.DurationInMinutes;
                existingService.description = service.description;
                existingService.Status = service.Status;
                existingService.Update_At = DateTime.Now;

                if (service.Service_Image_Upload != null)
                {
                    if (!string.IsNullOrEmpty(oldImageName))
                    {
                        await _fileService.DeleteFileAsync(oldImageName, "services");
                    }

                    existingService.ImageName = await _fileService.UploadFileAsync(service.Service_Image_Upload, "services");
                }

                repo.Update(existingService);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<ServiceModel>.Success(existingService);    
            }
            catch (Exception ex)
            {
                return ServiceResult<ServiceModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ServiceModel>> DeleteAsync(int id)
        {
            try
            {
                var service = await _unitOfWork.Repository<ServiceModel>().GetByIdAsync(id);

                if (service == null)
                    return ServiceResult<ServiceModel>.Failed("Không tìm thấy dịch vụ");

                string imageName = service.ImageName;
               

                _unitOfWork.Repository<ServiceModel>().Delete(service);
                await _unitOfWork.SaveChangesAsync();

                if (service.ImageName != null)
                {
                    await _fileService.DeleteFileAsync(imageName, "services");
                }

                return ServiceResult<ServiceModel>.Success(service);    
            }
            catch (Exception ex)
            {
                return ServiceResult<ServiceModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<PaginatedList<ServiceModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<ServiceModel>()
                .Query();
                

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(s => s.Service_Name.ToLower().Contains(searchTerm));
            }

            query = query.Include(s => s.TypeOfService).OrderBy(s => s.ServiceId);

            return await PaginatedList<ServiceModel>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<IEnumerable<ServiceModel>> GetAllServiceActiveAsync()
        {
            var servicesActive = await _unitOfWork.Repository<ServiceModel>()
                .Query()
                .Where(s => s.Status == true)
                .Include(s => s.TypeOfService)
                .ToListAsync();

            return servicesActive;
        }
    }
}
