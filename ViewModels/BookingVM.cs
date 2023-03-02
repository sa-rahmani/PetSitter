using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetSitter.ViewModels
{
    public class BookingVM
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int BookingId { get; set; }
        public int SitterId { get; set; }
        public int UserId { get; set; }
        public List<BookingPetVM> Pets { get; set; }

        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:D}")]
        public DateTime EndDate { get; set; }
        public string? SpecialRequests { get; set; }
        public decimal Price { get; set; }

        public string? PaymentId { get; set; }
    }
}
