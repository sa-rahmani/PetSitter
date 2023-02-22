using Microsoft.EntityFrameworkCore;
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

        public List<BookingVM> GetAllBookingVMs()
        {
            IQueryable<BookingVM> bookings = from b in _db.Bookings
                                             select new BookingVM
                                             {
                                                 BookingId = b.BookingId,
                                                 SitterId = (int)b.SitterId,
                                                 UserId = (int)b.UserId,
                                                 StartDate = (DateTime)b.StartDate,
                                                 EndDate = (DateTime)b.EndDate,
                                                 SpecialRequests = b.SpecialRequests,
                                                 Price = (decimal)b.Price
                                             };

            // Convert to list so that it can be looped through to add PetIDs.
            List<BookingVM> bookingsList = bookings.ToList();

            foreach (var booking in bookingsList)
            {
                List<int> pets = _db.BookingPets.Where(bp => bp.BookingId == booking.BookingId).Where(bp => bp.PetId.HasValue).Select(bp => bp.PetId.GetValueOrDefault()).ToList();
                booking.PetIDs = pets;
            }

            return bookingsList;
        }

        public List<BookingVM> GetBookingVMsByUserId(int userID)
        {
            List<BookingVM> bookings = GetAllBookingVMs();
            List<BookingVM> myBookings = new List<BookingVM>();
            foreach (var booking in bookings)
            {
                if(booking.UserId == userID)
                {
                    myBookings.Add(booking);
                }
            }
            return myBookings;
        }

        public BookingVM GetBookingVM(int bookingID)
        {
            return GetAllBookingVMs().Where(b => b.BookingId == bookingID).FirstOrDefault();
        }

        public BookingVM AddPriceToBooking(BookingVM booking)
        {
            // Calculate number of days in booking.
            int days = booking.EndDate.Subtract(booking.StartDate).Days;

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
            Booking newBooking = new Booking((decimal)booking.Price, booking.StartDate, booking.EndDate, booking.SpecialRequests, (int)booking.SitterId, (int)booking.UserId);

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

        public List<int> GetPetIdsByUserId(int userId)
        {
            return _db.Pets.Where(p => p.UserId == userId).Select(p => (int)p.PetId).ToList();
        }
    }
}
