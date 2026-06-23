namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
        public class Prescription
        {
            public int PrescriptionId { get; set; }

            public int PatientId { get; set; }

            public string? PatientName { get; set; }

            public int DoctorId { get; set; }

            public string? DoctorName { get; set; }

            public DateTime PrescriptionDate { get; set; }

            public string? Remarks { get; set; }

            public string? Status { get; set; }
        }
    
}
