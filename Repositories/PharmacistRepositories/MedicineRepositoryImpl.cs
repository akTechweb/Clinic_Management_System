using InfinityCoderzz_CMSV2026.Models.pharmacist;
using System.Data;
using Microsoft.Data.SqlClient;



namespace InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories
{
   

     


        public class MedicineRepositoryImpl : IMedicineRepository
        {
            private readonly IConfiguration _configuration;
            private readonly string connectionString;

            public MedicineRepositoryImpl(IConfiguration configuration)
            {
                _configuration = configuration;
                connectionString =
                    configuration.GetConnectionString("ConnStr");
            }

            #region Get All Medicines

            public IEnumerable<Medicine> GetAllMedicines()
            {
                List<Medicine> medicines =
                    new List<Medicine>();

                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd =
                        new SqlCommand(
                            "sp_GetAllMedicines",
                            connection);

                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            medicines.Add(new Medicine
                            {
                                MedicineId =
                                    Convert.ToInt32(reader["MedicineId"]),

                                MedicineCode =
                                    reader["MedicineCode"].ToString(),

                                MedicineName =
                                    reader["MedicineName"].ToString(),

                                GenericName =
                                    reader["GenericName"].ToString(),

                                CategoryId =
                                    Convert.ToInt32(reader["CategoryId"]),

                                CategoryName =
                                    reader["CategoryName"].ToString(),

                                ManufacturerId =
                                    Convert.ToInt32(reader["ManufacturerId"]),

                                ManufacturerName =
                                    reader["ManufacturerName"].ToString(),

                                Unit =
                                    reader["Unit"].ToString(),

                                UnitPrice =
                                    Convert.ToDecimal(reader["UnitPrice"]),

                                ReorderLevel =
                                    Convert.ToInt32(reader["ReorderLevel"]),

                                IsActive =
                                    Convert.ToBoolean(reader["IsActive"])
                            });
                        }
                    }
                }

                return medicines;
            }

            #endregion

            #region Search Medicine

            public IEnumerable<Medicine> SearchMedicine(string searchTerm)
            {
                List<Medicine> medicines =
                    new List<Medicine>();

                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd =
                        new SqlCommand(
                            "sp_SearchMedicine",
                            connection);

                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(
                        "@SearchTerm",
                        searchTerm);

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            medicines.Add(new Medicine
                            {
                                MedicineId =
                                    Convert.ToInt32(reader["MedicineId"]),

                                MedicineCode =
                                    reader["MedicineCode"].ToString(),

                                MedicineName =
                                    reader["MedicineName"].ToString(),

                                GenericName =
                                    reader["GenericName"].ToString(),

                                CategoryName =
                                    reader["CategoryName"].ToString(),

                                ManufacturerName =
                                    reader["ManufacturerName"].ToString(),

                                Unit =
                                    reader["Unit"].ToString(),

                                UnitPrice =
                                    Convert.ToDecimal(reader["UnitPrice"]),

                                ReorderLevel =
                                    Convert.ToInt32(reader["ReorderLevel"]),

                                IsActive =
                                    Convert.ToBoolean(reader["IsActive"])
                            });
                        }
                    }
                }

                return medicines;
            }

            #endregion

            #region Get Medicine By Id

            public Medicine GetMedicineById(int medicineId)
            {
                Medicine medicine = null;

                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd =
                        new SqlCommand(
                            "sp_GetMedicineById",
                            connection);

                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(
                        "@MedicineId",
                        medicineId);

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            medicine = new Medicine
                            {
                                MedicineId =
                                    Convert.ToInt32(reader["MedicineId"]),

                                MedicineCode =
                                    reader["MedicineCode"].ToString(),

                                MedicineName =
                                    reader["MedicineName"].ToString(),

                                GenericName =
                                    reader["GenericName"].ToString(),

                                CategoryId =
                                    Convert.ToInt32(reader["CategoryId"]),

                                ManufacturerId =
                                    Convert.ToInt32(reader["ManufacturerId"]),

                                Unit =
                                    reader["Unit"].ToString(),

                                UnitPrice =
                                    Convert.ToDecimal(reader["UnitPrice"]),

                                ReorderLevel =
                                    Convert.ToInt32(reader["ReorderLevel"]),

                                IsActive =
                                    Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }

                return medicine;
            }

            #endregion

            #region Add Medicine

            public void AddMedicine(Medicine medicine)
            {
                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd =
                        new SqlCommand(
                            "sp_InsertMedicine",
                            connection);

                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(
                        "@MedicineCode",
                        medicine.MedicineCode);

                    cmd.Parameters.AddWithValue(
                        "@MedicineName",
                        medicine.MedicineName);

                    cmd.Parameters.AddWithValue(
                        "@GenericName",
                        medicine.GenericName ?? "");

                    cmd.Parameters.AddWithValue(
                        "@CategoryId",
                        medicine.CategoryId);

                    cmd.Parameters.AddWithValue(
                        "@ManufacturerId",
                        medicine.ManufacturerId);

                    cmd.Parameters.AddWithValue(
                        "@Unit",
                        medicine.Unit ?? "");

                    cmd.Parameters.AddWithValue(
                        "@UnitPrice",
                        medicine.UnitPrice);

                    cmd.Parameters.AddWithValue(
                        "@ReorderLevel",
                        medicine.ReorderLevel);

                    cmd.ExecuteNonQuery();
                }
            }

            #endregion

            #region Update Medicine

            public void UpdateMedicine(Medicine medicine)
            {
                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd =
                        new SqlCommand(
                            "sp_UpdateMedicine",
                            connection);

                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(
                        "@MedicineId",
                        medicine.MedicineId);

                    cmd.Parameters.AddWithValue(
                        "@MedicineCode",
                        medicine.MedicineCode);

                    cmd.Parameters.AddWithValue(
                        "@MedicineName",
                        medicine.MedicineName);

                    cmd.Parameters.AddWithValue(
                        "@GenericName",
                        medicine.GenericName ?? "");

                    cmd.Parameters.AddWithValue(
                        "@CategoryId",
                        medicine.CategoryId);

                    cmd.Parameters.AddWithValue(
                        "@ManufacturerId",
                        medicine.ManufacturerId);

                    cmd.Parameters.AddWithValue(
                        "@Unit",
                        medicine.Unit ?? "");

                    cmd.Parameters.AddWithValue(
                        "@UnitPrice",
                        medicine.UnitPrice);

                    cmd.Parameters.AddWithValue(
                        "@ReorderLevel",
                        medicine.ReorderLevel);

                    cmd.Parameters.AddWithValue(
                        "@IsActive",
                        medicine.IsActive);

                    cmd.ExecuteNonQuery();
                }
            }

            #endregion

            #region Disable Medicine

            public void DisableMedicine(int medicineId)
            {
                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmd =
                        new SqlCommand(
                            "sp_DisableMedicine",
                            connection);

                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(
                        "@MedicineId",
                        medicineId);

                    cmd.ExecuteNonQuery();
                }
            }

        #endregion

        #region Get All Categories

        public IEnumerable<MedicineCategory> GetAllCategories()
        {
            List<MedicineCategory> categories =
                new List<MedicineCategory>();

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "sp_GetAllMedicineCategories",
                        connection);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MedicineCategory category =
                            new MedicineCategory();

                        category.CategoryId =
                            Convert.ToInt32(
                                reader["CategoryId"]);

                        category.CategoryName =
                            reader["CategoryName"]
                            .ToString();

                        categories.Add(category);
                    }
                }
            }

            return categories;
        }

        #endregion

        #region Get All Manufacturers

        public IEnumerable<Manufacturer> GetAllManufacturers()
        {
            List<Manufacturer> manufacturers =
                new List<Manufacturer>();

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "sp_GetAllManufacturers",
                        connection);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Manufacturer manufacturer =
                            new Manufacturer();

                        manufacturer.ManufacturerId =
                            Convert.ToInt32(
                                reader["ManufacturerId"]);

                        manufacturer.ManufacturerName =
                            reader["ManufacturerName"]
                            .ToString();

                        manufacturers.Add(manufacturer);
                    }
                }
            }

            return manufacturers;
        }

        #endregion

    }

}

