using InfinityCoderzz_CMSV2026.Models.pharmacist;
using InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories;

namespace InfinityCoderzz_CMSV2026.Services.PharmacyServices
{
  
        public class MedicineServiceImpl : IMedicineService
        {
            // Field
            private readonly IMedicineRepository _medicineRepository;

            // Dependency Injection
            public MedicineServiceImpl(
                IMedicineRepository medicineRepository)
            {
                _medicineRepository = medicineRepository;
            }

            #region Get All Medicines

            public IEnumerable<Medicine> GetAllMedicines()
            {
                return _medicineRepository.GetAllMedicines();
            }

            #endregion

            #region Search Medicine

            public IEnumerable<Medicine> SearchMedicine(
                string searchTerm)
            {
                return _medicineRepository
                    .SearchMedicine(searchTerm);
            }

            #endregion

            #region Get Medicine By Id

            public Medicine GetMedicineById(
                int medicineId)
            {
                return _medicineRepository
                    .GetMedicineById(medicineId);
            }

            #endregion

            #region Add Medicine

            public void AddMedicine(
                Medicine medicine)
            {
                _medicineRepository
                    .AddMedicine(medicine);
            }

            #endregion

            #region Update Medicine

            public void UpdateMedicine(
                Medicine medicine)
            {
                _medicineRepository
                    .UpdateMedicine(medicine);
            }

            #endregion

            #region Disable Medicine

            public void DisableMedicine(
                int medicineId)
            {
                _medicineRepository
                    .DisableMedicine(medicineId);
            }

            #endregion

            #region Get All Categories

            public IEnumerable<MedicineCategory>
                GetAllCategories()
            {
                return _medicineRepository
                    .GetAllCategories();
            }

            #endregion

            #region Get All Manufacturers

            public IEnumerable<Manufacturer>
                GetAllManufacturers()
            {
                return _medicineRepository
                    .GetAllManufacturers();
            }

            #endregion

            #region Medicine Code Generation

            public string GenerateNextMedicineCode()
            {
                const string prefix = "MED-";
                int max = 0;

                foreach (var medicine in _medicineRepository.GetAllMedicines())
                {
                    var code = medicine.MedicineCode;
                    if (string.IsNullOrWhiteSpace(code)) continue;
                    if (!code.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;

                    if (int.TryParse(code.Substring(prefix.Length), out int number) && number > max)
                        max = number;
                }

                return prefix + (max + 1).ToString("D6");
            }

            public bool IsMedicineCodeUnique(string medicineCode)
            {
                if (string.IsNullOrWhiteSpace(medicineCode)) return false;

                return !_medicineRepository.GetAllMedicines()
                    .Any(m => string.Equals(m.MedicineCode?.Trim(), medicineCode.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            #endregion
        }
    


}

