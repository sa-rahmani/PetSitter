using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSitter.Models;
using PetSitter.Repositories;
using PetSitter.ViewModels;

namespace PetSitter.Controllers
{
    public class CustomerController : Controller
    {
        private readonly PetSitterContext _db;

        public CustomerController(PetSitterContext db)
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
            IQueryable<SitterVM> allSitters = sitterRepo.GetAllSitters();

            // Display 10 sitters per page.
            int pageSize = 10;

            return View(PaginatedList<SitterVM>.Create(allSitters.AsNoTracking(), page ?? 1, pageSize));
        }

        public IActionResult SitterDetails(int sitterID)
        {
            // Get the SitterVM.
            CsFacingSitterRepo sitterRepo = new CsFacingSitterRepo(_db);
            SitterVM sitter = sitterRepo.GetSitter(sitterID);

            return View(sitter);
        }

        // GET: Book
        public IActionResult Book(int sitterID)
        {
            // temp use of a const userID during development
            const int USERID = 3;

            CsFacingSitterRepo sitterRepo = new CsFacingSitterRepo(_db);
            BookingVM booking = new BookingVM();
            booking.SitterId = sitterID;
            booking.UserId = USERID;

            return View(booking);
        }

        // POST: Book
        [HttpPost]
        public IActionResult Book(BookingVM booking)
        {
            if (ModelState.IsValid)
            {
                // Create the booking.

            }

            return View();
        }
    }
}
