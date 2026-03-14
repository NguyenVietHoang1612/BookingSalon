using BookingSalon.Areas.Admin.Models.ViewModel;
using BookingSalon.Services.Interface;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace BookingSalon.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IStatisticalService _statisticalService;

        public DashboardController(IStatisticalService statisticalService)
        {
            _statisticalService = statisticalService;
        }

        public async Task<IActionResult> Index()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var today = DateTime.Now;

            ViewBag.CountBranch = await _statisticalService.GetNumberOfBranch();
            ViewBag.CountServiceActive = await _statisticalService.GetNumberOfServiceAsync();
            ViewBag.CountStylist = await _statisticalService.GetNumberOfStylist();
            ViewBag.CountCustomer = await _statisticalService.GetNumberOfCustomerAsync();
            ViewBag.CountReception = await _statisticalService.GetNumberOfReceptionAsync();
            ViewBag.CountSkinner = await _statisticalService.GetNumberOfSkinner();
            ViewBag.CountCoupon = await _statisticalService.GetNumberOfCoupon();
            ViewBag.CountRank = await _statisticalService.GetNumberOfRank();
            ViewBag.CountNews = await _statisticalService.GetNumberOfNews();
            ViewBag.TotalRevenueinMounth = await _statisticalService.TotalRevenueinMounth();
            var allBooking = await _statisticalService.GetAllBookingAsync();
            int countBooking = allBooking
                .Where(s => s.Date_Booking.Month == DateTime.Now.Month)
                .Count();
            ViewBag.CountBooking = countBooking;

            var viewModel = new StatisticalViewModel
            {
                StylistRevenues = (await _statisticalService.GetStaffRevenueReportAsync(startOfMonth, today, "Stylist")).ToList(),
                SkinnerRevenues = (await _statisticalService.GetStaffRevenueReportAsync(startOfMonth, today, "Skinner")).ToList(),
                ServiceRevenues = (await _statisticalService.GetServiceRevenueReportAsync(startOfMonth, today)).ToList()
            };

            viewModel.TotalRevenue = viewModel.ServiceRevenues.Sum(x => x.TotalRevenue);
            viewModel.TotalCompletedBookings = viewModel.StylistRevenues.Sum(x => x.TotalBookings);

            return View(viewModel);
        }

        public async Task<IActionResult> GetRevenueData(DateTime startDate, DateTime endDate)
        {
            var data = await _statisticalService.GetRevenueData(startDate, endDate);
            return Json(data);
        }

        public async Task<IActionResult> ExportStaffAndServiceExcel(DateTime startDate, DateTime endDate)
        {
            var stylists = await _statisticalService.GetStaffRevenueReportAsync(startDate, endDate, "Stylist");
            var skinners = await _statisticalService.GetStaffRevenueReportAsync(startDate, endDate, "Skinner");
            var services = await _statisticalService.GetServiceRevenueReportAsync(startDate, endDate);

            using (var workbook = new XLWorkbook())
            {
                AddStaffSheet(workbook, "Doanh Thu Stylist", stylists, startDate, endDate);

                AddStaffSheet(workbook, "Doanh Thu Skinner", skinners, startDate, endDate);

                var wsService = workbook.Worksheets.Add("Hiệu Suất Dịch Vụ");
                wsService.Cell(1, 1).Value = "THỐNG KÊ DỊCH VỤ PHỔ BIẾN";
                wsService.Range(1, 1, 1, 4).Merge().Style.Font.SetBold().Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                string[] svHeaders = { "Tên dịch vụ", "Số lần thực hiện", "Doanh thu thu về", "Tỷ trọng (%)" };
                for (int i = 0; i < svHeaders.Length; i++)
                {
                    wsService.Cell(3, i + 1).Value = svHeaders[i];
                    wsService.Cell(3, i + 1).Style.Font.Bold = true;
                    wsService.Cell(3, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                }

                int rowSv = 4;
                foreach (var sv in services)
                {
                    wsService.Cell(rowSv, 1).Value = sv.ServiceName;
                    wsService.Cell(rowSv, 2).Value = sv.UsageCount;
                    wsService.Cell(rowSv, 3).Value = sv.TotalRevenue;
                    wsService.Cell(rowSv, 3).Style.NumberFormat.Format = "#,##0";
                    wsService.Cell(rowSv, 4).Value = sv.Percentage / 100.0;
                    wsService.Cell(rowSv, 4).Style.NumberFormat.Format = "0.0%";
                    rowSv++;
                }
                wsService.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCao_Salon_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
        }

        private void AddStaffSheet(XLWorkbook workbook, string sheetName, IEnumerable<dynamic> data, DateTime startDate, DateTime endDate)
        {
            var ws = workbook.Worksheets.Add(sheetName);

            // Tiêu đề
            ws.Cell(1, 1).Value = $"{sheetName.ToUpper()} Từ ngày ({startDate:dd/MM/yyyy} đến ngày {endDate: dd/MM/yyyy} )";
            ws.Range(1, 1, 1, 9).Merge().Style.Font.SetBold().Font.SetFontSize(14).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Header
            string[] headers = { "Ngày", "Chi nhánh", "Mã NV", "Tên nhân viên", "Email", "Vai trò", "Số lịch đặt", "Tổng doanh thu", "TB/Ca" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(3, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#E0E0E0");
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Nội dung dữ liệu
            int row = 4;
            var sortedData = data.OrderBy(x => x.Date).ThenBy(x => x.BranchName);

            foreach (var s in sortedData)
            {
                ws.Cell(row, 1).Value = s.Date;
                ws.Cell(row, 1).Style.NumberFormat.Format = "dd/MM/yyyy";
                ws.Cell(row, 2).Value = s.BranchName;
                ws.Cell(row, 3).Value = s.StaffId;
                ws.Cell(row, 4).Value = s.StaffName;
                ws.Cell(row, 5).Value = s.Email;
                ws.Cell(row, 6).Value = s.RoleName;
                ws.Cell(row, 7).Value = s.TotalBookings;
                ws.Cell(row, 8).Value = s.TotalRevenue;
                ws.Cell(row, 8).Style.NumberFormat.Format = "#,##0";
                ws.Cell(row, 9).Value = s.AvgRevenuePerBooking;
                ws.Cell(row, 9).Style.NumberFormat.Format = "#,##0";
                row++;
            }

            // Dòng tổng cộng
            int lastRow = row;
            ws.Cell(lastRow, 1).Value = "TỔNG CỘNG";
            ws.Range(lastRow, 1, lastRow, 6).Merge().Style.Font.Bold = true;
            ws.Cell(lastRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.Cell(lastRow, 7).FormulaA1 = $"=SUM(G4:G{lastRow - 1})";
            ws.Cell(lastRow, 8).FormulaA1 = $"=SUM(H4:H{lastRow - 1})";

            // Định dạng dòng tổng
            ws.Range(lastRow, 7, lastRow, 9).Style.Font.Bold = true;
            ws.Cell(lastRow, 8).Style.NumberFormat.Format = "#,##0";

            ws.Columns().AdjustToContents();
        }

        [HttpGet]
        public async Task<IActionResult> GetTopStats(DateTime startDate, DateTime endDate)
        {
            var stylists = await _statisticalService.GetStaffRevenueReportAsync(startDate, endDate, "Stylist");
            var skinners = await _statisticalService.GetStaffRevenueReportAsync(startDate, endDate, "Skinner");
            var services = await _statisticalService.GetServiceRevenueReportAsync(startDate, endDate);

            return Json(new
            {
                stylists = stylists.Take(5),
                skinners = skinners.Take(5),
                services = services.Take(6)
            });
        }
    }
}
