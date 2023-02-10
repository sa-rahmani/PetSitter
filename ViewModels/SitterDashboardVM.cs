using PetSitter.Models;

namespace PetSitter.ViewModels
{
    public class SitterDashboardVM
    {
        public int bookingId { get; set; }

        public string? sitter { get; set; }

        public string? petParent { get; set; }
        public string? petType { get; set; }
        public string? startDate { get; set; }
        public string? endDate { get; set; }
        public string? status { get; set; }
        public string? review { get; set; }
       public User user { get; set; }

        public int completeNbr { get; set; }
        public int  upComingNbr { get; set; }

        public int reviewsNbr { get; set; }



    }
}
