using PetSitter.Models;

namespace PetSitter.ViewModels
{
    public class SitterAvailabilityVM
    {
        public int SitterId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Availability>? AvailableDates { get; set; }
        public List<Booking>? BookedDates { get; set; }
        public string? Message { get; set; }

    }
}
