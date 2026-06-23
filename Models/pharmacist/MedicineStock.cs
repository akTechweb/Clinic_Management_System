using System.ComponentModel.DataAnnotations;

namespace InfinityCoderzz_CMSV2026.Models.pharmacist
{
    public class MedicineStock : IValidatableObject
    {
        public int StockId { get; set; }

        [Required(ErrorMessage = "Please select a medicine.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a medicine.")]
        public int MedicineId { get; set; }

        public string? MedicineName { get; set; }

        // NOTE: Character-format (regex) validation for batch numbers is enforced only on
        // the Create path (controller) so that editing pre-existing/legacy batches whose
        // values may contain other characters is never blocked. Length matches DB varchar(50).
        [Required(ErrorMessage = "Batch number is required.")]
        [StringLength(50, ErrorMessage = "Batch number cannot exceed 50 characters.")]
        public string BatchNumber { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 1000000, ErrorMessage = "Quantity must be between 1 and 1,000,000.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Purchase price is required.")]
        [Range(typeof(decimal), "0.01", "99999999.99",
            ErrorMessage = "Purchase price must be between ₹0.01 and ₹99,999,999.99.")]
        public decimal? PurchasePrice { get; set; }

        [Required(ErrorMessage = "Expiry date is required.")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PurchaseDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public int DaysRemaining { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            DateTime today = DateTime.Today;

            // Typo guard: expiry too far in the future (applies to both create and edit).
            // The "expiry must be in the future" rule is create-only and is enforced in the
            // controller's Create action, so already-expired batches can still be edited.
            if (ExpiryDate != default && ExpiryDate.Date > today.AddYears(20))
            {
                yield return new ValidationResult(
                    "Expiry date is unrealistically far in the future.",
                    new[] { nameof(ExpiryDate) });
            }

            if (PurchaseDate.HasValue)
            {
                if (PurchaseDate.Value.Date > today)
                {
                    yield return new ValidationResult(
                        "Purchase date cannot be in the future.",
                        new[] { nameof(PurchaseDate) });
                }

                if (PurchaseDate.Value.Date < new DateTime(2000, 1, 1))
                {
                    yield return new ValidationResult(
                        "Purchase date is too far in the past.",
                        new[] { nameof(PurchaseDate) });
                }

                if (ExpiryDate != default && ExpiryDate.Date <= PurchaseDate.Value.Date)
                {
                    yield return new ValidationResult(
                        "Expiry date must be after the purchase date.",
                        new[] { nameof(ExpiryDate) });
                }
            }
        }
    }
}
