using BookingSalon.Areas.Admin.Models;
using BookingSalon.Models.Entities;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ServiceSalonController : Controller
    {
        private readonly IServicesSalonService _serviceSalonService;

        public ServiceSalonController(IServicesSalonService serviceSalonService)
        {
            _serviceSalonService = serviceSalonService;
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

        public async Task<IActionResult> Create()
        {
            var listService = await _serviceSalonService.GetAllTypeOfServiceAsync();
            ViewBag.TypeOfServiceList = new SelectList(listService, "TypeOfServiceId", "Type_Service_Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Service service)
        {
            if (!ModelState.IsValid)
            {
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                string errorMessage = string.Join("; ", errors);

                return BadRequest(errorMessage);
            }

            var result = await _serviceSalonService.CreateAsync(service);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.Errors;
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View(service);

            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            var service = await _serviceSalonService.GetByIdAsync(id);
            var typeOfService = await _serviceSalonService.GetAllTypeOfServiceAsync();

            ViewBag.TypeOfServiceList = new SelectList(typeOfService, "TypeOfServiceId", "Type_Service_Name");

            if (service == null) return NotFound();

            Service serviceDetail = new Service
            {
                Service_Name = service.Data.Service_Name,
                Type_Service_Id = service.Data.Type_Service_Id,
                Price = service.Data.Price,
                DurationInMinutes = service.Data.DurationInMinutes,
                description = service.Data.description,
                ImageName = service.Data.ImageName,
                Status = service.Data.Status
            };

            return View(serviceDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Service service)
        {
            if (!ModelState.IsValid)
            {
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                string errorMessage = string.Join("; ", errors);

                return BadRequest(errorMessage);
            }

            var result = await _serviceSalonService.UpdateAsync(id, service);

            if (!result.Succeeded)
            {

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return View(service);

            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SoftDelete(int id)
        {
            await _serviceSalonService.DeleteAsync(id);
            return Ok(new { message = "Đã chuyển trạng thái user sang ngừng hoạt động" });
        }
    }
}
