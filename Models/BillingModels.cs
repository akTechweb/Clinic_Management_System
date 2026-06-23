using System;
using System.Collections.Generic;

namespace InfinityCoderzz_CMSV2026.Models
{
    public class UnbilledLabRequestViewModel
    {
        public int RequestId { get; set; }
        public string MMRCode { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public int TotalTests { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class LabBillViewModel
    {
        public int BillId { get; set; }
        public int RequestId { get; set; }
        public string MMRCode { get; set; }
        public string PatientName { get; set; }
        public string MobileNumber { get; set; }
        public DateTime BillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class LabBillDetailsViewModel
    {
        public LabBillViewModel Bill { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public List<LabBillLineItemViewModel> Items { get; set; } = new();
    }

    public class LabBillLineItemViewModel
    {
        public string TestName { get; set; }
        public decimal Amount { get; set; }
    }
}
