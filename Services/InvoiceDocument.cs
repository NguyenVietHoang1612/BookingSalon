using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using BookingSalon.Models.ViewModel.BookingSalonViewModel; // Đảm bảo đúng namespace của bạn

public class InvoiceDocument : IDocument
{
    public BookingDetailViewModel Model { get; }

    public InvoiceDocument(BookingDetailViewModel model) => Model = model;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {

            page.Size(300, 500);
            page.Margin(10);
            page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Verdana));

            // Phần Đầu hóa đơn (Header)
            page.Header().Column(col =>
            {
                col.Item().Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(3);
                    });

                    t.Cell().Text("LUXURY SALON").Bold().FontSize(12);
                    t.Cell().Text("HOÁ ĐƠN DỊCH VỤ").Bold().FontSize(10);

                    t.Cell().PaddingVertical(2).Text($"Chi nhánh: {Model.Branch_Name}").FontSize(8);
                    t.Cell().PaddingVertical(2).Text($"Số HD: {Model.BookingId}").FontSize(8);

                    t.Cell().PaddingVertical(2).Text($"ĐC: {Model.Branch_Address}").FontSize(8);
                    t.Cell().PaddingVertical(2).Text($"Ngày: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8);

                    t.Cell().PaddingVertical(2).Text($"SĐT: {Model.Branch_Phone}").FontSize(8);


                });
                col.Item().Text("LUXURY SALON").Bold().FontSize(14).AlignCenter();
                col.Item().Text($"{Model.Branch_Name}").AlignCenter();
                col.Item().Text($"ĐC: {Model.Branch_Address}").FontSize(8).AlignCenter();
                col.Item().Text($"SĐT: {Model.Branch_Phone}").FontSize(8).AlignCenter();

                col.Item().PaddingVertical(5).LineHorizontal(1);

                col.Item().Text("HOÁ ĐƠN BÁN HÀNG").Bold().AlignCenter();
                col.Item().Text($"Số HD: HD{Model.BookingId}").FontSize(8).AlignCenter();
                col.Item().Text($"Ngày: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).AlignCenter();
            });

            // Phần Nội dung (Content)
            page.Content().PaddingVertical(10).Column(col =>
            {
                col.Item().Text($"Khách hàng: {Model.Customer_Name}");
                col.Item().Text($"Số điện thoại: {Model.Customer_Phone}");
                col.Item().Text($"Stylist thực hiện: {Model.Stylist_Name}");
                col.Item().Text($"Skinner thực hiện: {Model.Skinner_Name}");


                col.Item().PaddingVertical(5).LineHorizontal(0.5f);

                // Bảng hiển thị SL 
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); 
                        columns.RelativeColumn(1); 
                        columns.RelativeColumn(2); 
                    });

                    // Tiêu đề bảng
                    table.Cell().Text("Dịch vụ").Bold().FontSize(8);
                    table.Cell().AlignCenter().Text("SL").Bold().FontSize(8);
                    table.Cell().AlignRight().Text("T.Tiền").Bold().FontSize(8);

                    if (Model.bookingDetails != null)
                    {
                        foreach (var detail in Model.bookingDetails)
                        {
                            table.Cell().PaddingVertical(2).Text(detail.Service?.Service_Name).FontSize(8);
                            table.Cell().PaddingVertical(2).AlignCenter().Text("1").FontSize(8);
                            table.Cell().PaddingVertical(2).AlignRight().Text($"" +
                                $"{detail.BasePriceSnapshot:#,##0} đ").FontSize(8);
                        }
                    }
                });

                col.Item().PaddingVertical(5).LineHorizontal(0.5f);

                // Phần Tổng kết (Totals)
                col.Item().AlignRight().Width(150).Column(c =>
                {
                    void BuildTotalRow(string label, string value, bool isBold = false, float fontSize = 9)
                    {
                        c.Item().Row(row =>
                        {
                            row.RelativeItem().Text(label).FontSize(fontSize);
                            row.ConstantItem(70).AlignRight().Text(value).FontSize(fontSize).Bold();
                        });
                    }

                    BuildTotalRow("Tổng tiền hàng:", $"{Model.TotalPrice:#,##0} đ");
                    BuildTotalRow("Giảm giá:", $"{Model.DiscountAmount:#,##0} đ");

                    c.Item().PaddingTop(2).LineHorizontal(0.5f); 

                    BuildTotalRow("TỔNG CỘNG:", $"{Model.FinalPrice:#,##0} đ", true, 11);
                });
            });

            // Chân trang (Footer)
            page.Footer().PaddingTop(10).Column(col => {
                col.Item().AlignCenter().Text("Quý khách vui lòng kiểm tra lại hóa đơn").Italic().FontSize(7);
                col.Item().AlignCenter().Text("Cảm ơn và hẹn gặp lại!").FontSize(8);
            });
        });
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
}