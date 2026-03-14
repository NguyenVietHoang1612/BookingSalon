using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSalon.Controllers
{
    [Authorize]
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpPost]
        public async Task<IActionResult> ValidateCouponAjax(string code, decimal orderAmount)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _couponService
                .ValidateForBookingAsync(code, orderAmount, userId);

            if (!result.Succeeded)
                return Json(new { succeeded = false, errors = result.Errors });

            return Json(new
            {
                succeeded = true,
                discount = result.Data.Discount
            });
        }
    }
}
