using BookingSalon.Data.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Models.Entities
{
    public class Service
    {
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
        [StringLength(150, ErrorMessage = "Tên dịch vụ không vượt quá 150 ký tự")]
        public string Service_Name { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá dịch vụ không hợp lệ")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

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

        public DateTime Create_At { get; set; }
        public DateTime Update_At { get; set; }

        [NotMapped]
        [FileExtension(ErrorMessage = "Chỉ chấp nhận file ảnh (jpg, png, jpeg)")]
        public IFormFile? Service_Image_Upload { get; set; }

        public TypeOfService? TypeOfService { get; set; }
        public ICollection<BookingDetail>? BookingDetails { get; set; }
    }
}
