using BookingSalon.Models.Entities;
using BookingSalon.Services;
using BookingSalon.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly IPaymentService _paymentService;
        private readonly IBookingService _bookingService;
        public PaymentController(IVnPayService vnPayService, IPaymentService paymentService, IBookingService bookingService)
        {

            _vnPayService = vnPayService;
            _paymentService = paymentService;
            _bookingService = bookingService;
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(int bookingId, string paymentMethod, decimal totalAmount)
        {
            var userLogin = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bookingResult = await _bookingService.GetByIdAsync(bookingId);
            if (!bookingResult.Succeeded || bookingResult.Data == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin đơn hàng.";
                return RedirectToAction("Index", "Booking");
            }

            var booking = bookingResult.Data;
            string customerName = booking.Customer?.FullName ?? "Khách hàng";

            if (paymentMethod == "Tiền mặt")
            {
                var updateResult = await _bookingService.UpdateStatusAsync(bookingId, 4, userLogin);

                if (updateResult.Succeeded)
                {
                    var paymentModel = new PaymentModel
                    {
                        BookingId = bookingId,
                        Amount = totalAmount, 
                        PaymentMethod = "Tiền mặt",
                        ProcessedBy = userLogin,
                        PaymentDate = DateTime.Now,
                    };

                    await _paymentService.CreatePaymentAsync(paymentModel);

                    TempData["Success"] = $"Thanh toán tiền mặt thành công cho khách {customerName}!";
                    return RedirectToAction("BookingDetails", "Booking", new { area = "Admin", id = bookingId });
                }
                else
                {
                    TempData["Error"] = "Cập nhật trạng thái thất bại.";
                }
            }
            else
            {
                var paymentInfo = new PaymentInformationModel
                {
                    BookingId = bookingId,
                    Amount = (double)totalAmount,
                    OrderDescription = $"Thanh toan don hang #{bookingId}",
                    OrderType = "beauty",
                    Name = customerName
                };
                var url = _vnPayService.CreatePaymentUrl(paymentInfo, HttpContext);
                return Redirect(url);
            }

            return RedirectToAction("BookingDetails", "Booking", new { area = "Admin", id = bookingId });
        }
    }
}
