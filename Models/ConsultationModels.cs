using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InfinityCoderzz_CMSV2026.Models
{
    public class PrescriptionItemInput
    {
        [Required] public int MedicineId { get; set; }
        [Required] public string Dosage { get; set; }
        [Required] public string Frequency { get; set; }
        [Required] public string Duration { get; set; }
        [Required] public int Quantity { get; set; }
        public string Instructions { get; set; }
    }

    public class HistoricalConsultationItemViewModel
    {
        public DateTime VisitDate { get; set; }
        public string Diagnosis { get; set; }
        public string Symptoms { get; set; }
        public string DoctorName { get; set; }
    }

    public class PatientHistoryContainerViewModel
    {
        public List<HistoricalConsultationItemViewModel> Consultations { get; set; } = new();
    }

    public class ConsultationSetupViewModel
    {
        public int AppointmentId { get; set; }
        public string MMRCode { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Symptoms required")]
        public string Symptoms { get; set; }

        [Required(ErrorMessage = "Diagnosis required")]
        public string Diagnosis { get; set; }

        public string Notes { get; set; }
        public DateTime? FollowUpDate { get; set; }

        public List<int> SelectedLabTests { get; set; } = new();
        public List<PrescriptionItemInput> PrescriptionItems { get; set; } = new();

        public List<MedicineLookupModel> AvailableMedicines { get; set; } = new();
        public List<LabTestLookupModel> AvailableLabTests { get; set; } = new();
        public PatientHistoryContainerViewModel HistoricalData { get; set; } = new();
    }

    public class FinalSummaryDocumentViewModel
    {
        public string MMRCode { get; set; }
        public string PatientName { get; set; }
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
        public string ClinicalNotes { get; set; }
        public DateTime? FollowUpDate { get; set; }

        public List<PrescriptionSummaryLine> PrescribedMedicines { get; set; } = new();
        public List<string> OrderedLabTests { get; set; } = new();
    }

    public class PrescriptionSummaryLine
    {
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public int Quantity { get; set; }
    }
}
