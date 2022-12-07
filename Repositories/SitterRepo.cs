using PetSitter.Models;
using PetSitter.ViewModels;

namespace PetSitter.Repositories
{
    public class SitterRepo
    {

        private readonly PetSitterContext _db;

        public SitterRepo(PetSitterContext db)
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
                                 ProfileBio = s.ProfileBio
                             };

            return allSitters;
        }
    }
}
