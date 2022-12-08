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

        public IQueryable<BookingVM> GetAllBookingVMs()
        {
            var allBookings = from b in _db.Bookings
                              select new BookingVM
                              {
                                  SitterId = b.SitterId,
                                  UserId = b.UserId,
                                  StartDate = b.StartDate,
                                  EndDate = b.EndDate,
                                  SpecialRequests = b.SpecialRequests,
                                  PetIDs = _db.BookingPets.Where(bp => bp.BookingId == b.BookingId).Select(bp => bp.PetId).ToList()
                              };

            return allBookings;
        }

        public IQueryable<BookingVM> GetMyBookingVMs(int userID)
        {
            return GetAllBookingVMs().Where(b => b.UserId == userID);
        }

        public BookingVM GetBookingVM(int bookingID)
        {
            return GetAllBookingVMs().Where(b => b.BookingId == bookingID).FirstOrDefault();
        }

        public BookingVM AddPriceToBooking(BookingVM booking)
        {
            // Calculate number of days in booking.
            DateTime endDate = (DateTime)booking.EndDate;
            int days = endDate.Subtract((DateTime)booking.StartDate).Days;

            // Get sitter.
            CsFacingSitterRepo sitterRepo = new CsFacingSitterRepo(_db);
            SitterVM sitter = sitterRepo.GetSitterVM((int)booking.SitterId);

            decimal price = sitter.Rate * days * booking.PetIDs.Count;

            booking.Price = price;

            return booking;
        }

        public Booking Create(BookingVM booking)
        {
            // Create a new Booking object.
            Booking newBooking = new Booking((decimal)booking.Price, (DateTime)booking.StartDate, (DateTime)booking.EndDate, booking.SpecialRequests, (int)booking.SitterId, (int)booking.UserId);

            // Save to database.
            _db.Add(newBooking);
            _db.SaveChanges();

            // Create BookingPet objects and add to database.
            foreach (var petID in booking.PetIDs)
            {
                BookingPet bookingPet = new BookingPet(newBooking.BookingId, petID);
                _db.Add(bookingPet);
                _db.SaveChanges();
            }

            return newBooking;
        }
    }
}
