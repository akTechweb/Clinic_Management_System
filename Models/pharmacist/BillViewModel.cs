namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
   
        public class BillViewModel
        {
            public int BillId { get; set; }

            public int PatientId { get; set; }

            public DateTime BillDate { get; set; }

            public decimal TotalAmount { get; set; }

            public string? Status { get; set; }

            public string? PatientCode { get; set; }

            public string? PatientName { get; set; }
    }
    
}

