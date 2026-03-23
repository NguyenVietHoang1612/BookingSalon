using BookingSalon.Areas.Admin.Models;
using BookingSalon.Models.Entities;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Reception")]
    public class ServiceSalonController : Controller
    {
        private readonly IServicesSalonService _serviceSalonService;
        private readonly ITypeOfServiceService _typeOfServiceService;

        public ServiceSalonController(IServicesSalonService serviceSalonService, ITypeOfServiceService typeOfServiceService)
        {
            _serviceSalonService = serviceSalonService;
            _typeOfServiceService = typeOfServiceService;
        }

        public async Task<IActionResult> Index(int? pageNumber, string term)
        {
            int pageSize = 10;
            var listService = await _serviceSalonService.GetPagedListAsync(pageNumber ?? 1, pageSize, term);


            if (!string.IsNullOrEmpty(term))
            {
                ViewBag.SearchTerm = term;
            }

            return View(listService);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var listService = await _typeOfServiceService.GetAllTypeServiceAsync();
            ViewBag.TypeOfServiceList = new SelectList(listService, "TypeOfServiceId", "Type_Service_Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ServiceModel service)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = $"Lỗi Bind dữ liệu dịch vụ!";

                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                var listService = await _typeOfServiceService.GetAllTypeServiceAsync();
                ViewBag.TypeOfServiceList = new SelectList(listService, "TypeOfServiceId", "Type_Service_Name");
                string errorMessage = string.Join("; ", errors);

                return BadRequest(errorMessage);
            }

            var result = await _serviceSalonService.CreateAsync(service);

            if (!result.Succeeded)
            {
                TempData["Error"] = "Thêm dịch vụ thất bại: " + result.Errors;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                var listService = await _typeOfServiceService.GetAllTypeServiceAsync();
                ViewBag.TypeOfServiceList = new SelectList(listService, "TypeOfServiceId", "Type_Service_Name");
                return View(service);

            }
            TempData["Success"] = $"Thêm dịch vụ {service.Service_Name} thành công!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id)
        {
            var service = await _serviceSalonService.GetByIdAsync(id);
            var typeOfService = await _typeOfServiceService.GetAllTypeServiceAsync();

            ViewBag.TypeOfServiceList = new SelectList(typeOfService, "TypeOfServiceId", "Type_Service_Name");

            if (service == null) return NotFound();

            ServiceModel serviceDetail = new ServiceModel
            {
                Service_Name = service.Data.Service_Name,
                Type_Service_Id = service.Data.Type_Service_Id,
                Base_Price = service.Data.Base_Price,
                DurationInMinutes = service.Data.DurationInMinutes,
                Promotion_Start = service.Data.Promotion_Start,
                Promotion_End = service.Data.Promotion_End,
                Promotion_Price = service.Data.Promotion_Price,
                description = service.Data.description,
                ImageName = service.Data.ImageName,
                Status = service.Data.Status
            };

            return View(serviceDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, ServiceModel service)
        {
            if (!ModelState.IsValid)
            {
                TempData["Warning"] = $"Lỗi Bind dữ liệu dịch vụ!";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                var typeOfService = await _typeOfServiceService.GetAllTypeServiceAsync();

                ViewBag.TypeOfServiceList = new SelectList(typeOfService, "TypeOfServiceId", "Type_Service_Name");
                string errorMessage = string.Join("; ", errors);

                return BadRequest(errorMessage);
            }

            var result = await _serviceSalonService.UpdateAsync(id, service);

            if (!result.Succeeded)
            {
                TempData["Error"] = "Cập nhật dịch vụ thất bại: " + result.Errors;

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                var typeOfService = await _typeOfServiceService.GetAllTypeServiceAsync();

                ViewBag.TypeOfServiceList = new SelectList(typeOfService, "TypeOfServiceId", "Type_Service_Name");
                return View(service);

            }
            TempData["Success"] = $"Cập nhật dịch vụ {service.Service_Name} thành công!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _serviceSalonService.GetByIdAsync(id);

            if (!result.Succeeded)
                return NotFound();

            return View(result.Data);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result =  await _serviceSalonService.DeleteAsync(id);
            
            if (result.Succeeded)
            {
                return Ok(new { message = "Đã chuyển trạng thái user sang ngừng hoạt động" });
            }

            return BadRequest(new { success = false, message = "Không thể xóa dữ liệu" });
        }
    }
}
