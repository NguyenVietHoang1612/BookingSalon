using BookingSalon.Areas.Admin.Models;
using BookingSalon.Data.Repository;
using BookingSalon.Migrations;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookingSalon.Services
{
    public class StylistProfileService : IStylistProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly UserManager<Users> _userManager;

        public StylistProfileService(IUnitOfWork context, IFileService fileService, UserManager<Users> userManager)
        {
            _unitOfWork = context;
            _fileService = fileService;
            _userManager = userManager;
        }

        public async Task<ServiceResult<CreateStylistProfileVM>> CreateAsync(CreateStylistProfileVM stylistProfileVM)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(stylistProfileVM.StylistProfile.StylistId);

                if (user == null)
                {
                    return ServiceResult<CreateStylistProfileVM>.Failed("StylistId không tồn tại");
                }

                var exists = await _unitOfWork.Repository<StylistProfile>().ExistsAsync(x => x.StylistId == stylistProfileVM.StylistProfile.StylistId);

                if (exists)
                {
                    return ServiceResult<CreateStylistProfileVM>.Failed("Stylist profile đã tồn tại");
                }

                stylistProfileVM.StylistProfile.Update_At = DateTime.Now;

                await _unitOfWork.Repository<StylistProfile>()
                    .AddAsync(stylistProfileVM.StylistProfile);

                await _unitOfWork.SaveChangesAsync();

                if (stylistProfileVM.ImageUploads != null)
                {
                    var images = new List<StylistImage>();

                    foreach (var file in stylistProfileVM.ImageUploads)
                    {
                        var imageUrl = await _fileService.UploadFileAsync(file, "stylist");

                        images.Add(new StylistImage
                        {
                            StylistProfileId = stylistProfileVM.StylistProfile.StylistId,
                            ImageUrl = imageUrl,
                            Create_At = DateTime.Now
                        });
                    }

                    await _unitOfWork.Repository<StylistImage>()
                        .AddRangeAsync(images);

                    await _unitOfWork.SaveChangesAsync();
                }

                return ServiceResult<CreateStylistProfileVM>.Success(stylistProfileVM);
            }
            catch(Exception ex)
            { 

                return ServiceResult<CreateStylistProfileVM>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<ServiceResult<StylistProfile>> DeleteAsync(string id)
        {
            try
            {
                var profile = await _unitOfWork.Repository<StylistProfile>().GetByIdAsync(id);

                if (profile == null)
                    return  ServiceResult<StylistProfile>.Failed("Không tìm thấy dịch vụ");


                _unitOfWork.Repository<StylistProfile>().Delete(profile);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<StylistProfile>.Success(profile);
            }
            catch (Exception ex)
            {
                return ServiceResult<StylistProfile>.Failed($"Lỗi: {ex.Message}");
            }
        }
        

        public async Task<IEnumerable<StylistProfile>> GetAllAsync()
        {
            var listProfile = await _unitOfWork.Repository<StylistProfile>()
                .Query()
                .Include(p => p.Stylist)
                .Include(p => p.Branch)
                .ToListAsync();
            return listProfile;
        }

        public async Task<IEnumerable<Branch>> GetAllBranchAsync()
        {
            var listBranch = await _unitOfWork.Repository<Branch>().GetAllAsync();
            return listBranch;
        } 

        public async Task<ServiceResult<StylistProfile>> GetByIdAsync(string id)
        {
            var profile = _unitOfWork.Repository<StylistProfile>()
                .Query()
                .Include(profile => profile.Stylist)
                .Include(p => p.StylistImages)
                .FirstOrDefault(i => i.StylistId == id);

            if (profile == null)             {
                return ServiceResult<StylistProfile>.Failed("Stylist profile không tồn tại");
            }

            return ServiceResult<StylistProfile>.Success(profile);
        }

        public async Task<ServiceResult<UpdateStylistProfileVM>> UpdateAsync(string id, UpdateStylistProfileVM stylistProfileVM)
        {
            try
            {
                var repo = _unitOfWork.Repository<StylistProfile>();
                var repoImage = _unitOfWork.Repository<StylistImage>();

                var existingStylistProfile = await repo.Query()
                .Include(x => x.StylistImages)
                .FirstOrDefaultAsync(x => x.StylistId == stylistProfileVM.StylistProfile.StylistId);

                if (existingStylistProfile == null)
                    return ServiceResult<UpdateStylistProfileVM>.Failed("Không tìm thấy Profile");

                existingStylistProfile.Branch_Id = stylistProfileVM.StylistProfile.Branch_Id;
                existingStylistProfile.StylistSkill = stylistProfileVM.StylistProfile.StylistSkill;
                existingStylistProfile.StylistExperience = stylistProfileVM.StylistProfile.StylistExperience;
                existingStylistProfile.Start_Work_Time = stylistProfileVM.StylistProfile.Start_Work_Time;
                existingStylistProfile.End_Work_Time = stylistProfileVM.StylistProfile.End_Work_Time;
                existingStylistProfile.Update_At = DateTime.Now;

                 _unitOfWork.Repository<StylistProfile>().Update(existingStylistProfile);

                await _unitOfWork.SaveChangesAsync();

                foreach(var img in stylistProfileVM.Images)
                {
                    if (img.IsRemoved && img.ImageId.HasValue)
                    {
                        var entity = await repoImage
                            .GetByIdAsync(img.ImageId);

                        if (entity != null)
                        {
                            var result = _fileService.DeleteFileAsync(entity.ImageUrl, "stylist");
                            if (result.IsCompleted) 
                            {
                                repoImage.Delete(entity);
                            }
                            else
                            {
                                return ServiceResult<UpdateStylistProfileVM>.Failed("Xóa ảnh thất bại");
                            }
                            
                        }
                    }
                }

                if (stylistProfileVM.NewImageUploads != null)
                {
                    foreach (var file in stylistProfileVM.NewImageUploads)
                    {
                        var fileName = await _fileService.UploadFileAsync(file, "stylist");

                        await repoImage.AddAsync(new StylistImage
                        {
                            StylistProfileId = stylistProfileVM.StylistProfile.StylistId,
                            ImageUrl = fileName,
                            Update_At = DateTime.Now
                        });
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<UpdateStylistProfileVM>.Success(stylistProfileVM);
            }
            catch (Exception ex)
            {

                return ServiceResult<UpdateStylistProfileVM>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<PaginatedList<StylistProfile>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<StylistProfile>().Query();

            query = query.Include(p => p.Stylist)
                .Include(p => p.Branch);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower().Trim();
                query = query.Where(r => r.Stylist.FullName.ToLower().Contains(searchTerm) || r.StylistId.ToLower().Contains(searchTerm));
            }

            return await PaginatedList<StylistProfile>.CreateAsync(query, pageNumber, pageSize);
        }
    }
}
