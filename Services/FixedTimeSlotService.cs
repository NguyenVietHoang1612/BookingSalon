using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;

namespace BookingSalon.Services
{
    public class FixedTimeSlotService : IFixedTimeSlotService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FixedTimeSlotService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<FixedTimeSlotModel>> CreateAsync(FixedTimeSlotModel fixedTime)
        {
            if (string.IsNullOrEmpty(fixedTime.TimeLabel.ToString()))
            {
                return ServiceResult<FixedTimeSlotModel>.Failed("Khung thời gian không được để trống");
            }

            try
            {
                var repo = _unitOfWork.Repository<FixedTimeSlotModel>();

                var exists = await repo.ExistsAsync(s => s.TimeLabel == fixedTime.TimeLabel);

                if(exists)
                {
                    return ServiceResult<FixedTimeSlotModel>.Failed("Khung thời gian đã tồn tại");
                }

                fixedTime.Create_At = DateTime.Now;

                await repo.AddAsync(fixedTime);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<FixedTimeSlotModel>.Success(fixedTime);

            }
            catch (Exception ex)
            {
                return ServiceResult<FixedTimeSlotModel>.Failed($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<FixedTimeSlotModel>> DeleteAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<FixedTimeSlotModel>();
                var existingFixedTime = await repo.GetByIdAsync(id);

                if (existingFixedTime == null)
                    return ServiceResult<FixedTimeSlotModel>.Failed("Khung thời gian không tìm thấy");

                repo.Delete(existingFixedTime);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult<FixedTimeSlotModel>.Success(existingFixedTime);
            }
            catch (Exception ex)
            {
                return ServiceResult<FixedTimeSlotModel>.Failed($"Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<FixedTimeSlotModel>> GetAllListAsync()
        {
            var datas = await _unitOfWork.Repository<FixedTimeSlotModel>().GetAllAsync();
            return datas;

        }

        public async Task<ServiceResult<FixedTimeSlotModel>> GetByIdAsync(int id)
        {
            try
            {
                var data = await _unitOfWork.Repository<FixedTimeSlotModel>().GetByIdAsync(id);

                if (data == null)
                {
                    return await Task.FromResult(ServiceResult<FixedTimeSlotModel>.Failed("FixedTimeSlot not found"));
                }

                return await Task.FromResult(ServiceResult<FixedTimeSlotModel>.Success(data));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(ServiceResult<FixedTimeSlotModel>.Failed($"Error: {ex.Message}"));
            }
        }

        public async Task<ServiceResult<FixedTimeSlotModel>> UpdateAsync(int id, FixedTimeSlotModel fixedTime)
        {
            try
            {
                var repo = _unitOfWork.Repository<FixedTimeSlotModel>();
                var existingFixedTime = await repo.GetByIdAsync(id);

                if (existingFixedTime == null)
                    return ServiceResult<FixedTimeSlotModel>.Failed("Khung thời gian không được tìm thấy");

                existingFixedTime.TimeLabel = fixedTime.TimeLabel;
                existingFixedTime.Update_At = DateTime.Now;

                repo.Update(existingFixedTime);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<FixedTimeSlotModel>.Success(existingFixedTime);


            }
            catch (Exception ex)
            {
                return ServiceResult<FixedTimeSlotModel>.Failed($"Error: {ex.Message}");
            }
        }

        public async Task<PaginatedList<FixedTimeSlotModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<FixedTimeSlotModel>().Query();

            if(!string.IsNullOrEmpty(searchTerm)) {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(r => r.TimeLabel.ToString().ToLower().Contains(searchTerm));
            }

            query = query.OrderBy(x => x.TimeLabel);

            return await PaginatedList<FixedTimeSlotModel>.CreateAsync(query, pageNumber, pageSize);
        }
    }
}
