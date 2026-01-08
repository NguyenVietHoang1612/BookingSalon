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

        public async Task<IEnumerable<Service>> GetAllServiceAsync()
        {       
            return await _unitOfWork.Repository<Service>()
                .Query()
                .Include(s => s.TypeOfService)
                .ToListAsync();
        }

        public async Task<IEnumerable<TypeOfService>> GetAllTypeOfServiceAsync()
        {
            var typeOfServices = await _unitOfWork.Repository<TypeOfService>().GetAllAsync();
            return typeOfServices;
        }

        public async Task<ServiceResult<Service>> GetByIdAsync(int id)
        {
            try
            {
                var branch = await _unitOfWork.Repository<Service>().GetByIdAsync(id);

                if (branch == null)
                    return ServiceResult<Service>.Failed("Không tìm thấy dịch vụ");

                return ServiceResult<Service>.Success(branch);
            }
            catch (Exception ex)
            {
                return ServiceResult<Service>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Service>> CreateAsync(Service service)
        {
            if (string.IsNullOrEmpty(service.Service_Name))
                return ServiceResult<Service>.Failed("Tên dịch vụ không được để trống");

            try
            {
                var repo = _unitOfWork.Repository<Service>();

                var exists = await repo.ExistsAsync(s => s.Service_Name == service.Service_Name);
                if (exists)
                    return ServiceResult<Service>.Failed("Tên dịch vụ đã tồn tại");

                var newService = new Service
                {
                    Service_Name = service.Service_Name,
                    Type_Service_Id = service.Type_Service_Id,
                    Price = service.Price,
                    Status = service.Status,
                    DurationInMinutes = service.DurationInMinutes,
                    description = service.description,
                    Create_At = DateTime.Now
                };

                if (service.Service_Image_Upload != null)
                {
                    newService.ImageName = await _fileService.UploadFileAsync(service.Service_Image_Upload, "services");
                }

                await repo.AddAsync(newService);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<Service>.Success(newService);     
            }
            catch (Exception ex)
            {
                return ServiceResult<Service>.Failed($"Lỗi: {ex.Message}");
            }
        }
        public async Task<ServiceResult<Service>> UpdateAsync(int id, Service service)
        {
            try
            {
                var repo = _unitOfWork.Repository<Service>();
                var existingService = await repo.GetByIdAsync(id);

                if (existingService == null)
                    return ServiceResult<Service>.Failed("Không tìm thấy dịch vụ");

                existingService.Service_Name = service.Service_Name;
                existingService.Type_Service_Id = service.Type_Service_Id;
                existingService.Price = service.Price;
                existingService.DurationInMinutes = service.DurationInMinutes;
                existingService.description = service.description;
                existingService.Status = service.Status;
                existingService.Update_At = DateTime.Now;

                if (service.Service_Image_Upload != null)
                {
                    if (service.ImageName != null)
                    {
                        await _fileService.DeleteFileAsync(existingService.ImageName, "services");
                    }

                    existingService.ImageName = await _fileService.UploadFileAsync(service.Service_Image_Upload, "services");
                }

                repo.Update(existingService);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<Service>.Success(existingService);    
            }
            catch (Exception ex)
            {
                return ServiceResult<Service>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Service>> DeleteAsync(int id)
        {
            try
            {
                var service = await _unitOfWork.Repository<Service>().GetByIdAsync(id);

                if (service == null)
                    return ServiceResult<Service>.Failed("Không tìm thấy dịch vụ");

                if (service.ImageName != null)
                {
                    await _fileService.DeleteFileAsync(service.ImageName, "services");
                }

                _unitOfWork.Repository<Service>().Delete(service);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<Service>.Success(service);    
            }
            catch (Exception ex)
            {
                return ServiceResult<Service>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<PaginatedList<Service>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<Service>()
                .Query();
                

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(s => s.Service_Name.ToLower().Contains(searchTerm));
            }

            query = query.Include(s => s.TypeOfService).OrderBy(s => s.ServiceId);

            return await PaginatedList<Service>.CreateAsync(query, pageNumber, pageSize);
        }
    }
}
