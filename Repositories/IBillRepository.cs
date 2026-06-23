using InfinityCoderzz_CMSV2026.Models;

namespace InfinityCoderzzz_CMSV2026.Repositories
{
    public interface IBillRepository
    {
        // List all bills
        List<Bill> GetAllBills();

        // Get bill with items and payments by ID
        Bill GetBillById(int billId);

        // Generate a new bill (calls sp_GenerateBill)
        void GenerateBill(int patientId, int appointmentId, out int billId, out decimal totalAmount, out string message);

        // Process payment for a bill (calls sp_ProcessPayment)
        void ProcessPayment(Payment payment, out string message);
    }
}
