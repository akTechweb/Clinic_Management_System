using InfinityCoderzz_CMSV2026.Models;

namespace InfinityCoderzzz_CMSV2026.Services
{
    public interface IBillService
    {
        List<Bill> GetAllBills();

        Bill GetBillById(int billId);

        void GenerateBill(int patientId, int appointmentId, out int billId, out decimal totalAmount, out string message);

        void ProcessPayment(Payment payment, out string message);
    }
}