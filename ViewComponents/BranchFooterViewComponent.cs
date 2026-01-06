using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookingSalon.ViewComponents
{
    public class BranchFooterViewComponent : ViewComponent
    {
        private readonly IBranchService _branchService;

        public BranchFooterViewComponent(IBranchService branchService)
        {
            _branchService = branchService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var branches = await _branchService.GetAllAsync();
            return View(branches.Data);
        }
    }
}
