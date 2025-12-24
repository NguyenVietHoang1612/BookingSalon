using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;

namespace BookingSalon.Data.Validation
{
    public class FileExtension : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            if (value is IFormFile file)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return new ValidationResult(
                        $"File có đuôi không phù hợp với hệ thống. Hệ thống chỉ phù hợp với các đuôi: {string.Join(", ", allowedExtensions)}"
                    );
                }
            }

            return ValidationResult.Success;

        }
    }
}
