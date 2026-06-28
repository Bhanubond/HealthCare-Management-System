using HMS.Entities;

namespace HMS.Models
{
    public class DoctorFormViewModel
    {
        public Doctor Doctor { get; set; } = new Doctor();
        public List<MASDepartment> Departments { get; set; } = new();
        public List<MASDesignation> Designations { get; set; } = new();
        public List<MASCountry> Countries { get; set; } = new();
        public List<MASState> States { get; set; } = new();
        public List<MASMandal> Mandals { get; set; } = new();
        public List<MASCity> Cities { get; set; } = new();
    }
}
