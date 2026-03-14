using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Twilio.Rest;


namespace BookingSalon.Services
{
    public class StaffProfileService : IStaffProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly UserManager<UsersModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public StaffProfileService(IUnitOfWork context, IFileService fileService, UserManager<UsersModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = context;
            _fileService = fileService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ServiceResult<StaffCombinedCreateVM>> CreateAsync(StaffCombinedCreateVM model)
        {
            try
            {
                //var emailParts = model.User.Email.Split('@');
                //string prefix = emailParts[0];
                //string domain = emailParts.Count() > 1 ? emailParts[1] : "StayHere.com";
                model.User.UserName = model.User.Email;
                model.User.Create_At = DateTime.Now;
                model.User.Status = true;

                if (model.User.Avatar_Image_Upload != null)
                    model.User.Avatar_Name = await _fileService.UploadFileAsync(model.User.Avatar_Image_Upload, "users");

                var result = await _userManager.CreateAsync(model.User, model.Password);

                if (!result.Succeeded) return ServiceResult<StaffCombinedCreateVM>.Failed("Lỗi tạo tài khoản nhân viên");

                var role = await _roleManager.FindByIdAsync(model.User.RoleId);
                await _userManager.AddToRoleAsync(model.User, role.Name);

                


                var profile = model.ProfileInfo;
                profile.StaffId = model.User.Id; 
                profile.Create_At = DateTime.Now;
                profile.Update_At = DateTime.Now;

                await _unitOfWork.Repository<StaffProfileModel>().AddAsync(profile);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<StaffCombinedCreateVM>.Success(model);
            }
            catch (Exception ex)
            {
                return ServiceResult<StaffCombinedCreateVM>.Failed(ex.Message);
            }
        }

        public async Task<ServiceResult<StaffCombinedUpdateVM>> UpdateAsync(string id, StaffCombinedUpdateVM model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                var profile = await _unitOfWork.Repository<StaffProfileModel>()
                    .Query()
                    .FirstOrDefaultAsync(x => x.StaffId == id);

                if (user == null || profile == null)
                    return ServiceResult<StaffCombinedUpdateVM>.Failed("Không tìm thấy thông tin nhân viên");

                if (user.Email != model.User.Email)
                {
                    var existingUser = await _userManager.FindByEmailAsync(model.User.Email);
                    if (existingUser != null) return ServiceResult<StaffCombinedUpdateVM>.Failed("Email này đã được sử dụng bởi người khác");
                }

                user.FullName = model.User.FullName;
                user.PhoneNumber = model.User.PhoneNumber;
                user.WardId = model.User.WardId;
                user.Address = model.User.Address;
                user.Status = model.User.Status;
                user.RoleId = model.User.RoleId;
                user.Update_At = DateTime.Now;

                if (user.Email != model.User.Email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(user, model.User.Email);
                    var setUserNameResult = await _userManager.SetUserNameAsync(user, model.User.Email);
                }

                if (model.User.Avatar_Image_Upload != null)
                {
                    if (!string.IsNullOrEmpty(user.Avatar_Name))
                        await _fileService.DeleteFileAsync(user.Avatar_Name, "users");

                    user.Avatar_Name = await _fileService.UploadFileAsync(model.User.Avatar_Image_Upload, "users");
                }

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var removePassResult = await _userManager.RemovePasswordAsync(user);
                    if (removePassResult.Succeeded)
                        await _userManager.AddPasswordAsync(user, model.Password);
                }

                var userResult = await _userManager.UpdateAsync(user);
                if (!userResult.Succeeded)
                {
                    return ServiceResult<StaffCombinedUpdateVM>.Failed("Lỗi cập nhật tài khoản Identity");
                }

                profile.Branch_Id = model.ProfileInfo.Branch_Id;
                profile.Staff_Bio = model.ProfileInfo.Staff_Bio;
                profile.Start_Work_Time = model.ProfileInfo.Start_Work_Time;
                profile.End_Work_Time = model.ProfileInfo.End_Work_Time;
                profile.Update_At = DateTime.Now;

                _unitOfWork.Repository<StaffProfileModel>().Update(profile);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<StaffCombinedUpdateVM>.Success(model);
            }
            catch (Exception ex)
            {
                return ServiceResult<StaffCombinedUpdateVM>.Failed($"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> StaffSelfUpdateAsync(string staffId, StaffSelfUpdateVM model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(staffId);
                var profile = await _unitOfWork.Repository<StaffProfileModel>()
                    .Query()
                    .FirstOrDefaultAsync(x => x.StaffId == staffId);

                if (user == null || profile == null)
                    return ServiceResult<bool>.Failed("Không tìm thấy thông tin nhân viên");

                user.FullName = model.User.FullName;
                user.PhoneNumber = model.User.PhoneNumber;
                user.Email = model.User.Email;
                user.Address = model.User.Address;
                user.Update_At = DateTime.Now;

                if (model.User.Avatar_Image_Upload != null)
                {
                    if (!string.IsNullOrEmpty(user.Avatar_Name))
                        await _fileService.DeleteFileAsync(user.Avatar_Name, "users");

                    user.Avatar_Name = await _fileService.UploadFileAsync(
                        model.User.Avatar_Image_Upload, "users");
                }

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var removePass = await _userManager.RemovePasswordAsync(user);
                    if (!removePass.Succeeded)
                        return ServiceResult<bool>.Failed("Không thể đổi mật khẩu");

                    await _userManager.AddPasswordAsync(user, model.Password);
                }

                var userResult = await _userManager.UpdateAsync(user);
                if (!userResult.Succeeded)
                    return ServiceResult<bool>.Failed("Lỗi cập nhật tài khoản");

                profile.Staff_Bio = model.Staff_Bio;
                profile.Update_At = DateTime.Now;

                _unitOfWork.Repository<StaffProfileModel>().Update(profile);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failed($"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteAsync(string id)
        {
            try
            {
                var profile = await _unitOfWork.Repository<StaffProfileModel>().GetByIdAsync(id);
                var user = await _userManager.FindByIdAsync(id);

                if (profile != null)
                {
                    _unitOfWork.Repository<StaffProfileModel>().Delete(profile);
                    await _unitOfWork.SaveChangesAsync();
                }

                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.Avatar_Name))
                        await _fileService.DeleteFileAsync(user.Avatar_Name, "users");

                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded) return ServiceResult<bool>.Failed("Lỗi khi xóa tài khoản");
                }

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<IEnumerable<StaffProfileModel>> GetAllAsync()
        {
            var listProfile = await _unitOfWork.Repository<StaffProfileModel>()
                .Query()
                .Include(p => p.Staff)
                .Include(p => p.Branch)
                .ToListAsync();
            return listProfile;
        }

        public async Task<ServiceResult<StaffProfileModel>> GetByIdAsync(string id)
        {
            var profile = _unitOfWork.Repository<StaffProfileModel>()
                .Query()
                .Include(profile => profile.Staff)
                .Include(profile => profile.Branch)
                .FirstOrDefault(i => i.StaffId == id);

            if (profile == null)             {
                return ServiceResult<StaffProfileModel>.Failed("Stylist profile không tồn tại");
            }

            return ServiceResult<StaffProfileModel>.Success(profile);
        }
        public async Task<PaginatedList<StaffProfileModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<StaffProfileModel>().Query()
                .Include(p => p.Staff)
                    .ThenInclude(s => s.Ward)
                    .ThenInclude(s => s.District)
                    .ThenInclude(s=>s.Province)
                .Include(p => p.Branch)
                .OrderByDescending(p => p.Create_At) 
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(r => r.Staff.FullName.ToLower().Contains(searchTerm)
                                      || r.Staff.Email.ToLower().Contains(searchTerm));
            }

            return await PaginatedList<StaffProfileModel>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<IEnumerable<StaffProfileModel>> GetAllStylistActiveAsync()
        {
            var roleStylistId = await _roleManager.FindByNameAsync("Stylist");
            if (roleStylistId == null) return Enumerable.Empty<StaffProfileModel>();

            var reviewStylist = await _unitOfWork.Repository<ReviewModel>()
                .Query()
                .Where(r => r.Staff_Id == roleStylistId.Id)
                .ToListAsync();

            return await _unitOfWork.Repository<StaffProfileModel>()
                .Query()
                .Include(t => t.Staff)
                .Include(t=>t.Branch)
                .Where((p => p.Staff.Status == true && p.Staff.RoleId == roleStylistId.Id))
                .ToListAsync();
        }


        public async Task<IEnumerable<StaffProfileModel>> GetAllSkinnerActiveAsync()
        {
            var skinnerRole = await _roleManager.FindByNameAsync("Skinner");

            if (skinnerRole == null) return Enumerable.Empty<StaffProfileModel>();

            return await _unitOfWork.Repository<StaffProfileModel>()
                .Query()
                .Include(p => p.Staff)
                .Where(p => p.Staff.Status == true && p.Staff.RoleId == skinnerRole.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<IdentityRole>> GetRoleStaff()
        {
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            var customerRole = await _roleManager.FindByNameAsync("Customer");

            return _roleManager.Roles.Where(r => r.Id != adminRole.Id && r.Id != customerRole.Id);
        }

    }
}
