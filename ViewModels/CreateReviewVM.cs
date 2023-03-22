namespace PetSitter.ViewModels
{
    public class CreateReviewVM
    {
        //public string? FirstName { get; set; }
        //public string? LastName { get; set; }


        public string? petParent { get; set; }

        public string? sitter { get; set; }

        public int? SitterId { get; set; }
        // public int? UserId { get; set; }


        public int BookingId { get; set; }

        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }

        public int rating { get; set; }
        public string? review { get; set; }
    }
}


