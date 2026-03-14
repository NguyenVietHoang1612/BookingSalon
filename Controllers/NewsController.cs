using BookingSalon.Data.Repository;
using BookingSalon.Models.Entities;
using BookingSalon.Models.ViewModel;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BookingSalon.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        public async Task<IActionResult> Index(int? pageNumber, string searchTerm)
        {
            int pageSize = 6;
            var pagedNews = await _newsService.GetPagedListAsync(pageNumber ?? 1, pageSize, searchTerm);
            var featuredNews = (await _newsService.GetAllAsync())
                                .Where(n => n.IsActive)
                                .Take(3)
                                .ToList();

            var viewModel = new NewsPageVM
            {
                NewsList = pagedNews,
                FeaturedNews = featuredNews
            };

            ViewBag.SearchTerm = searchTerm;
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _newsService.GetByIdAsync(id);

            if (!result.Succeeded || !result.Data.IsActive)
                return NotFound();

            return View(result.Data);
        }
    }
}
