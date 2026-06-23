using InfinityCoderzz_CMSV2026.Models;
using InfinityCoderzzz_CMSV2026.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfinityCoderzz_CMSV2026.Controllers
{
    public class BillsController : Controller
    {
        private readonly IBillService _billService;
        private readonly IAppointmentService _appointmentService;

        public BillsController(
            IBillService billService,
            IAppointmentService appointmentService)
        {
            _billService = billService;
            _appointmentService = appointmentService;
        }

        public IActionResult Index()
        {
            List<Bill> bills = _billService.GetAllBills();
            return View(bills);
        }

        [HttpGet]
        public IActionResult Create(int appointmentId)
        {
            if (appointmentId <= 0)
            {
                TempData["ErrorMessage"] = "Please select a booked appointment before billing.";
                return RedirectToAction("Index", "Appointments");
            }

            Appointment appointment = _appointmentService.GetAppointmentById(appointmentId);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Appointment not found. Bill cannot be generated.";
                return RedirectToAction("Index", "Appointments");
            }

            if (appointment.Status == "Cancelled")
            {
                TempData["ErrorMessage"] = "Cancelled appointment cannot be billed.";
                return RedirectToAction("Details", "Appointments", new { id = appointmentId });
            }

            ViewBag.Appointment = appointment;

            return View(new Bill
            {
                PatientId = appointment.PatientId,
                AppointmentId = appointment.AppointmentId,
                PatientName = appointment.PatientName,
                PatientCode = appointment.PatientCode,
                DoctorName = appointment.DoctorName,
                AppointmentNumber = appointment.AppointmentNumber
            });
        }

        [HttpPost]
        public IActionResult Create(
    int patientId,
    int appointmentId,
    string paymentMethod,
    decimal amountReceived)
        {
            _billService.GenerateBill(
                patientId,
                appointmentId,
                out int billId,
                out decimal totalAmount,
                out string message);

            if (!string.IsNullOrWhiteSpace(message) &&
                (message.StartsWith("SUCCESS") || message.StartsWith("INFO")))
            {
                if (amountReceived > 0)
                {
                    Payment payment = new Payment
                    {
                        BillId = billId,
                        Amount = amountReceived,
                        PaymentMethod = paymentMethod
                    };

                    _billService.ProcessPayment(payment, out string payMessage);
                    TempData["SuccessMessage"] = "Bill generated and payment received successfully.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Bill generated successfully.";
                }

                return RedirectToAction("Details", new { id = billId });
            }

            TempData["ErrorMessage"] = message;
            return RedirectToAction("Create", new { appointmentId });
        }

        public IActionResult Details(int id)
        {
            Bill bill = _billService.GetBillById(id);

            if (bill == null)
                return NotFound();

            return View(bill);
        }

        [HttpPost]
        public IActionResult ReceivePayment(int billId, string paymentMethod)
        {
            Bill bill = _billService.GetBillById(billId);

            if (bill == null)
                return NotFound();

            if (bill.Status == "Paid")
            {
                TempData["SuccessMessage"] = "Bill is already fully paid.";
                return RedirectToAction("Details", new { id = billId });
            }

            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                TempData["ErrorMessage"] = "Please select a payment method.";
                return RedirectToAction("Details", new { id = billId });
            }

            decimal paidAmount = bill.Payments?
                .Where(p => p.PaymentStatus == "Completed")
                .Sum(p => p.Amount) ?? 0;

            decimal remainingAmount = bill.TotalAmount - paidAmount;

            if (remainingAmount <= 0)
            {
                TempData["SuccessMessage"] = "Bill is already fully paid.";
                return RedirectToAction("Details", new { id = billId });
            }

            Payment payment = new Payment
            {
                BillId = billId,
                Amount = remainingAmount,
                PaymentMethod = paymentMethod,
                TransactionReference = null
            };

            _billService.ProcessPayment(payment, out string message);

            if (!string.IsNullOrWhiteSpace(message) &&
                message.StartsWith("SUCCESS"))
            {
                TempData["SuccessMessage"] = "Payment received successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction("Details", new { id = billId });
        }
    }
}