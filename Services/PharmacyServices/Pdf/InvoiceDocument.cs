using InfinityCoderzz_CMSV2026.Models.pharmacist;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using InfinityCoderzz_CMSV2026.Helpers;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices.Pdf
{
    public class InvoiceDocument : IDocument
    {
        private readonly BillViewModel _bill;
        private readonly List<BillItemViewModel> _items;

        public InvoiceDocument(BillViewModel bill, IEnumerable<BillItemViewModel> items)
        {
            _bill = bill;
            _items = items.ToList();
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(36);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(t => t.FontSize(10).FontColor("#1e293b"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Infinity Clinic Pharmacy — Computer generated invoice. ").FontSize(8).FontColor("#94a3b8");
                    t.Span("Page ").FontSize(8).FontColor("#94a3b8");
                    t.CurrentPageNumber().FontSize(8).FontColor("#94a3b8");
                    t.Span(" / ").FontSize(8).FontColor("#94a3b8");
                    t.TotalPages().FontSize(8).FontColor("#94a3b8");
                });
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Infinity Clinic").FontSize(20).Bold().FontColor("#16a34a");
                        c.Item().Text("Pharmacy Department").FontSize(10).FontColor("#64748b");
                    });
                    row.ConstantItem(180).Column(c =>
                    {
                        c.Item().AlignRight().Text("INVOICE").FontSize(18).Bold();
                        c.Item().AlignRight().Text($"Bill No: {RefNo.Bill(_bill.BillId)}").FontSize(10);
                        c.Item().AlignRight().Text($"Date: {_bill.BillDate:dd MMM yyyy hh:mm tt}").FontSize(10);
                        c.Item().AlignRight().Text($"Status: {_bill.Status}").FontSize(10).FontColor("#64748b");
                    });
                });
                col.Item().PaddingTop(8).LineHorizontal(1).LineColor("#e2e8f0");
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(12).Column(col =>
            {
                col.Item().PaddingBottom(10).Column(c =>
                {
                    c.Item().Text("Billed To").FontSize(9).Bold().FontColor("#64748b");
                    c.Item().Text(_bill.PatientName ?? $"Patient #{_bill.PatientId}").FontSize(12).Bold();
                    if (!string.IsNullOrWhiteSpace(_bill.PatientCode))
                        c.Item().Text($"Patient Code: {_bill.PatientCode}").FontSize(9).FontColor("#64748b");
                });

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);
                        columns.RelativeColumn();
                        columns.ConstantColumn(60);
                        columns.ConstantColumn(80);
                        columns.ConstantColumn(80);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("#");
                        header.Cell().Element(HeaderCell).Text("Medicine");
                        header.Cell().Element(HeaderCell).AlignCenter().Text("Qty");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Unit Price");
                        header.Cell().Element(HeaderCell).AlignRight().Text("Amount");
                    });

                    int idx = 1;
                    foreach (var item in _items)
                    {
                        table.Cell().Element(BodyCell).Text(idx.ToString());
                        table.Cell().Element(BodyCell).Text(item.MedicineName ?? "Medicine");
                        table.Cell().Element(BodyCell).AlignCenter().Text(item.Quantity.ToString());
                        table.Cell().Element(BodyCell).AlignRight().Text($"₹{item.UnitPrice:N2}");
                        table.Cell().Element(BodyCell).AlignRight().Text($"₹{item.Amount:N2}");
                        idx++;
                    }
                });

                col.Item().PaddingTop(10).AlignRight().Column(c =>
                {
                    c.Item().Row(row =>
                    {
                        row.ConstantItem(120).Text("Total Amount").Bold();
                        row.ConstantItem(90).AlignRight().Text($"₹{_bill.TotalAmount:N2}").Bold().FontColor("#16a34a");
                    });
                });
            });
        }

        private static IContainer HeaderCell(IContainer container)
            => container.Background("#f0fdf4").PaddingVertical(6).PaddingHorizontal(6)
                        .BorderBottom(1).BorderColor("#bbf7d0").DefaultTextStyle(t => t.Bold().FontSize(9));

        private static IContainer BodyCell(IContainer container)
            => container.PaddingVertical(5).PaddingHorizontal(6).BorderBottom(1).BorderColor("#f1f5f9");
    }
}
