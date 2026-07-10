namespace HMS.Models
{
    public class PatientTreatmentVM
    {
        public int CaseSheetId { get; set; }

        public int PatientId { get; set; }

        public int DeptId { get; set; }

        public int DoctorId { get; set; }


        public List<PatientServiceVM> Services { get; set; }
            = new List<PatientServiceVM>();


        public int SelectedServiceId { get; set; }

        public decimal SelectedRate { get; set; }

        public decimal SelectedQuantity { get; set; } = 1;

        public decimal SelectedDiscountPer { get; set; }


        public List<PatientTreatmentVMItem> ExistingTreatments { get; set; }
            = new List<PatientTreatmentVMItem>();


        // Temporary list from javascript
        public string PatientTreatmentJson { get; set; } = string.Empty;

    }
}
