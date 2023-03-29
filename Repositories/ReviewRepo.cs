using PetSitter.Models;
using PetSitter.ViewModels;
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
                bookReview.Rating = createReviewVM.rating;
                bookReview.Review = createReviewVM.review;

                _db.Bookings.Update(bookReview);
                _db.SaveChanges();


                message = $"Success adding your new Review. " +
                               $"Your new Review number is: {createReviewVM.BookingId}";
            }
            catch (Exception e)
            {
                //pet.PetId = -1;
                message = $"Error creating your new Review, error: {e.Message}";

            }

            return Tuple.Create(createReviewVM.BookingId, message);
        }




















    }
}
