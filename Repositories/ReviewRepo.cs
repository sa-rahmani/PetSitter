using Microsoft.AspNetCore.Hosting;
using PetSitter.Models;
using PetSitter.ViewModels;
using System.Data.Entity;
using System.Drawing;

namespace PetSitter.Repositories
{
    public class ReviewRepo
    {
        PetSitterContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ReviewRepo(PetSitterContext context)
        {
            _db = context;
        }


        public Tuple<int, string> UpdateReview(CreateReviewVM createReviewVM)
        {
            string message;

            try
            {
                Booking bookReview = _db.Bookings.Where(b => b.BookingId == createReviewVM.BookingId).FirstOrDefault();
                bookReview.Rating = createReviewVM.Rating;
                bookReview.Review = createReviewVM.Review;

                _db.Bookings.Update(bookReview);
                _db.SaveChanges();


                message = $"Success adding your new Review. " +
                               $"Your new Review number is: {createReviewVM.BookingId}";
            }
            catch (Exception e)
            {
                message = $"Error creating your new Review, error: {e.Message}";

            }

            return Tuple.Create(createReviewVM.BookingId, message);
        }


        public IQueryable<SitterVM> GetTop3SitterVMs() { 

            var Top3Sitters = (from s in _db.Sitters.Include(s => s.Availabilities)
                               join u in _db.Users on s.UserId equals u.UserId
                               select new SitterVM
                               {
                                   SitterId = s.SitterId,
                                   FirstName = u.FirstName,
                                   Rate = (decimal)s.RatePerPetPerDay,
                                   ProfileBio = s.ProfileBio,
                                   ProfileImage = u.ProfileImage,
                                   AvgRating = (double)_db.Bookings.Where(b => b.SitterId == s.SitterId).Average(b => b.Rating),
                                   petTypes = _db.Sitters.Where(b => b.SitterId == s.SitterId).SelectMany(s => s.PetTypes).Select(p => p.PetType1).ToList(),
                                   availabilities = s.Availabilities.ToList(),
                               }).OrderByDescending(s => s.AvgRating)
                                 .ToList();

            if (Top3Sitters.Count > 3)
            {
                Top3Sitters = Top3Sitters.Take(3).ToList();
            }


            AvailabilityRepo availabilityRepo = new AvailabilityRepo(_db);
            foreach (var sitter in Top3Sitters)
            {
                if (sitter.availabilities != null)
                {
                    sitter.availableDates = availabilityRepo.GetAvailableDates(sitter.availabilities);
                }
            }

            return Top3Sitters.AsQueryable();


        }









    }
}
