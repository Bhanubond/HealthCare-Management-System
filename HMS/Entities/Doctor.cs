using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities
{
    [Table("Doctors")]
    public class Doctor
    {
        [Key]
        public long DoctorId { get; set; }

        public string DoctorCode { get; set; }
        public string DoctorName { get; set; }

        public string Gender { get; set; }
        public DateTime? DOB { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }

        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? MandalId { get; set; }
        public int? CityId { get; set; }

        public DateTime JoiningDate { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public MASDepartment? Department { get; set; }
        public MASDesignation? Designation { get; set; }

        public MASCountry? Country { get; set; }
        public MASState? State { get; set; }
        public MASMandal? Mandal { get; set; }
        public MASCity? City { get; set; }
    }
}
