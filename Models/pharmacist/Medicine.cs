using System.ComponentModel.DataAnnotations;

namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
 
        
        public class Medicine
        {
            public int MedicineId { get; set; }

            [Required(ErrorMessage = "Medicine Code is required")]
            public string MedicineCode { get; set; }

            [Required(ErrorMessage = "Medicine Name is required")]
            public string MedicineName { get; set; }

            public string? GenericName { get; set; }

            [Required(ErrorMessage = "Category is required")]
            public int CategoryId { get; set; }

            public string? CategoryName { get; set; }

            [Required(ErrorMessage = "Manufacturer is required")]
            public int ManufacturerId { get; set; }

            public string? ManufacturerName { get; set; }

            public string? Unit { get; set; }

            [Required(ErrorMessage = "Unit Price is required")]
            [Range(0.01, 999999)]
            public decimal UnitPrice { get; set; }

            [Range(0, 9999)]
            public int ReorderLevel { get; set; }

            public bool IsActive { get; set; }
        }
    
}
