using InfinityCoderzz_CMSV2026.Models;
using InfinityCoderzz_CMSV2026.Repositories;
using InfinityCoderzz_CMSV2026.Services;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace InfinityCoderzz_CMSV2026.Services
{
    public class LabTechnicianService : ILabTechnicianService
    {
        private readonly ILabTechnicianRepository _repo;
        private readonly IConfiguration _config;

        public LabTechnicianService(ILabTechnicianRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        public Task<LabDashboardStatsViewModel> GetDashboardStatsAsync() => _repo.GetDashboardStatsAsync();
        public Task<List<PendingLabRequestViewModel>> GetPendingLabRequestsAsync(string s) => _repo.GetPendingLabRequestsAsync(s);
        public Task<List<LabPatientSearchViewModel>> SearchPatientByMMRAsync(string s) => _repo.SearchPatientByMMRAsync(s);
        public Task<EnterLabResultViewModel> GetLabRequestItemDetailsAsync(int id) => _repo.GetLabRequestItemDetailsAsync(id);
        public Task<int> SaveLabResultAsync(EnterLabResultViewModel m, int tid) => _repo.AddLabResultAsync(m, tid);
        public Task<List<CompletedLabReportViewModel>> GetCompletedLabReportsAsync(string s) => _repo.GetCompletedLabReportsAsync(s);
        public Task<LabResultPdfViewModel> GetLabResultDetailsAsync(int id) => _repo.GetLabResultDetailsAsync(id);
        public Task<List<UnbilledLabRequestViewModel>> GetUnbilledLabRequestsAsync(string s) => _repo.GetUnbilledLabRequestsAsync(s);
        public Task<int> GenerateLabBillAsync(int rid, int tid) => _repo.GenerateLabBillAsync(rid, tid);
        public Task<List<LabBillViewModel>> GetLabBillsAsync(string s) => _repo.GetLabBillsAsync(s);
        public Task<LabBillDetailsViewModel> GetLabBillDetailsAsync(int id) => _repo.GetLabBillDetailsAsync(id);
        public Task UpdateLabBillPaymentStatusAsync(int id, string s) => _repo.UpdateLabBillPaymentStatusAsync(id, s);

        // ── GENERATE BILL PDF ─────────────────────────────────────────────────
        public async Task<System.IO.MemoryStream> GenerateBillPdfAsync(int billId)
        {
            var vm = await _repo.GetLabBillDetailsAsync(billId);
            if (vm == null || vm.Bill == null)
                throw new Exception($"Bill #{billId} not found.");
            return await BuildBillPdfAsync(vm);
        }

        private async Task<System.IO.MemoryStream> BuildBillPdfAsync(LabBillDetailsViewModel vm)
        {
            return await Task.Run(() =>
            {
                var ms = new System.IO.MemoryStream();
                var b = vm.Bill;

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(35);

                        // ── HEADER ────────────────────────────────────────────
                        page.Header().Column(col =>
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text(t =>
                                    {
                                        t.Span("INFINITY CLINIC MANAGEMENT SYSTEM")
                                            .Bold().FontSize(16).FontColor(Colors.Blue.Darken3);
                                    });
                                    c.Item().Text(t =>
                                    {
                                        t.Span("Lab Bill Invoice")
                                            .FontSize(11).FontColor(Colors.Grey.Darken1);
                                    });
                                });
                                row.ConstantItem(170).AlignRight().Column(c =>
                                {
                                    c.Item().Text(t =>
                                    {
                                        t.Span($"Bill # {b.BillId}")
                                            .Bold().FontSize(14).FontColor(Colors.Blue.Darken2);
                                    });
                                    c.Item().Text(t =>
                                    {
                                        t.Span($"Date: {b.BillDate:dd-MM-yyyy HH:mm}")
                                            .FontSize(10).FontColor(Colors.Grey.Darken1);
                                    });
                                });
                            });
                            col.Item().PaddingTop(4).LineHorizontal(2).LineColor(Colors.Blue.Darken2); 
                        });

                        // ── CONTENT ───────────────────────────────────────────
                        page.Content().PaddingVertical(16).Column(col =>
                        {
                            col.Spacing(12);

                            // Patient info box
                            col.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(info =>
                            {
                                info.Spacing(6);
                                info.Item().Row(row =>
                                {
                                    row.RelativeItem().Text(t =>
                                    {
                                        t.Span("MMR Code: ").Bold();
                                        t.Span(b.MMRCode ?? "");
                                    });
                                    row.RelativeItem().Text(t =>
                                    {
                                        t.Span("Patient: ").Bold();
                                        t.Span(b.PatientName ?? "");
                                    });
                                });
                                info.Item().Row(row =>
                                {
                                    row.RelativeItem().Text(t =>
                                    {
                                        t.Span("Age / Gender: ").Bold();
                                        t.Span($"{vm.Age} yrs / {vm.Gender}");
                                    });
                                    row.RelativeItem().Text(t =>
                                    {
                                        t.Span("Mobile: ").Bold();
                                        t.Span(b.MobileNumber ?? "");
                                    });
                                });
                                info.Item().Row(row =>
                                {
                                    row.RelativeItem().Text(t =>
                                    {
                                        t.Span("Payment Status: ").Bold();
                                        var statusColor = (b.PaymentStatus == "Paid")
                                            ? Colors.Green.Darken2
                                            : Colors.Orange.Darken2;
                                        t.Span(b.PaymentStatus ?? "").FontColor(statusColor);
                                    });
                                });
                            });

                            // Section heading
                            col.Item().Text(t =>
                            {
                                t.Span("Billed Tests").Bold().FontSize(13);
                            });

                            // Tests table
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(4);
                                    c.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Blue.Darken2).Padding(7).Text(t =>
                                    {
                                        t.Span("Test Name").Bold().FontColor(Colors.White);
                                    });
                                    header.Cell().Background(Colors.Blue.Darken2).Padding(7).AlignRight().Text(t =>
                                    {
                                        t.Span("Amount (Rs.)").Bold().FontColor(Colors.White);
                                    });
                                });

                                bool alt = false;
                                foreach (var item in vm.Items)
                                {
                                    var bg = alt ? Colors.Grey.Lighten4 : Colors.White;
                                    table.Cell().Background(bg).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(7).Text(item.TestName ?? "");
                                    table.Cell().Background(bg).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(7).AlignRight().Text($"{item.Amount:0.00}");
                                    alt = !alt;
                                }

                                // Total row
                                table.Cell().Background(Colors.Grey.Lighten2).BorderTop(1).BorderColor(Colors.Grey.Darken1).Padding(7).Text(t =>
                                {
                                    t.Span("TOTAL").Bold();
                                });
                                table.Cell().Background(Colors.Grey.Lighten2).BorderTop(1).BorderColor(Colors.Grey.Darken1).Padding(7).AlignRight().Text(t =>
                                {
                                    t.Span($"Rs. {b.TotalAmount:0.00}").Bold().FontColor(Colors.Green.Darken2);
                                });
                            });

                            // Signature
                            col.Item().PaddingTop(30).Row(row =>
                            {
                                row.RelativeItem();
                                row.ConstantItem(180).Column(c =>
                                {
                                    c.Item().LineHorizontal(1).LineColor(Colors.Grey.Darken1);
                                    c.Item().PaddingTop(4).AlignCenter().Text(t =>
                                    {
                                        t.Span("Authorised Signatory").FontSize(10).Bold();
                                    });
                                });
                            });
                        });

                        // ── FOOTER ────────────────────────────────────────────
                        page.Footer().AlignCenter().Text(t =>
                        {
                            t.Span("Infinity Clinic Management System 2026  •  Generated: ")
                                .FontSize(9).FontColor(Colors.Grey.Darken1);
                            t.Span(DateTime.Now.ToString("dd-MM-yyyy HH:mm"))
                                .Bold().FontSize(9).FontColor(Colors.Grey.Darken1);
                        });
                    });
                });

                document.GeneratePdf(ms);
                ms.Position = 0;
                return ms;
            });
        }

        // ── SEND RESULT TO DOCTOR ─────────────────────────────────────────────
        public async Task<bool> SendResultToDoctorAsync(int resultId)
        {
            var result = await _repo.GetLabResultDetailsAsync(resultId);
            if (result == null || string.IsNullOrWhiteSpace(result.DoctorEmail))
            {
                await _repo.AddReportNotificationAsync(resultId, result?.DoctorEmail ?? "", "Doctor", "Failed");
                return false;
            }

            try
            {
                var smtp = _config.GetSection("SmtpSettings");
                string host = smtp["Host"] ?? "smtp.gmail.com";
                int port = int.TryParse(smtp["Port"], out var p) ? p : 587;
                string fromEmail = smtp["FromEmail"] ?? "";
                string fromName = smtp["FromName"] ?? "Infinity Clinic Lab";
                string password = smtp["Password"] ?? "";
                bool enableSsl = !bool.TryParse(smtp["EnableSsl"], out var ssl) || ssl;

                using var pdfStream = await BuildResultPdfAsync(result);
                using var mail = new MailMessage();
                mail.From = new MailAddress(fromEmail, fromName);
                mail.To.Add(result.DoctorEmail);
                mail.Subject = $"Lab Report — {result.PatientName} (MMR: {result.MMRCode}) — {result.TestName}";
                mail.IsBodyHtml = true;
                mail.Body = $@"<p>Dear Dr. {result.DoctorName},</p>
                    <p>Lab result for <strong>{result.PatientName}</strong> (MMR: {result.MMRCode}) is ready.</p>
                    <table style='font-size:13px;border-collapse:collapse;'>
                        <tr><td style='padding:4px 10px;'><b>Test:</b></td><td>{result.TestName}</td></tr>
                        <tr><td style='padding:4px 10px;'><b>Result:</b></td><td>{result.ResultValue}</td></tr>
                        <tr><td style='padding:4px 10px;'><b>Normal Range:</b></td><td>{result.NormalRange}</td></tr>
                        <tr><td style='padding:4px 10px;'><b>Status:</b></td><td>{(result.IsAbnormal ? "ABNORMAL" : "Normal")}</td></tr>
                    </table>
                    <p>Please find the detailed report attached.</p>";
                mail.Attachments.Add(new Attachment(pdfStream, $"LabReport_{result.MMRCode}_{result.ResultId}.pdf", "application/pdf"));

                using var smtpClient = new SmtpClient(host, port);
                smtpClient.Credentials = new NetworkCredential(fromEmail, password);
                smtpClient.EnableSsl = enableSsl;
                await smtpClient.SendMailAsync(mail);

                await _repo.AddReportNotificationAsync(resultId, result.DoctorEmail, "Doctor", "Sent");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EMAIL ERROR: {ex.Message}");
                // ADD THIS LINE so TempData shows the real error:
                // Store ex.Message somewhere accessible, OR just log it properly
                await _repo.AddReportNotificationAsync(resultId, result.DoctorEmail, "Doctor", $"Failed: {ex.Message}");
                return false;
            }
        }

        // ── BUILD LAB RESULT PDF ──────────────────────────────────────────────
        private async Task<System.IO.MemoryStream> BuildResultPdfAsync(LabResultPdfViewModel r)
        {
            return await Task.Run(() =>
            {
                var ms = new System.IO.MemoryStream();

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);

                        page.Header().Column(col =>
                        {
                            col.Item().Text(t =>
                            {
                                t.Span("INFINITY CLINIC MANAGEMENT SYSTEM").Bold().FontSize(16);
                            });
                            col.Item().Text(t =>
                            {
                                t.Span($"Lab Report — Generated {DateTime.Now:dd-MM-yyyy HH:mm}")
                                    .FontSize(10).FontColor(Colors.Grey.Darken1);
                            });
                        });

                        page.Content().PaddingVertical(15).Column(col =>
                        {
                            col.Spacing(8);
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text(t =>
                                {
                                    t.Span($"MMR Code: {r.MMRCode}").Bold();
                                });
                                row.RelativeItem().Text(t =>
                                {
                                    t.Span($"Patient: {r.PatientName}").Bold();
                                });
                            });
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"Age / Gender: {r.Age} yrs / {r.Gender}");
                                row.RelativeItem().Text($"Mobile: {r.MobileNumber}");
                            });

                            col.Item().PaddingTop(10).Text(t =>
                            {
                                t.Span("Test Result").Bold().FontSize(13);
                            });

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(2);
                                    c.RelativeColumn(3);
                                });

                                void AddRow(string label, string value, bool highlight = false)
                                {
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(t =>
                                    {
                                        t.Span(label).Bold();
                                    });
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(t =>
                                    {
                                        var span = t.Span(value ?? "");
                                        if (highlight) span.FontColor(Colors.Red.Medium);
                                    });
                                }

                                AddRow("Test Name", r.TestName);
                                AddRow("Description", r.TestDescription);
                                AddRow("Normal Range", r.NormalRange);
                                AddRow("Result Value", r.ResultValue, r.IsAbnormal);
                                AddRow("Observation", r.Observation);
                                AddRow("Remarks", r.Remarks);
                                AddRow("Status", r.IsAbnormal ? "ABNORMAL" : "Normal", r.IsAbnormal);
                                AddRow("Result Date", r.ResultDate.ToString("dd-MM-yyyy HH:mm"));
                                AddRow("Referring Doctor", $"Dr. {r.DoctorName}");
                                AddRow("Lab Technician", r.TechnicianName);
                            });
                        });

                        page.Footer().AlignRight().Text(t =>
                        {
                            t.Span("Authorised Lab Signatory").FontSize(10);
                        });
                    });
                });

                document.GeneratePdf(ms);
                ms.Position = 0;
                return ms;
            });
        }
    }
}
