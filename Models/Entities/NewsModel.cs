using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingSalon.Models.Entities
{
    public class NewsModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề tin tức")]
        [Display(Name = "Tiêu đề")]
        [StringLength(100)]
        public string Title { get; set; }

        [Display(Name = "Mô tả ngắn")]
        [StringLength(250)]
        public string? Summary { get; set; } 

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [Display(Name = "Nội dung chi tiết")]
        public string Content { get; set; } 

        [Display(Name = "Hình ảnh")]
        public string? Thumbnail { get; set; } 

        [Display(Name = "Ngày đăng")]
        public DateTime PublishedDate { get; set; } = DateTime.Now;

        [Display(Name = "Hiển thị lên trang chủ?")]
        public bool IsActive { get; set; } = true;

        public string? AuthorId { get; set; }

        public UsersModel? Author { get; set; }

        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
}
