using BookingSalon.Data.Validation;
using BookingSalon.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Areas.Admin.Models.ViewModel
{
    public class CreateStaffProfileVM
    {
        public StaffProfileModel StylistProfile { get; set; }

        [FileExtension(ErrorMessage = "Chỉ chấp nhận file ảnh (jpg, png, jpeg)")]
        public List<IFormFile>? ImageUploads { get; set; }

        public SelectList? StylistUsersSelectList { get; set; }
        public SelectList? BranchSelectList { get; set; }
    }
}
