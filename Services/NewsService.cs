using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Services
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public NewsService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<ServiceResult<NewsVM>> CreateAsync(NewsVM newsVM, string userId)
        {
            try
            {
                newsVM.News.AuthorId = userId;
                newsVM.News.Created_At = DateTime.Now;
                newsVM.News.Updated_At = DateTime.Now;

                if (newsVM.ImageUpload != null)
                {
                    newsVM.News.Thumbnail = await _fileService.UploadFileAsync(newsVM.ImageUpload, "news");
                }

                await _unitOfWork.Repository<NewsModel>().AddAsync(newsVM.News);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<NewsVM>.Success(newsVM);
            }
            catch (Exception ex)
            {
                return ServiceResult<NewsVM>.Failed($"Lỗi khi tạo tin tức: {ex.Message}");
            }
        }

        public async Task<ServiceResult<NewsVM>> UpdateAsync(int id, NewsVM newsVM)
        {
            try
            {
                var existingNews = await _unitOfWork.Repository<NewsModel>().GetByIdAsync(id);
                if (existingNews == null) return ServiceResult<NewsVM>.Failed("Không tìm thấy bài viết");

                string imageName = existingNews.Thumbnail;

                existingNews.Title = newsVM.News.Title;
                existingNews.Summary = newsVM.News.Summary;
                existingNews.Content = newsVM.News.Content; 
                existingNews.IsActive = newsVM.News.IsActive;
                existingNews.PublishedDate = newsVM.News.PublishedDate;
                existingNews.Updated_At = DateTime.Now;

                if (newsVM.ImageUpload != null)
                {
                    if (!string.IsNullOrEmpty(imageName))
                    {
                        await _fileService.DeleteFileAsync(imageName, "news");
                    }
                    existingNews.Thumbnail = await _fileService.UploadFileAsync(newsVM.ImageUpload, "news");
                }

                _unitOfWork.Repository<NewsModel>().Update(existingNews);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<NewsVM>.Success(newsVM);
            }
            catch (Exception ex)
            {
                return ServiceResult<NewsVM>.Failed($"Lỗi khi cập nhật: {ex.Message}");
            }
        }

        public async Task<ServiceResult<NewsModel>> DeleteAsync(int id)
        {
            try
            {
                var news = await _unitOfWork.Repository<NewsModel>().GetByIdAsync(id);
                if (news == null) return ServiceResult<NewsModel>.Failed("Không tìm thấy tin tức");

                if (!string.IsNullOrEmpty(news.Thumbnail))
                {
                    await _fileService.DeleteFileAsync(news.Thumbnail, "news");
                }

                _unitOfWork.Repository<NewsModel>().Delete(news);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<NewsModel>.Success(news);
            }
            catch (Exception ex)
            {
                return ServiceResult<NewsModel>.Failed($"Lỗi: {ex.Message}");
            }
        }

        public async Task<IEnumerable<NewsModel>> GetAllAsync()
        {
            return await _unitOfWork.Repository<NewsModel>().Query()
                .Include(n => n.Author)
                .OrderByDescending(n => n.PublishedDate)
                .ToListAsync();
        }

        public async Task<ServiceResult<NewsModel>> GetByIdAsync(int id)
        {
            var news = await _unitOfWork.Repository<NewsModel>().Query()
                .Include(n => n.Author)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (news == null) return ServiceResult<NewsModel>.Failed("Không tồn tại");
            return ServiceResult<NewsModel>.Success(news);
        }

        public async Task<PaginatedList<NewsModel>> GetPagedListAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<NewsModel>().Query();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(searchTerm) || n.Author.FullName.ToLower().Contains(searchTerm));
            }
            query = query.Include(n => n.Author);

            return await PaginatedList<NewsModel>.CreateAsync(query.OrderByDescending(n => n.Created_At), pageNumber, pageSize);
        }

        public async Task<PaginatedList<NewsModel>> GetReceptionPagedListAsync(string receptionId,int pageNumber, int pageSize, string searchTerm)
        {
            var query = _unitOfWork.Repository<NewsModel>().Query();
            query = query.Include(n => n.Author).
                Where(n => n.Author.Id == receptionId);
           
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(n => n.Title.ToLower().Contains(searchTerm) || n.Author.FullName.ToLower().Contains(searchTerm));
            }
            

            return await PaginatedList<NewsModel>.CreateAsync(query.OrderByDescending(n => n.Created_At), pageNumber, pageSize);
        }
    }
}
