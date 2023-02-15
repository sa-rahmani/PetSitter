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

        public IActionResult ViewMyBookings()
        {
            // temp use of const user id
            const int USERID = 3;

            BookingRepo bookingRepo = new BookingRepo(_db);
            IQueryable<BookingVM> myBookings = bookingRepo.GetBookingVMsByUserId(USERID);

            return View(myBookings);
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

        // possible code for selecting particular pets for a booking
        //// GET: Select Pets
        //public IActionResult SelectPets()
        //{
        //    // temp user ID for use during dev
        //    int userID = 3;

        //    // Get user's pets
        //    BookingRepo bookingRepo = new BookingRepo(_db);
        //    IQueryable<SelectPetsVM> pets = bookingRepo.GetSelectPetVMsByUserId(userID);

        //    return View(pets);
        //}

        //// POST: Select Pets
        //[HttpPost]
        //public IActionResult SelectPets(List<SelectPetsVM> pets)
        //{
        //    var selectedPets = pets.Where(p => p.IsChecked).ToList();
        //    return RedirectToAction("Book", new { selectedPets = selectedPets });
        //}

        // GET: Initial Book
        public IActionResult Book(int sitterID)
        {
            BookingFormVM booking = new BookingFormVM();
            booking.SitterId= sitterID;

            return View(booking);
        }

        // POST: Initial Book
        [HttpPost]
        public IActionResult Book(BookingFormVM bookingForm)
        {
            if (ModelState.IsValid)
            {
                // temporary values while developing
                int userID = 3;
                BookingRepo bookingRepo = new BookingRepo(_db);
                List<int> petIds = bookingRepo.GetPetIdsByUserId(userID);

                // Create BookingVM
                BookingVM booking = new BookingVM();
                booking.SitterId = bookingForm.SitterId;
                booking.UserId = userID;
                booking.PetIDs = petIds;
                booking.StartDate = bookingForm.StartDate;
                booking.EndDate = bookingForm.EndDate;
                booking.SpecialRequests = bookingForm.SpecialRequests;

                // Add price to BookingVM
                BookingVM fullBooking = bookingRepo.AddPriceToBooking(booking);

                // Redirect to confirmation page
                return Redirect(Url.Action("ConfirmBooking", "Booking", fullBooking));
            }

            // Show booking page again.
            return View(bookingForm);
        }

        public IActionResult ConfirmBooking(BookingVM booking)
        {
            return View(booking);
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

        
    }
}
