using PetSitter.Models;
using PetSitter.ViewModels;
using System.Security.Policy;

namespace PetSitter.Repositories
{
    public class CsFacingSitterRepo
    {

        private readonly PetSitterContext _db;

        public CsFacingSitterRepo(PetSitterContext db)
        {
            _db = db;
        }

        public IQueryable<SitterVM> GetAllSitters()
        {
            var allSitters = from s in _db.Sitters
                             join u in _db.Users
                                    on s.UserId equals u.UserId
                             select new SitterVM
                             {
                                 SitterId = s.SitterId,
                                 FirstName = u.FirstName,
                                 Rate = s.RatePerPetPerDay,
                                 ProfileBio = s.ProfileBio,
                                 AvgRating = (double)_db.Bookings.Where(b => b.SitterId == s.SitterId).Average(b => b.Rating),
                                 Reviews = _db.Bookings.Where(b => b.SitterId == s.SitterId).Select(b => b.Review).ToList()
                             };

            return allSitters;
        }

        public SitterVM GetSitter(int sitterID)
        {
            SitterVM sitter = GetAllSitters().Where(s => s.SitterId == sitterID).FirstOrDefault();

            return sitter;
        }
    }
}
