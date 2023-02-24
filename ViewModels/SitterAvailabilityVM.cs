namespace PetSitter.ViewModels
{
    public class SitterAvailabilityVM
    {
        public int SitterId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DateTime>? AvailableDates { get; set; }
        public List<DateTime>? BookedDates { get; set; }
        public string? Message { get; set; }

    }
}
