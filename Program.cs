using QuestPDF.Infrastructure;
using InfinityCoderzz_CMSV2026.Repositories;
using InfinityCoderzz_CMSV2026.Services;
using InfinityCoderzzz_CMSV2026.Repositories;
using InfinityCoderzzz_CMSV2026.Services;
using InfinityCoderzz_CMSV2026.Repositories.PharmacistRepositories;
using InfinityCoderzz_CMSV2026.Services.PharmacyServices;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add MVC
builder.Services.AddControllersWithViews();

// Session Services
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Dependency Injection
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

builder.Services.AddScoped<IBillRepository, BillRepository>();
builder.Services.AddScoped<IBillService, BillService>();

builder.Services.AddScoped<IPatientVisitRepository, PatientVisitRepository>();
builder.Services.AddScoped<IPatientVisitService, PatientVisitService>();

builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IDoctorService, DoctorService>();

builder.Services.AddScoped<ILabTechnicianRepository, LabTechnicianRepository>();
builder.Services.AddScoped<ILabTechnicianService, LabTechnicianService>();

// -------------------- Pharmacy Module --------------------

builder.Services.AddScoped<IMedicineRepository, MedicineRepositoryImpl>();
builder.Services.AddScoped<IMedicineService, MedicineServiceImpl>();

builder.Services.AddScoped<IMedicineStockRepository, MedicineStockRepositoryImpl>();
builder.Services.AddScoped<IMedicineStockService, MedicineStockServiceImpl>();

builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepositoryImpl>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionServiceImpl>();

builder.Services.AddScoped<IPharmacyDashboardRepository, PharmacyDashboardRepositoryImpl>();
builder.Services.AddScoped<IPharmacyDashboardService, PharmacyDashboardServiceImpl>();

builder.Services.AddScoped<IInventoryLogRepository, InventoryLogRepositoryImpl>();
builder.Services.AddScoped<IInventoryLogService, InventoryLogServiceImpl>();

builder.Services.AddScoped<IMedicineDispensingRepository, MedicineDispensingRepositoryImpl>();
builder.Services.AddScoped<IMedicineDispensingService, MedicineDispensingServiceImpl>();

builder.Services.AddScoped<IPharmacyBillRepository, BillRepositoryImpl>();
builder.Services.AddScoped<IPharmacyBillService, BillServiceImpl>();

builder.Services.AddScoped<IPharmacyLoginRepository, PharmacyLoginRepositoryImpl>();
builder.Services.AddScoped<IPharmacyLoginService, PharmacyLoginServiceImpl>();

builder.Services.AddScoped<IAuditLogRepository, AuditLogRepositoryImpl>();
builder.Services.AddScoped<IAuditLogService, AuditLogServiceImpl>();

builder.Services.AddScoped<IReportRepository, ReportRepositoryImpl>();
builder.Services.AddScoped<IReportService, ReportServiceImpl>();

var app = builder.Build();

// Configure Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Session Middleware
app.UseSession();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();