using HMS.Entities;

namespace HMS.Models
{
    public class RegistrationFormViewModel
    {
        public OPDPatientRegistration Patient { get; set; }
        public List<MASCategory> Categories { get; set; }
        public List<MASCountry> Countries { get; set; }
        public List<MASState> States { get; set; }
        public List<MASMandal> Mandals { get; set; }
        public List<MASCity> Cities { get; set; }

        public List<MASPaymentCode> PaymentModes { get; set; }

        public List<string> Titles { get; set; }
    }
}
