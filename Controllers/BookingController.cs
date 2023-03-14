﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSitter.Data.Services;
using PetSitter.Models;
using PetSitter.Repositories;
using PetSitter.ViewModels;
using SendGrid.Helpers.Mail;
using System.Security.Principal;

namespace PetSitter.Controllers
{
    public class BookingController : Controller
    {
        private readonly PetSitterContext _db;
        private readonly IEmailService _emailService;

        public BookingController(PetSitterContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }   

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewMyBookings()
        {
            // FOR DEVELOPMENT: GET USER ID IF LOGGED IN, OTHERWISE RETURN DEFAULT FOR QUICK TESTING OF FEATURES
            int userId = 3;
            
            if (HttpContext.Session.GetString("UserID") != null) {
                userId = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            }

            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            List<BookingVM> myBookings = bookingRepo.GetUpcomingBookingVMsByUserId(userId);

            return View(myBookings);
        }

        public IActionResult ViewPastBookings()
        {
            // FOR DEVELOPMENT: GET USER ID IF LOGGED IN, OTHERWISE RETURN DEFAULT FOR QUICK TESTING OF FEATURES
            int userId = 3;

            if (HttpContext.Session.GetString("UserID") != null)
            {
                userId = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            }

            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            List<BookingVM> myBookings = bookingRepo.GetPastBookingVMsByUserId(userId);

            return View(myBookings);
        }

        public IActionResult BookingDetails(int bookingID)
        {
            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            BookingVM booking = bookingRepo.GetBookingVM(bookingID);

            return View(booking);
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
            BookingFormVM booking = new BookingFormVM();
            booking.SitterId= sitterID;

            // FOR DEVELOPMENT: GET USER ID IF LOGGED IN, OTHERWISE RETURN DEFAULT FOR QUICK TESTING OF FEATURES
            int userId = 3;

            if (HttpContext.Session.GetString("UserID") != null)
            {
                userId = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            }

            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            List<BookingPetVM> pets = bookingRepo.GetBookingPetVMsByUserId(userId);
            booking.Pets = pets;

            return View(booking);
        }

        // POST: Initial Book
        [HttpPost]
        public IActionResult Book(BookingFormVM bookingForm)
        {
            // If the message is null, set to an empty string.
            bookingForm.Message ??= "";

            // Check that at least one pet was selected.
            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            bool petsSelected = bookingRepo.CheckPetSelection(bookingForm);

            if (petsSelected)
            {
                if (ModelState.IsValid)
                {
                    // FOR DEVELOPMENT: GET USER ID IF LOGGED IN, OTHERWISE RETURN DEFAULT FOR QUICK TESTING OF FEATURES
                    int userId = 3;

                    if (HttpContext.Session.GetString("UserID") != null)
                    {
                        userId = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
                    }

                    // Create booking
                    int bookingId = bookingRepo.Create(bookingForm, userId);

                    // Redirect to confirmation and payment page
                    return RedirectToAction("ConfirmBooking", "Booking", new { bookingId });
                }
            } else
            {
                bookingForm.Message = "Please select at least one pet for this booking.";
            }

            // Show booking page again.
            return View(bookingForm);
        }

        public IActionResult ConfirmBooking(int bookingId)
        {
            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            BookingVM confirmBooking = bookingRepo.GetBookingVM(bookingId);
            return View(confirmBooking);
        }

        // GET: Edit
        public IActionResult Edit(int bookingId)
        {
            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            BookingFormVM booking = bookingRepo.GetBookingFormVM(bookingId);
            return View(booking);
        }

        // POST: Edit
        [HttpPost]
        public IActionResult Edit(BookingFormVM bookingForm)
        {
            // If the message is null, set to an empty string.
            bookingForm.Message ??= "";

            // Check that at least one pet was selected.
            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);
            bool petsSelected = bookingRepo.CheckPetSelection(bookingForm);

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
            else
            {
                bookingForm.Message = "Please select at least one pet for this booking.";
            }

            // Show booking page again.
            return View(bookingForm);
        }

        [HttpPost]
        public JsonResult PaySuccess([FromBody] IPN ipn)
        {
            BookingRepo bookingRepo = new BookingRepo(_db, _emailService);

            // FOR DEVELOPMENT: GET EMAIL IF LOGGED IN, OTHERWISE RETURN DEFAULT FOR QUICK TESTING OF FEATURES
            string email = "laurenemilybyrne@gmail.com";

            if (HttpContext.Session.GetString("Email") != null)
            {
                email = HttpContext.Session.GetString("Email");
            }

            IPN completeIPN = bookingRepo.AddTransaction(ipn, email);

            return Json(completeIPN);
        }

        
    }
}
