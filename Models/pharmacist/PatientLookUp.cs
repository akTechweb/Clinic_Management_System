namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
    public class PatientLookup
    {
        public int PatientId { get; set; }

        public string? PatientCode { get; set; }

        public string? FullName { get; set; }

        public string DisplayText
        {
            get
            {
                return $"{PatientCode} - {FullName}";
            }
        }
    }
}