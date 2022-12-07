namespace PetSitter.ViewModels
{
    public class BookingVM
    {
        public int SitterId { get; set; }
        public int UserId { get; set; }
        public List<int> PetIDs { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SpecialRequests { get; set; }
    }
}
