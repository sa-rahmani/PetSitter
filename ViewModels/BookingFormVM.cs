using FoolProof.Core;
using Microsoft.AspNetCore.Mvc;
using PetSitter.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetSitter.ViewModels
{
    public class BookingFormVM
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }
        public int SitterId { get; set; }
        [BindProperty]
        public List<BookingPetVM> Pets { get; set; } = new List<BookingPetVM>();
        public DateTime StartDate { get; set; }
        [GreaterThan("StartDate")]
        public DateTime EndDate { get; set; }
        public string? SpecialRequests { get; set; }
        public string? Message { get; set; }

    }
}
