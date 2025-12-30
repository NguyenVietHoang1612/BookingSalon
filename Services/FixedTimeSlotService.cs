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

        public async Task<ServiceResult<FixedTimeSlot>> CreateAsync(FixedTimeSlot fixedTime)
        {
            if (string.IsNullOrEmpty(fixedTime.TimeLabel.ToString()))
            {
                return ServiceResult<FixedTimeSlot>.Failed("Khung thời gian không được để trống");
            }

            try
            {
                var repo = _unitOfWork.Repository<FixedTimeSlot>();

                var exists = await repo.ExistsAsync(s => s.TimeLabel == fixedTime.TimeLabel);

                if(exists)
                {
                    return ServiceResult<FixedTimeSlot>.Failed("Khung thời gian đã tồn tại");
                }

                fixedTime.Create_At = DateTime.Now;

                await repo.AddAsync(fixedTime);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<FixedTimeSlot>.Success(fixedTime);

            }
            catch (Exception ex)
            {
                return ServiceResult<FixedTimeSlot>.Failed($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<FixedTimeSlot>> DeleteAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<FixedTimeSlot>();
                var existingFixedTime = await repo.GetByIdAsync(id);

                if (existingFixedTime == null)
                    return ServiceResult<FixedTimeSlot>.Failed("Khung thời gian không tìm thấy");

                repo.Delete(existingFixedTime);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult<FixedTimeSlot>.Success(existingFixedTime);
            }
            catch (Exception ex)
            {
                return ServiceResult<FixedTimeSlot>.Failed($"Error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<FixedTimeSlot>> GetAllListAsync()
        {
            var datas = await _unitOfWork.Repository<FixedTimeSlot>().GetAllAsync();
            return datas;

        }

        public async Task<ServiceResult<FixedTimeSlot>> GetByIdAsync(int id)
        {
            try
            {
                var data = await _unitOfWork.Repository<FixedTimeSlot>().GetByIdAsync(id);

                if (data == null)
                {
                    return await Task.FromResult(ServiceResult<FixedTimeSlot>.Failed("FixedTimeSlot not found"));
                }

                return await Task.FromResult(ServiceResult<FixedTimeSlot>.Success(data));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(ServiceResult<FixedTimeSlot>.Failed($"Error: {ex.Message}"));
            }
        }

        public async Task<ServiceResult<FixedTimeSlot>> UpdateAsync(int id, FixedTimeSlot fixedTime)
        {
            try
            {
                var repo = _unitOfWork.Repository<FixedTimeSlot>();
                var existingFixedTime = await repo.GetByIdAsync(id);

                if (existingFixedTime == null)
                    return ServiceResult<FixedTimeSlot>.Failed("Khung thời gian không được tìm thấy");

                existingFixedTime.TimeLabel = fixedTime.TimeLabel;
                existingFixedTime.Sort_Order = fixedTime.Sort_Order;
                existingFixedTime.Update_At = DateTime.Now;

                repo.Update(existingFixedTime);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<FixedTimeSlot>.Success(existingFixedTime);


            }
            catch (Exception ex)
            {
                return ServiceResult<FixedTimeSlot>.Failed($"Error: {ex.Message}");
            }
        }
    }
}
