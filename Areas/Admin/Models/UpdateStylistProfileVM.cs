using BookingSalon.Data.Validation;
using BookingSalon.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingSalon.Areas.Admin.Models
{
    public class UpdateStylistProfileVM
    {
        public StylistProfile StylistProfile { get; set; }

        public List<StylistImageItemVM> Images { get; set; } = new();

        [FileExtension(ErrorMessage = "Chỉ chấp nhận file ảnh (jpg, png, jpeg)")]
        public List<IFormFile>? NewImageUploads { get; set; }

        public SelectList? BranchSelectList { get; set; }
    }
}
