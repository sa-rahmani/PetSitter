using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSitter.Data.Services;
using PetSitter.Models;
using PetSitter.Repositories;
using PetSitter.ViewModels;
using System.Globalization;

namespace PetSitter.Controllers
{
    public class BookingController : Controller
    {
        private readonly PetSitterContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailService _emailService;

        public BookingController(PetSitterContext db, IEmailService emailService, IWebHostEnvironment webHost)
        {
            _db = db;
            _emailService = emailService;
            _webHostEnvironment = webHost;
        }   

        [Authorize]
        public IActionResult ViewMyBookings()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            List<BookingVM> myBookings = bookingRepo.GetUpcomingBookingVMsByUserId(userId);

            return View(myBookings);
        }

        [Authorize]
        public IActionResult ViewPastBookings()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            List<BookingVM> myBookings = bookingRepo.GetPastBookingVMsByUserId(userId);

            return View(myBookings);
        }

        [Authorize]
        public IActionResult BookingDetails(int bookingID)
        {
            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            BookingVM booking = bookingRepo.GetBookingVM(bookingID);

            return View(booking);
        }

        public IActionResult FindASitter(int? page, List<string> petTypes, string selectedDates)
        {
            // Assign ViewBag values for customer's filter options.
            ViewBag.SelectedDates = selectedDates;
            ViewBag.SelectedPetTypes = petTypes;

            List<DateTime> dates = new List<DateTime>();
            if (selectedDates != null)
            {
                string[] selectedDatesString = selectedDates.Split(',');

                // Convert selected dates to DateTime objects.
                foreach (string date in selectedDatesString)
                {
                    DateTime dt;
                    if (DateTime.TryParseExact(date.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        dates.Add(dt);
                    }
                }
            }

            // Get all pet types. 
            SitterRepos sitterRepos = new SitterRepos(_db, _webHostEnvironment);
            var allPetTypes = sitterRepos.getPetTypes();
            ViewBag.PetTypes = allPetTypes;

            // Get an IQueryable of all sitters.
            CsFacingSitterRepo sitterRepo = new CsFacingSitterRepo(_db);

            var allSitters = sitterRepo.GetAllSitterVMs().ToList();

            // Filter sitters.
            if ((petTypes != null && petTypes.Count > 0) && (dates != null && dates.Count > 0))
            {
                allSitters = allSitters.Where(s => s.petTypes.Any(pt => petTypes.Contains(pt)) && s.availableDates.Any(d => dates.Contains(d))).ToList();
            }
            else if (dates != null && dates.Count > 0)
            {
                allSitters = allSitters.Where(s => dates.All(d => s.availableDates.Contains(d))).ToList();
            }
            else if (petTypes != null && petTypes.Count > 0)
            {
                allSitters = allSitters.Where(s => s.petTypes.Any(pt => petTypes.Contains(pt))).ToList();
            }

            // Display 10 sitters per page.
            int pageSize = 10;

            return View(PaginatedList<SitterVM>.Create(allSitters.AsQueryable().AsNoTracking(), page ?? 1, pageSize));
        }

        [Authorize]
        // GET: Initial Book
        public IActionResult Book(int sitterID)
        {
            // Add sitter details to the booking form.
            BookingFormVM booking = new BookingFormVM();
            booking.SitterId = sitterID;
            CsFacingSitterRepo sitterRepo = new CsFacingSitterRepo(_db);
            var sitter = sitterRepo.GetSitterVM(sitterID);
            booking.SitterName = sitter.FirstName;

            // Get the current users pets to display on the form.
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            List<BookingPetVM> pets = bookingRepo.GetBookingPetVMsByUserId(userId);
            booking.Pets = pets;

            return View(booking);
        }

        [Authorize]
        // POST: Initial Book
        [HttpPost]
        public IActionResult Book(BookingFormVM bookingForm)
        {
            // If the message is null, set to an empty string.
            bookingForm.Message ??= "";

            // Check that at least one pet was selected.
            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            bool petsSelected = bookingRepo.CheckPetSelection(bookingForm);

            // Check that sitter is available for selected dates.
            bool sitterAvailable = bookingRepo.CheckSitterAvailability(bookingForm);

            // Check that start date is after today's date.
            bool validDate = bookingRepo.CheckDate(bookingForm.StartDate);

            if (validDate)
            {
                if (sitterAvailable)
                {
                    if (petsSelected)
                    {
                        if (ModelState.IsValid)
                        {
                            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

                            // Create booking
                            int bookingId = bookingRepo.Create(bookingForm, userId);

                            // Redirect to confirmation and payment page
                            return RedirectToAction("ConfirmBooking", "Booking", new { bookingId });
                        }
                    }
                    else // else if no pets have been selected
                    {
                        bookingForm.Message = "Please select at least one pet for this booking.";
                    }
                }
                else // else if sitter is not available
                {
                    bookingForm.Message = $"Sorry, {bookingForm.SitterName} is not available those days.";
                }
            } else // else if date is invalid 
            {
                bookingForm.Message = $"Please select a future date for the start date.";
            }

            // Show booking page again.
            return View(bookingForm);
        }

        [Authorize]
        public IActionResult ConfirmBooking(int bookingId)
        {
            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            BookingVM confirmBooking = bookingRepo.GetBookingVM(bookingId);
            return View(confirmBooking);
        }

        [Authorize]
        // GET: Edit
        public IActionResult Edit(int bookingId)
        {
            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            BookingFormVM booking = bookingRepo.GetBookingFormVM(bookingId);
            return View(booking);
        }

        [Authorize]
        // POST: Edit
        [HttpPost]
        public IActionResult Edit(BookingFormVM bookingForm)
        {
            // If the message is null, set to an empty string.
            bookingForm.Message ??= "";

            // Check that at least one pet was selected.
            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            bool petsSelected = bookingRepo.CheckPetSelection(bookingForm);

            // Check that sitter is available for selected dates.
            bool sitterAvailable = bookingRepo.CheckSitterAvailability(bookingForm);

            if (sitterAvailable)
            {
                if (petsSelected)
                {
                    if (ModelState.IsValid)
                    {
                        // Update booking
                        int bookingId = bookingRepo.Update(bookingForm);

                        // Redirect to confirmation page
                        return RedirectToAction("ConfirmBooking", "Booking", new { bookingId = bookingId });
                    }
                }
                else // if no pets selected
                {
                    bookingForm.Message = "Please select at least one pet for this booking.";
                }
            }
            else // if sitter is not available
            {
                bookingForm.Message = $"Sorry, {bookingForm.SitterName} is not available those days.";
            }

            // Show booking page again.
            return View(bookingForm);
        }

        [Authorize]
        [HttpPost]
        public JsonResult PaySuccess([FromBody] IPN ipn)
        {
            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);

            string email = HttpContext.Session.GetString("Email");

            IPN completeIPN = bookingRepo.AddTransaction(ipn, email);

            return Json(completeIPN);
        }

        public IActionResult SitterDetails(int sitterID)
        {
            // Get the SitterVM.
            CsFacingSitterRepo sitterRepo = new CsFacingSitterRepo(_db);
            SitterVM sitter = sitterRepo.GetSitterVM(sitterID);
            return View(sitter);

        }

        [Authorize]
        public IActionResult CreateReview(int sitterID, int bookingID)
        {
            int customerID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            SitterRepos sRepos = new SitterRepos(_db, _webHostEnvironment);
            var sitterInfor = sRepos.GetSitterById(sitterID);

            BookingRepo bRepo = new BookingRepo(_db, _emailService);
            var bookInfo = bRepo.GetBookingVM(bookingID);

            CsFacingSitterRepo cfsRepo = new CsFacingSitterRepo(_db);
            User user = cfsRepo.getUserById(sitterID);
            ViewData["SitterProfileImg"] = user;

            // Add sitter and booking details to the CreateReviewVM.
            CreateReviewVM reviewCreating = new CreateReviewVM
            {
                sitter = sitterInfor.FirstName + " " + sitterInfor.LastName,
                BookingId = bookingID,
                startDate = bookInfo.StartDate,
                endDate = bookInfo.EndDate,
            };

            ViewBag.SitterName = reviewCreating.sitter;

            return View(reviewCreating);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateReview(CreateReviewVM createReviewVM)
        {

            int customerID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            ReviewRepo reviewRepo = new ReviewRepo(_db);

            Tuple<int, string> response =
                reviewRepo.UpdateReview(createReviewVM);

            return RedirectToAction("SitterDetails", "Booking", new { createReviewVM.SitterId });
        }
    }
}
