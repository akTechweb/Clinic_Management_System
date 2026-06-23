using InfinityCoderzz_CMSV2026.Models;
using InfinityCoderzzz_CMSV2026.Repositories;

namespace InfinityCoderzzz_CMSV2026.Services
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;

        public BillService(IBillRepository billRepository)
        {
            _billRepository = billRepository;
        }

        #region Get All Bills

        public List<Bill> GetAllBills()
        {
            return _billRepository.GetAllBills();
        }

        #endregion

        #region Get Bill By ID

        public Bill GetBillById(int billId)
        {
            return _billRepository.GetBillById(billId);
        }

        #endregion

        #region Generate Bill

        public void GenerateBill(int patientId, int appointmentId, out int billId, out decimal totalAmount, out string message)
        {
            _billRepository.GenerateBill(patientId, appointmentId, out billId, out totalAmount, out message);
        }

        #endregion

        #region Process Payment

        public void ProcessPayment(Payment payment, out string message)
        {
            _billRepository.ProcessPayment(payment, out message);
        }

        #endregion
    }
}