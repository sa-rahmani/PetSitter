using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PetSitter.Models;
using PetSitter.Repositories;
using PetSitter.ViewModels;
using System.Drawing.Drawing2D;

namespace PetSitter.Controllers
{
    public class SitterController : Controller
    {
        private readonly PetSitterContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<CustomerController> _logger;


        public static int clientID;
        public SitterController(ILogger<CustomerController> logger, PetSitterContext db, IWebHostEnvironment webHost)
        {
            _db = db;
            _logger = logger;
            _webHostEnvironment = webHost;


        }
        public IActionResult Dashboard()
        {
            int sitterID = Convert.ToInt32(HttpContext.Session.GetString("SitterID"));

            SitterRepos sitterRepos = new SitterRepos(_db, _webHostEnvironment);
            IEnumerable<SitterDashboardVM> bookings = sitterRepos.GetBooking(sitterID);
            ViewData["UpComing"] = bookings.Select(b => b.upComingNbr).LastOrDefault();
            ViewData["Complete"] = bookings.Select(b => b.completeNbr).LastOrDefault();
            ViewData["Reviews"] = bookings.Select(b => b.reviewsNbr).LastOrDefault();


            return View(bookings);
        }
        public IActionResult Details(int id)
        {
            SitterRepos sitterRepos = new SitterRepos(_db, _webHostEnvironment);
            SitterDashboardVM booking = sitterRepos.GetBookingDetails(id);

            return View(booking);
        }

        public IActionResult Profile(string message)
        {
            SitterRepos sitterRepos = new SitterRepos(_db, _webHostEnvironment);
            //SitterProfileVM sitterProfileVM = sitterRepos.GetSitterByEmail(User.Identity.Name);
            ViewData["UserName"] = HttpContext.Session.GetString("UserName");

            int userID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            ViewData["UserProfileImg"] = sitterRepos.getUser(userID);

            int sitterID = Convert.ToInt32(HttpContext.Session.GetString("SitterID"));

            SitterProfileVM sitterProfileVM = sitterRepos.GetSitterById(sitterID);

            sitterProfileVM.Message = message;
            return View(sitterProfileVM);
        }
        public IActionResult EditProfile()
        {
            SitterRepos sitterRepos = new SitterRepos(_db, _webHostEnvironment);
            int userID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            int sitterID = Convert.ToInt32(HttpContext.Session.GetString("SitterID"));

            SitterProfileVM vm = sitterRepos.GetSitterById(sitterID);

            ViewData["UserName"] = HttpContext.Session.GetString("UserName");

            return View(vm);
        }
        [HttpPost]
        public IActionResult EditProfile(SitterProfileVM sitterProfileVM)
        {
            sitterProfileVM.Message = "Invalid entry please try again";

            SitterRepos sitterRepos = new SitterRepos(_db, _webHostEnvironment);


            if (ModelState.IsValid)
            {
                //sitterProfileVM.SelectedPetTypes = PetTypes?.ToList() ?? new List<string>();


                Tuple<int, string> response = sitterRepos.EditSitter(sitterProfileVM);

                if (response.Item1 < 0)
                {
                    sitterProfileVM.Message = response.Item2;
                }
                else
                {
                    return RedirectToAction("Profile", "Sitter", new { message = response.Item2 });
                }
            }

            return View(sitterProfileVM);
        }

        public IActionResult Availability()
        {
            // Get the logged in sitter ID
            int sitterID = Convert.ToInt32(HttpContext.Session.GetString("SitterID"));

            SitterAvailabilityVM vm = new SitterAvailabilityVM();

            // Set the sitter ID for the view model
            vm.SitterId = sitterID;

            // Get the booked dates for the sitter from the Booking table
            var Bookings = _db.Sitters.Include(s => s.Bookings)
                .Where(s => s.SitterId == sitterID).Select(s => s.Bookings);

            // Pass the booked dates to the view
            ViewBag.BookedDates = JsonConvert.SerializeObject(Bookings.ToList());


            //// Get the available dates for the sitter from the Availability table
            //var availableDates = _db.Availability
            //    .Where(a => !_db.SitterAvailability
            //        .Where(sa => sa.SitterID == sitterID)
            //        .Select(sa => sa.AvailabilityID)
            //        .Contains(a.AvailabilityID))
            //    .ToList();

            //// Pass the available dates to the view
            //ViewBag.BookedDates = JsonConvert.SerializeObject(sitterObj.Availabilities);

            // Return the view with the view model
            return View(vm);


        }

        [HttpPost]
        public IActionResult Availability(SitterAvailabilityVM availability)
        {
            availability.Message = "Invalid entry please try again";

            // Check if the model is valid
            if (ModelState.IsValid)
            {
                SitterRepos sitterRepos = new SitterRepos(_db, _webHostEnvironment);
                Tuple<int, string> response = sitterRepos.AddAvailability(availability);
                if (response.Item1 < 0)
                {
                    availability.Message = response.Item2;
                }
                else
                {
                    return RedirectToAction("Availability", "Sitter");
                }
            }

            // If the model is not valid, return the view with the errors
            return View(availability);

        }


    }
}
