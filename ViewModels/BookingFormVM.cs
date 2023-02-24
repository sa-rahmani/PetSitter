using FoolProof.Core;
using System.ComponentModel.DataAnnotations;

namespace PetSitter.ViewModels
{
    public class BookingFormVM
    {
        public int SitterId { get; set; }
        public DateTime StartDate { get; set; }
        [GreaterThan("StartDate")]
        public DateTime EndDate { get; set; }
        public string? SpecialRequests { get; set; }
    }
}
