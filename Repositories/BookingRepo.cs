using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PetSitter.Models;
using PetSitter.ViewModels;
using System.Drawing;
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
                                                 Price = (decimal)b.Price,
                                                 PaymentId = b.PaymentId
                                             };

            // Convert to list so that it can be looped through to add pets.
            List<BookingVM> bookingsList = bookings.ToList();

            // Get all BookingPetVMs.
            IQueryable<BookingPetVM> bookingPetVMs = from p in _db.Pets
                                                     select new BookingPetVM
                                                     {
                                                         PetId = p.PetId,
                                                         Name = p.Name
                                                     };

            // Assign the appropriate BookingPetVM to each BookingVM.
            foreach (var booking in bookingsList)
            {
                List<int> petIds = _db.BookingPets.Where(bp => bp.BookingId == booking.BookingId).Where(bp => bp.PetId.HasValue).Select(bp => bp.PetId.GetValueOrDefault()).ToList();

                List<BookingPetVM> pets = new List<BookingPetVM>();
                foreach (var petId in petIds)
                {
                    pets.Add(bookingPetVMs.Where(pv => pv.PetId == petId).FirstOrDefault());
                }

                booking.Pets = pets;
            }

            return bookingsList;
        }

        public List<BookingVM> GetBookingVMsByUserId(int userID)
        {
            List<BookingVM> bookings = GetAllBookingVMs();
            List<BookingVM> myBookings = new List<BookingVM>();
            foreach (var booking in bookings)
            {
                if(booking.UserId == userID && booking.PaymentId != null)
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

            decimal price = sitter.Rate * days * booking.Pets.Count;

            booking.Price = price;

            return booking;
        }

        public int Create(BookingVM booking)
        {
            // Add price to booking.
            BookingVM fullBooking = AddPriceToBooking(booking);

            // Create a new Booking object.
            Booking newBooking = new Booking((decimal)fullBooking.Price, fullBooking.StartDate, fullBooking.EndDate, fullBooking.SpecialRequests, (int)fullBooking.SitterId, (int)fullBooking.UserId);

            // Save to database.
            _db.Add(newBooking);
            _db.SaveChanges();

            // Create BookingPet objects and add to database.
            foreach (var pet in booking.Pets)
            {
                BookingPet bookingPet = new BookingPet(newBooking.BookingId, pet.PetId);
                _db.Add(bookingPet);
                _db.SaveChanges();
            }

            return newBooking.BookingId;
        }

        public IPN AddTransaction(IPN ipn)
        {
            // Add IPN record.
            _db.IPNs.Add(ipn);
            _db.SaveChanges();

            // Update Booking record with payment ID.
            Booking booking = _db.Bookings.Where(b => b.BookingId.ToString() == ipn.custom).FirstOrDefault();
            booking.PaymentId = ipn.paymentID;
            _db.Bookings.Update(booking);
            _db.SaveChanges();

            return ipn;
        }

        public List<BookingPetVM> GetBookingPetVMsByUserId(int userId)
        {
            //var pets = _db.Pets.Where(p => p.UserId == userId)
            //return _db.Pets.Where(p => p.UserId == userId).ToList();

            IQueryable<BookingPetVM> bookingPetVMs =    from p in _db.Pets
                                                        where p.UserId == userId
                                                        select new BookingPetVM
                                                        {
                                                            PetId = p.PetId,
                                                            Name = p.Name
                                                        };

            List<BookingPetVM> result = bookingPetVMs.ToList();

            return result;
        }

        //Get All sitter's booking
        public List<Booking> GetBookingsBySitter(int sitterID)
        {
            var bookings = _db.Bookings.Where(s => s.SitterId == sitterID).ToList();

            return bookings;
        }
        public List<DateTime> GetBookedDates(List<Booking> bookings)
        {
            var bookedDates = new List<DateTime>();
            foreach (var booking in bookings)
            {
                for (DateTime date = (DateTime)booking.StartDate; date <= (DateTime)booking.EndDate; date = date.AddDays(1))
                {
                    bookedDates.Add(date);
                }
            }
            return bookedDates;
        }



    }
}
