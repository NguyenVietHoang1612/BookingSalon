using BookingSalon.Data.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Models.Entities
{
    public class ServiceModel
    {
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
        [StringLength(150, ErrorMessage = "Tên dịch vụ không vượt quá 150 ký tự")]
        public string Service_Name { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá dịch vụ không hợp lệ")]
        [DataType(DataType.Currency)]
        public decimal Base_Price { get; set; }
        public decimal? Promotion_Price { get; set; }
        public DateTime? Promotion_Start { get; set; }
        public DateTime? Promotion_End { get; set; }
        public int Type_Service_Id { get; set; }

        [Range(1, 600, ErrorMessage = "Thời gian dịch vụ phải lớn hơn 0")]
        [Required(ErrorMessage = "Thời gian không được để trống")]
        public int DurationInMinutes { get; set; }

        [StringLength(255)]
        public string? ImageName { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(500, ErrorMessage = "Mô tả không vượt quá 500 ký tự")]
        public string? description { get; set; }

        public bool Status { get; set; }

        [BindNever]
        public bool IsPromotionActive { get; private set; }

        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        [NotMapped]
        [FileExtension(ErrorMessage = "Chỉ chấp nhận file ảnh (jpg, png, jpeg)")]
        public IFormFile? Service_Image_Upload { get; set; }

        public TypeOfServiceModel? TypeOfService { get; set; }
        public ICollection<BookingDetailModel>? BookingDetails { get; set; }
    }
}
