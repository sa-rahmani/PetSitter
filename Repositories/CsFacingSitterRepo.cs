﻿using Microsoft.AspNetCore.Hosting;
using PetSitter.Models;
using PetSitter.ViewModels;
using System.Data.Entity;
using System.Security.Policy;

namespace PetSitter.Repositories
{
    public class CsFacingSitterRepo
    {

        private readonly PetSitterContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public CsFacingSitterRepo(PetSitterContext db)
        {
            _db = db;
        }

        public IQueryable<SitterVM> GetAllSitterVMs()
        {
            SitterRepos sRepos = new SitterRepos(_db, _webHostEnvironment);


            var allSitters = (from s in _db.Sitters.Include(s=> s.Availabilities)
                             join u in _db.Users
                                    on s.UserId equals u.UserId
                           
                             select new SitterVM
                             {
                                 //SitterId = s.SitterId,
                                 //FirstName = u.FirstName,
                                 //Rate = (decimal)s.RatePerPetPerDay,
                                 //ProfileBio = s.ProfileBio,
                                 //AvgRating = (double)_db.Bookings.Where(b => b.SitterId == s.SitterId).Average(b => b.Rating),
                                 //Reviews = _db.Bookings.Where(b => b.SitterId == s.SitterId).Select(b => b.Review).ToList(),
                                 //petTypes = _db.Sitters.Where(b => b.SitterId == s.SitterId).SelectMany(s => s.PetTypes).Select(p => p.PetType1).ToList(),
                                 //availabilities = s.Availabilities.ToList()

                                 SitterId = s.SitterId,
                                 FirstName = u.FirstName,
                                 Rate = (decimal)s.RatePerPetPerDay,
                                 ProfileBio = s.ProfileBio,
                                 AvgRating = (double)_db.Bookings.Where(b => b.SitterId == s.SitterId).Average(b => b.Rating),
                                 petTypes = _db.Sitters.Where(b => b.SitterId == s.SitterId).SelectMany(s => s.PetTypes).Select(p => p.PetType1).ToList(),
                                 availabilities = s.Availabilities.ToList(),
                                 Reviews = sRepos.GetReviews(s.SitterId).ToList(),
                                 }).ToList();


            AvailabilityRepo availabilityRepo = new AvailabilityRepo(_db);
            foreach (var sitter in allSitters)
            {
                // get a list of bookings for this sitter from the database

                // foreach loop through each booking and create a reviewVM for each one that has a review


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





        public User getUserById(int sitterId)
        {
            var user = (from s in _db.Sitters
                        join u in _db.Users on s.UserId equals u.UserId
                        where s.SitterId == sitterId
                        select u).FirstOrDefault();

            return user;
        }


    }
}
