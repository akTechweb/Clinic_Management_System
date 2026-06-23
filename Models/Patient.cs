using System;
using System.ComponentModel.DataAnnotations;

namespace InfinityCoderzz_CMSV2026.Models
{
    public class Patient
    {
        public int PatientId { get; set; }

        public string? PatientCode { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters.")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Full name should contain only letters and spaces.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        public string? BloodGroup { get; set; } = "Unknown";

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^[6-9][0-9]{9}$", ErrorMessage = "Mobile number must be 10 digits and start with 6, 7, 8, or 9.")]
        public string? MobileNumber { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? Email { get; set; }

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string? Address { get; set; }

        [RegularExpression(@"^[6-9][0-9]{9}$", ErrorMessage = "Emergency contact must be 10 digits and start with 6, 7, 8, or 9.")]
        public string? EmergencyContactNumber { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public bool IsActive { get; set; }

        public int? Age
        {
            get
            {
                if (DOB == null) return null;

                var today = DateTime.Today;
                int age = today.Year - DOB.Value.Year;

                if (DOB.Value.Date > today.AddYears(-age))
                    age--;

                return age;
            }
        }
    }
}