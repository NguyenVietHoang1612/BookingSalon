using BookingSalon.Models.Entities;

namespace BookingSalon.Services.Interface
{
    public interface IAddressService
    {
        Task<IEnumerable<WardModel>> GetAllWardByDistrictIdAsync(int districtId);
        Task<IEnumerable<DistrictModel>> GetAllDistrictByProvinceIdAsync(int provinceId);
        Task<IEnumerable<ProvinceModel>> GetAllProvinceAsync();
        Task<WardModel> GetAddressByWardIdAsync(int wardId);
    }
}
