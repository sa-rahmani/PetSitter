using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSitter.Models;
using PetSitter.Repositories;
using PetSitter.ViewModels;

namespace PetSitter.Controllers
{
    public class BookingController : Controller
    {
        private readonly PetSitterContext _db;

        public BookingController(PetSitterContext db)
        {
            _db = db;
        }   

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FindASitter(int? page)
        {
            // Get an IQueryable of all sitters.
            CsFacingSitterRepo sitterRepo = new CsFacingSitterRepo(_db);
            IQueryable<SitterVM> allSitters = sitterRepo.GetAllSitterVMs();

            // Display 10 sitters per page.
            int pageSize = 10;

            return View(PaginatedList<SitterVM>.Create(allSitters.AsNoTracking(), page ?? 1, pageSize));
        }

        public IActionResult SitterDetails(int sitterID)
        {
            // Get the SitterVM.
            CsFacingSitterRepo sitterRepo = new CsFacingSitterRepo(_db);
            SitterVM sitter = sitterRepo.GetSitterVM(sitterID);

            return View(sitter);
        }

        // GET: Initial Book
        public IActionResult Book(int sitterID)
        {
            // temp use of a const userID + petIDs during development
            const int USERID = 3;
            List<int> PETIDS = new List<int>() { 1, 2 };

            BookingVM booking = new BookingVM();
            booking.SitterId = sitterID;
            booking.UserId = USERID;
            booking.Price = 0;
            booking.PetIDs = PETIDS;

            return View(booking);
        }

        // POST: Initial Book
        [HttpPost]
        public IActionResult Book(BookingVM booking)
        {
            if (ModelState.IsValid)
            {
                // Redirect to confirmation page
                return RedirectToAction("ConfirmBooking", "Customer", new { booking });
            }

            // Show booking page again.
            return View(booking);
        }

        public IActionResult ConfirmBooking(BookingVM booking)
        {
            // Determine price.
            BookingRepo bookingRepo = new BookingRepo(_db);
            BookingVM updatedBooking = bookingRepo.AddPriceToBooking(booking);

            return View(updatedBooking);
        }

        public IActionResult Pay(BookingVM booking)
        {
            return View(booking);
        }

        public IActionResult CompleteBooking(BookingVM booking)
        {
            // Add confirmed + paid for booking to the database.
            BookingRepo bookingRepo = new BookingRepo(_db);
            Booking newBooking = bookingRepo.Create(booking);

            return View(newBooking);
        }

        public IActionResult BookingDetails(int bookingID)
        {
            BookingRepo bookingRepo = new BookingRepo(_db);
            BookingVM booking = bookingRepo.GetBookingVM(bookingID);

            return View(booking);
        }

        public IActionResult ViewMyBookings()
        {
            // temp use of const user id
            const int USERID = 3;

            BookingRepo bookingRepo = new BookingRepo(_db);
            IQueryable<BookingVM> myBookings = bookingRepo.GetMyBookingVMs(USERID);

            return View(myBookings);
        }
    }
}
