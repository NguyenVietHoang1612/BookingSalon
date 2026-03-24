using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Models.Entities;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ComboController : Controller
    {
        private readonly IComboService _comboService;
        private readonly IServicesSalonService _serviceService; 

        public ComboController(IComboService comboService, IServicesSalonService serviceService)
        {
            _comboService = comboService;
            _serviceService = serviceService;
        }


        public async Task<IActionResult> Index()
        {
            var combos = await _comboService.GetAllComboAsync();
            return View(combos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new ComboViewModel
            {
                Services = await _serviceService.GetAllServiceActiveAsync(),
                ComboModel = new ComboModel(),
                ComboServiceModel = new List<ComboServiceModel>()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComboViewModel vm, int[] selectedServices) 
        {
            if (selectedServices == null || selectedServices.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một dịch vụ cho Combo.");
            }
            else
            {
                vm.ComboServiceModel = selectedServices.Select(id => new ComboServiceModel
                {
                    ServiceId = id
                }).ToList();
              

                var result = await _comboService.Create(vm);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Tạo combo thành công!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Errors[0]);
            }

            // Nếu lỗi, phải load lại danh sách dịch vụ để hiển thị lại checkbox ở View
            vm.Services = await _serviceService.GetAllServiceActiveAsync();
            return View(vm);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _comboService.DeleteAsync(id);

            if (result.Succeeded)
            {
                return Ok(new { success = true, message = "Xóa thành công" });
            }

            return BadRequest(new { success = false, message = "Không thể xóa dữ liệu" });
        }
    }
}