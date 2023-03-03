﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            List<BookingVM> myBookings = bookingRepo.GetBookingVMsByUserId(USERID);

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

        // GET: Initial Book
        public IActionResult Book(int sitterID)
        {
            BookingFormVM booking = new BookingFormVM();
            booking.SitterId= sitterID;

            // temporary values while developing
            int userID = 3;
            BookingRepo bookingRepo = new BookingRepo(_db);
            List<BookingPetVM> pets = bookingRepo.GetBookingPetVMsByUserId(userID);
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
            BookingRepo bookingRepo = new BookingRepo(_db);
            bool petsSelected = bookingRepo.CheckPetSelection(bookingForm);

            if (petsSelected)
            {
                if (ModelState.IsValid)
                {
                    // temporary values while developing
                    int userId = 3;

                    // Create booking
                    int bookingId = bookingRepo.Create(bookingForm, userId);

                    // Redirect to confirmation page
                    return RedirectToAction("ConfirmBooking", "Booking", new { bookingId = bookingId });
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
            BookingRepo bookingRepo = new BookingRepo(_db);
            BookingVM confirmBooking = bookingRepo.GetBookingVM(bookingId);
            return View(confirmBooking);
        }

        // GET: Edit
        public IActionResult Edit(int bookingId)
        {
            BookingRepo bookingRepo = new BookingRepo(_db);
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
            BookingRepo bookingRepo = new BookingRepo(_db);
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
            BookingRepo bookingRepo = new BookingRepo(_db);
            IPN completeIPN = bookingRepo.AddTransaction(ipn);
            return Json(ipn);
        }

        public IActionResult BookingDetails(int bookingID)
        {
            BookingRepo bookingRepo = new BookingRepo(_db);
            BookingVM booking = bookingRepo.GetBookingVM(bookingID);

            return View(booking);
        }

        
    }
}
