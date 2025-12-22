namespace BookingSalon.Services.Interface
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder);
        Task DeleteFileAsync(string fileName, string folder);
    }
}
