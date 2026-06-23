using InfinityCoderzz_CMSV2026.Models.pharmacist;

namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
   

    
    
        public interface IMedicineRepository
        {
            IEnumerable<Medicine> GetAllMedicines();

            IEnumerable<Medicine> SearchMedicine(string searchTerm);

            Medicine GetMedicineById(int medicineId);

            void AddMedicine(Medicine medicine);

            void UpdateMedicine(Medicine medicine);

            void DisableMedicine(int medicineId);

           IEnumerable<MedicineCategory> GetAllCategories();

           IEnumerable<Manufacturer> GetAllManufacturers();

      

          // IEnumerable<MedicineCategory> GetAllMedicineCategories();

      


    }
    
}
