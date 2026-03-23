using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel;
using BookingSalon.Services.Interface;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class UserService : IUsersService
    {
        private readonly UserManager<UsersModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IFileService _fileService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRankService _customerRankService;

        public UserService(UserManager<UsersModel> userManager, RoleManager<IdentityRole> roleManager, IFileService fileService, IUnitOfWork unitOfWork, ICustomerRankService customerRankService)
        {
            _userManager = userManager;
            _fileService = fileService;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _customerRankService = customerRankService;
        }

        public Task<UsersModel?> GetByIdAsync(string id)
        {
            var user = _userManager.FindByIdAsync(id);
            return user;
        }

        public async Task<IdentityResult> CreateUserCustomerAsync(UserCreateViewModel userVM)
        {
            if (userVM == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Không được để trống. Vui lòng nhập đầy đủ" });
            }

            if (!String.IsNullOrEmpty(userVM.User.Email))
            {
                var existingEmail = await _userManager.FindByEmailAsync(userVM.User.Email);

                if (existingEmail != null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return IdentityResult.Failed(new IdentityError { Description = "Email đã được sử dụng" });
                }
            }

            var isPhoneTaken = await _userManager.Users.AnyAsync(x => x.PhoneNumber == userVM.User.PhoneNumber);
            if (isPhoneTaken)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return IdentityResult.Failed(new IdentityError { Description = "Số điện thoại đã được sử dụng" });
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                userVM.User.UserName = userVM.User.PhoneNumber;
                userVM.User.FullName = userVM.User.FullName ?? "Khách hàng" + userVM.User.PhoneNumber;
                userVM.User.Create_At = DateTime.Now;
                userVM.User.Update_At = DateTime.Now;
                userVM.User.Status = true;

                if (userVM.User.Avatar_Image_Upload != null)
                {
                    userVM.User.Avatar_Name = await _fileService
                        .UploadFileAsync(userVM.User.Avatar_Image_Upload, "users");
                }

                var resultAddCustomer = await _userManager.CreateAsync(userVM.User);

                if (!resultAddCustomer.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return IdentityResult.Failed(new IdentityError { Description = "Lỗi khi lưu khách hàng vào db" });
                }

                userVM.CustomerRank.Customer_Id = userVM.User.Id;

                var resultAddRank = await _customerRankService.CreateAsync(userVM.CustomerRank);

                if (!resultAddRank.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return IdentityResult.Failed(new IdentityError { Description = "Lỗi khi tạo rank cho người dùng" });
                }

                await _unitOfWork.SaveChangesAsync();

                var role = await _roleManager.FindByIdAsync(userVM.User.RoleId);
                if (role == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return IdentityResult.Failed(new IdentityError { Description = "Vai trò người dùng không tồn tại" });
                }

                await _userManager.AddToRoleAsync(userVM.User, role.Name);
                await _unitOfWork.CommitTransactionAsync();

                return resultAddCustomer;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return IdentityResult.Failed(new IdentityError { Description = "Lỗi khi tạo người dùng" });
            }
        }

        //Admin, Reception cập nhật thông tin khách hàng
        public async Task<IdentityResult> UpdateUserCustomerAsync(UserUpdateViewModel userVM, string id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingUser = await _userManager.FindByIdAsync(id);
                if (existingUser == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return IdentityResult.Failed(new IdentityError { Description = "Không tìm thấy người dùng" });
                }


                existingUser.FullName = userVM.User.FullName;
                existingUser.PhoneNumber = userVM.User.PhoneNumber;
                existingUser.Address = userVM.User.Address;
                existingUser.WardId = userVM.User.WardId;
                existingUser.Status = userVM.User.Status;

                if (userVM.User.Avatar_Image_Upload != null)
                {
                    if (!string.IsNullOrEmpty(userVM.User.Avatar_Name))
                        await _fileService.DeleteFileAsync(userVM.User.Avatar_Name, "users");

                    existingUser.Avatar_Name = await _fileService.UploadFileAsync(userVM.User.Avatar_Image_Upload, "users");
                }

                var updateResult = await _userManager.UpdateAsync(existingUser);
                if (!updateResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return updateResult;
                }

                if (userVM.CustomerRank != null)
                {
                    var dbRank = await _unitOfWork.Repository<CustomerRankModel>()
                        .Query()
                        .FirstOrDefaultAsync(cr => cr.Customer_Id == id);

                    if (dbRank != null)
                    {
                        int addedPoints = userVM.CustomerRank.CurrentPoints - dbRank.CurrentPoints;

                        if (addedPoints > 0)
                        {
                            dbRank.LifetimePoints += addedPoints;
                        }

                        dbRank.CurrentPoints = userVM.CustomerRank.CurrentPoints;
                        dbRank.Is_Active = userVM.CustomerRank.Is_Active;
                        dbRank.Update_At = DateTime.Now;

                        var rankResult = await _customerRankService.UpdateAsync(id, dbRank);

                        if (!rankResult.Succeeded)
                        {
                            await _unitOfWork.RollbackTransactionAsync();
                            return IdentityResult.Failed(new IdentityError { Description = "Lỗi cập nhật hạng: " + rankResult.Errors });
                        }
                    }
                }

                await _unitOfWork.CommitTransactionAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return IdentityResult.Failed(new IdentityError { Description = "Lỗi hệ thống: " + ex.Message });
            }
        }
        //Profile cá nhân Customer
        public async Task<IdentityResult> UpdateCustomerProfileAsync(string userId, ProfileViewModel model)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var phoneRegex = @"^(0|\+84)[0-9]{9}$";
                if (!string.IsNullOrEmpty(model.PhoneNumber) &&
                    !System.Text.RegularExpressions.Regex.IsMatch(model.PhoneNumber, phoneRegex))
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Số điện thoại không hợp lệ" });
                }
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Tài khoản không tồn tại." });
                }

                user.FullName = model.FullName;
                user.Address = model.Address;
                user.WardId = model.WardId;
                user.Update_At = DateTime.Now;

                if (user.PhoneNumber != model.PhoneNumber)
                {
                    var phoneExists = await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber && u.Id != userId);
                    if (phoneExists)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return IdentityResult.Failed(new IdentityError { Description = "Số điện thoại mới đã được sử dụng bởi tài khoản khác." });
                    }
                    user.PhoneNumber = model.PhoneNumber;
                    user.UserName = model.PhoneNumber;
                }

                if (model.Avatar_Image_Upload != null)
                {
                    if (!string.IsNullOrEmpty(user.Avatar_Name))
                    {
                        await _fileService.DeleteFileAsync(user.Avatar_Name, "users");
                    }
                    user.Avatar_Name = await _fileService.UploadFileAsync(model.Avatar_Image_Upload, "users");
                }

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return result;
                }

                await _unitOfWork.CommitTransactionAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return IdentityResult.Failed(new IdentityError { Description = "Lỗi hệ thống: " + ex.Message });
            }
        }

        //Profile Admin
        public async Task<IdentityResult> UpdateUserAsync(UserViewModel user, string id)
        {
            try
            {
                var existingUser = await _userManager.FindByIdAsync(id);
                if (existingUser == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Không tìm thấy người dùng" });
                }

                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.UserName = user.Email;
                existingUser.Address = user.Address;
                existingUser.WardId = user.WardId;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Update_At = DateTime.Now;

                string oldImageName = existingUser.Avatar_Name;
                if (user.Avatar_Image_Upload != null)
                {
                    if (!string.IsNullOrEmpty(oldImageName))
                    {
                        await _fileService.DeleteFileAsync(oldImageName, "users");
                    }

                    existingUser.Avatar_Name = await _fileService.UploadFileAsync(user.Avatar_Image_Upload, "users");
                }

                if (existingUser.Email != user.Email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(existingUser, user.Email);
                    var setUserNameResult = await _userManager.SetUserNameAsync(existingUser, user.Email);
                }

                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    var removePassResult = await _userManager.RemovePasswordAsync(existingUser);
                    if (removePassResult.Succeeded)
                        await _userManager.AddPasswordAsync(existingUser, user.Password);
                }

                var updateResult = await _userManager.UpdateAsync(existingUser);
                if (!updateResult.Succeeded)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Lỗi hệ thống: " });
                }
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Lỗi hệ thống: " + ex.Message });
            }
        }

        public async Task<ServiceResult<UsersModel>> Delete(string id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return ServiceResult<UsersModel>.Failed("Không tìm thấy người dùng");
                }

                var rankDeleteResult = await _customerRankService.DeleteAsync(user.Id);

                if (!string.IsNullOrEmpty(user.Avatar_Name))
                {
                    await _fileService.DeleteFileAsync(user.Avatar_Name, "users");
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ServiceResult<UsersModel>.Failed("Lỗi khi xóa tài khoản từ hệ thống Identity");
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return ServiceResult<UsersModel>.Success(user);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult<UsersModel>.Failed($"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<PaginatedList<CustomerRankVM>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm, string sortOrder)
        {
            var query = _unitOfWork.Repository<CustomerRankModel>().Query();

            query = query.Include(cr => cr.Customer)
                                .ThenInclude(cr => cr.Ward)
                                    .ThenInclude(w => w.District)
                            .ThenInclude(d => d.Province)
                        .Include(cr => cr.Rank);
                            

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(cr => cr.Customer.FullName.ToLower().Contains(searchTerm)
                                       || cr.Customer.PhoneNumber.Contains(searchTerm));
            }

            query = sortOrder switch
            {
                "rank_desc" => query.OrderByDescending(cr => cr.LifetimePoints),
                "rank_asc" => query.OrderBy(cr => cr.LifetimePoints),
                _ => query.OrderByDescending(cr => cr.Created_At)
            };

            var projection = query.Select(cr => new CustomerRankVM
            {
                    CustomerId = cr.Customer_Id,
                    CustomerName = cr.Customer.FullName,
                    CustomerEmail = cr.Customer.Email,
                    CustomerPhone = cr.Customer.PhoneNumber,
                    Address = cr.Customer.Address,
                    AvatarName = cr.Customer.Avatar_Name,
                    RankName = cr.Rank.RankName,
                    CurrentPoints = cr.CurrentPoints,
                    LifetimePoints = cr.LifetimePoints,
                    DiscountPercent = cr.Rank.DiscountPercent,
                    Status = cr.Customer.Status,
                    EmailConfirm = cr.Customer.EmailConfirmed,
                    Ward = cr.Customer.Ward,
                    ActiveRank = cr.Is_Active,
                    CreateAt = cr.Created_At
                });

            return await PaginatedList<CustomerRankVM>.CreateAsync(projection, pageNumber, pageSize);
        }

        public async Task<IEnumerable<UsersModel>> GetAllCustomerAsync()
        {
            var roleCustomer = await _roleManager.FindByNameAsync("Customer");
            var roleCustomerId = roleCustomer?.Id;

            return await _userManager.Users
                .Where(u => u.RoleId == roleCustomerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsersModel>> GetAllStylistAsync()
        {
            var roleStylist = await _roleManager.FindByNameAsync("Stylist");
            var roleStylistId = roleStylist?.Id;

            return await _userManager.Users
                .Where(u => u.RoleId == roleStylistId && u.Status == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsersModel>> GetAllSkinnerAsync()
        {
            var roleSkinner = await _roleManager.FindByNameAsync("Skinner");
            var roleSkinnerId = roleSkinner?.Id;

            return await _userManager.Users
                .Where(u => u.RoleId == roleSkinnerId && u.Status == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsersModel>> GetAllReceptionAsync()
        {
            var roleReception = await _roleManager.FindByNameAsync("Reception");
            var roleReceptionId = roleReception?.Id;

            return await _userManager.Users
                .Where(u => u.RoleId == roleReceptionId && u.Status == true)
                .ToListAsync();
        }

        public async Task<WardModel> GetWardById(UsersModel user)
        {
            var ward = await _unitOfWork.Repository<WardModel>().Query()
                    .Include(w => w.District)
                    .FirstOrDefaultAsync(w => w.Id == user.WardId);

            return ward;
        }

    }
}
