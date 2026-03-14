using BookingSalon.Data.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Models.Entities
{
    public class BranchModel
    {
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Tên chi nhánh không được để trống")]
        [StringLength(150, ErrorMessage = "Tên chi nhánh không vượt quá 150 ký tự")]
        public string Branch_Name { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(255, ErrorMessage = "Địa chỉ không vượt quá 255 ký tự")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
        public int? WardId { get; set; } 

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0|\+84)[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }
        public string? Branch_Image { get; set; }
        public bool? Is_Main_Branch { get; set; }

        public bool Status { get; set; }

        public DateTime Created_At { get; set; }
        public DateTime Update_At { get; set; }

        [NotMapped]
        [FileExtension(ErrorMessage = "Chỉ chấp nhận file ảnh (jpg, png, jpeg)")]
        public IFormFile? Branch_Image_Upload { get; set; }

        public WardModel? Ward { get; set; }

       
    }
}
