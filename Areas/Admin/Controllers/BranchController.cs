using BookingSalon.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BranchController : Controller
    {
        private readonly BookingContext bookingContext;

        public BranchController(BookingContext bookingContext)
        {
            this.bookingContext = bookingContext;
        }

        public async Task<IActionResult> Index()
        {
            var branches = await bookingContext.Branches
                                       .OrderBy(od => od.BranchId)
                                       .ToListAsync();
            return View(branches);

        }
    }
}
