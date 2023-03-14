﻿using PetSitter.Models;
using PetSitter.ViewModels;
using System.Data.Entity;
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

        public IQueryable<SitterVM> GetAllSitterVMs()
        {



            var allSitters = (from s in _db.Sitters.Include(s=> s.Availabilities)
                             join u in _db.Users
                                    on s.UserId equals u.UserId
                           
                             select new SitterVM
                             {
                                 SitterId = s.SitterId,
                                 FirstName = u.FirstName,
                                 Rate = (decimal)s.RatePerPetPerDay,
                                 ProfileBio = s.ProfileBio,
                                 AvgRating = (double)_db.Bookings.Where(b => b.SitterId == s.SitterId).Average(b => b.Rating),
                                 Reviews = _db.Bookings.Where(b => b.SitterId == s.SitterId).Select(b => b.Review).ToList(),
                                 petTypes = _db.Sitters.Where(b => b.SitterId == s.SitterId).SelectMany(s => s.PetTypes).Select(p => p.PetType1).ToList(),
                                 availabilities = s.Availabilities.ToList()



                             }).ToList();

            AvailabilityRepo availabilityRepo = new AvailabilityRepo(_db);
            foreach (var sitter in allSitters)
            {
                if (sitter.availabilities != null)
                {
                    sitter.availableDates = availabilityRepo.GetAvailableDates(sitter.availabilities);

                }
            }

            return allSitters.AsQueryable();
        }

        public SitterVM GetSitterVM(int sitterID)
        {
            SitterVM sitter = GetAllSitterVMs().Where(s => s.SitterId == sitterID).FirstOrDefault();

            return sitter;
        }
    }
}
