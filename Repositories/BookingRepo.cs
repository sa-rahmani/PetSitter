using PetSitter.Models;
using PetSitter.ViewModels;
using System.Runtime.InteropServices;

namespace PetSitter.Repositories
{
    public class BookingRepo
    {
        private readonly PetSitterContext _db;

        public BookingRepo(PetSitterContext db)
        {
            _db = db;
        }

        public BookingVM Create(BookingVM booking)
        {
            // Determine number of days booking is for

            // Determine price

            return booking;
        }
    }
}
