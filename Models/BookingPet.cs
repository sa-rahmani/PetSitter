using System;
using System.Collections.Generic;

namespace PetSitter.Models
{
    public partial class BookingPet
    {
        public int BookingId { get; set; }
        public int PetId { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Pet Pet { get; set; }

        public BookingPet(int bookingId, int petId)
        {
            BookingId = bookingId;
            PetId = petId;
        }
    }
}
