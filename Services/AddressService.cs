using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WardModel>> GetAllWardByDistrictIdAsync(int districtId)
        {
            var wards = await _unitOfWork.Repository<WardModel>()
                 .Query().Where(w => w.DistrictId == districtId).ToListAsync();

            return wards;
        }

        public async Task<IEnumerable<DistrictModel>> GetAllDistrictByProvinceIdAsync(int provinceId)
        {
            var districts = await _unitOfWork.Repository<DistrictModel>()
                .Query().Where(d => d.ProvinceId == provinceId).ToListAsync();

            return districts;
        }

        public async Task<IEnumerable<ProvinceModel>> GetAllProvinceAsync()
        {
            var provinces = await _unitOfWork.Repository<ProvinceModel>().GetAllAsync();
            return provinces;
        }

        public async Task<WardModel> GetAddressByWardIdAsync(int wardId)
        {
            var wards = await _unitOfWork.Repository<WardModel>().Query()
                .Include(w => w.District)
                    .ThenInclude(d => d.Province)
                .FirstOrDefaultAsync(w => w.Id == wardId);

            return wards;
        }
    }
}
