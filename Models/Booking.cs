using System;
using System.Collections.Generic;

namespace PetSitter.Models
{
    public partial class Booking
    {
        public int BookingId { get; set; }
        public decimal? Price { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SpecialRequests { get; set; }
        public int? Rating { get; set; }
        public string? Review { get; set; }
        public string? Complaint { get; set; }
        public int? SitterId { get; set; }
        public int? UserId { get; set; }

        public virtual Sitter? Sitter { get; set; }
        public virtual User? User { get; set; }
    }
}
