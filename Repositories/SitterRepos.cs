﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PetSitter.Data;
using PetSitter.Models;
using PetSitter.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace PetSitter.Repositories
{
    public class SitterRepos
    {
        private readonly PetSitterContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SitterRepos(PetSitterContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public List<string> getPetTypes()
        {
            return _db.PetTypes.Select(p => p.PetType1).ToList();
        }
        public List<PetType> getPetTypeSitter(int sitterId)
        {
            var petTypeSitter = (from s in _db.Sitters
                                 from p in s.PetTypes
                                 where s.SitterId == sitterId
                                 select p).ToList();
            return petTypeSitter;

        }
        //public Sitter getSitterById(int sitterId)
        //{
        //    var sitter = (from s in _db.Sitters
        //                  where s.SitterId == sitterId
        //                  select s).FirstOrDefault();
        //    return sitter;
        //}
        //Add new sitter
        public void AddSiter(Sitter sitter)
        {
            _db.Sitters.Add(sitter);
            _db.SaveChanges();
        }
        public User getUser(int? userId)
        {
            var user = (from u in _db.Users
                        where u.UserId == userId
                        select u).FirstOrDefault();
            return user;
        }
        public Sitter GetSitterByEmail(string email)
        {
            var sitter = _db.Sitters.Where(s => s.User.Email == email).FirstOrDefault();

            return sitter;
        }
        public SitterProfileVM GetSitterById(int sitterId)
        {
            var sitter = (from u in _db.Users
                          from s in _db.Sitters

                          where s.SitterId == sitterId
                          && s.UserId == u.UserId
                          select new
                          {

                              SitterId = s.SitterId,
                              UserId = u.UserId,
                              FirstName = u.FirstName,
                              LastName = u.LastName,
                              Email = u.Email,
                              City = u.City,
                              StreetAddress = u.StreetAddress,
                              PostalCode = u.PostalCode,
                              PhoneNumber = u.PhoneNumber,
                              RatePerPetPerDay = s.RatePerPetPerDay,
                              UserType = u.UserType,
                              ProfileBio = s.ProfileBio
                          }).FirstOrDefault();

            List<PetType> petTypeSitter = getPetTypeSitter(sitter.SitterId);

            SitterProfileVM sitterProfileVM = new SitterProfileVM
            {
                SitterId = sitter.SitterId,
                UserId = sitter.UserId,
                FirstName = sitter.FirstName,
                LastName = sitter.LastName,
                Email = sitter.Email,
                City = sitter.City,
                StreetAddress = sitter.StreetAddress,
                PostalCode = sitter.PostalCode,
                PhoneNumber = sitter.PhoneNumber,
                RatePerPetPerDay = sitter.RatePerPetPerDay,
                ProfileBio = sitter.ProfileBio,
                UserType = sitter.UserType,
                PetTypesAvailable = getPetTypes(),
                SelectedPetTypes = petTypeSitter.Select(p => p.PetType1).ToList()

            };


            return sitterProfileVM;
        }
        public Tuple<int, string> EditSitter(SitterProfileVM sitterProfileVM)
        {
            string stringFileName = UploadCustomerFile(sitterProfileVM);

            List<string> petTypeSitter = getPetTypeSitter(sitterProfileVM.SitterId).Select(p => p.PetType1).ToList();
            PetType petTypeObj = null;



            User sitterBasicInfo = new User
            {
                UserId = sitterProfileVM.UserId,
                FirstName = sitterProfileVM.FirstName,
                LastName = sitterProfileVM.LastName,
                City = sitterProfileVM.City,
                PostalCode = sitterProfileVM.PostalCode,
                StreetAddress = sitterProfileVM.StreetAddress,
                Email = sitterProfileVM.Email,
                PhoneNumber = sitterProfileVM.PhoneNumber,
                UserType = sitterProfileVM.UserType,

                ProfileImage = stringFileName




            };
            var petTypesToInsert = sitterProfileVM.SelectedPetTypes.Except(petTypeSitter).ToList();
            var petTypesToDelete = petTypeSitter.Except(sitterProfileVM.SelectedPetTypes).ToList();


            string message = String.Empty;
            //Sitter sitterObj = (from s in _db.Sitters
            //                 where s.SitterId == sitterProfileVM.SitterId
            //                 select s).FirstOrDefault();

            Sitter sitterObj = _db.Sitters.Include(s => s.PetTypes)
                .FirstOrDefault(s => s.SitterId == sitterProfileVM.SitterId);


            sitterObj.ProfileBio = sitterProfileVM.ProfileBio;
            sitterObj.RatePerPetPerDay = sitterProfileVM.RatePerPetPerDay;
            try
            {
                foreach (var petType in petTypesToInsert)
                {
                    petTypeObj = (from p in _db.PetTypes
                                  where p.PetType1 == petType
                                  select p).FirstOrDefault();
                    sitterObj.PetTypes.Add(petTypeObj);
                    _db.SaveChanges();

                }

                foreach (var petType in petTypesToDelete)
                {
                    petTypeObj = (from p in _db.PetTypes
                                  where p.PetType1 == petType
                                  select p).FirstOrDefault();
                    sitterObj.PetTypes.Remove(petTypeObj);
                    _db.SaveChanges();

                }

                // Check for nulls.
                _db.Users.Update(sitterBasicInfo);
                _db.Sitters.Update(sitterObj);
                _db.SaveChanges();



                message = $"Success editing" +
                    $"{sitterProfileVM.FirstName}";

            }
            catch (Exception e)
            {
                sitterObj.SitterId = -1;

                message = e.Message + " "
    + "The sitter account may not exist or "
    + "there could be a foreign key restriction.";

            }

            return Tuple.Create(sitterObj.SitterId, message);
        }
        private string UploadCustomerFile(SitterProfileVM sitterProfileVM)
        {
            string fileName = null;
            if (sitterProfileVM.ProfileImage != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                fileName = Guid.NewGuid().ToString() + "_" + sitterProfileVM.ProfileImage.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    sitterProfileVM.ProfileImage.CopyTo(fileStream);
                }

            }
            return fileName;
        }

        public List<SitterDashboardVM> GetBooking(int sitterId)
        {
            SitterProfileVM sitter = GetSitterById(sitterId);

            var bookings = from b in _db.Bookings
                           join s in _db.Sitters on b.SitterId equals s.SitterId
                           join u in _db.Users on b.UserId equals u.UserId
                           join bp in _db.BookingPets on b.BookingId equals bp.BookingId
                           join p in _db.Pets on bp.PetId equals p.PetId
                           where b.SitterId == sitter.SitterId
                           select new
                           {
                               b.StartDate,
                               b.EndDate,
                               b.BookingId,
                               u.FirstName,
                               u.LastName,
                               p.PetType
                           };


            List<SitterDashboardVM> vm = new List<SitterDashboardVM>();

            int complete = 0;
            int upComing = 0;
            int reviews = (from b in _db.Bookings

                           where b.SitterId == sitterId && b.Review != null

                           select b.Review).Count();
            foreach (var b in bookings)
            {
                var endDateString = b.EndDate.HasValue ? b.EndDate.Value.ToString("MM/dd/yyyy") : "";
                var startDateString = b.StartDate.HasValue ? b.StartDate.Value.ToString("MM/dd/yyyy") : "";

                //Check the current date
                DateTime currentDate = DateTime.Now;
                string status = "";
                //Compare the date from the database
                if (currentDate.Date.CompareTo(b.EndDate.Value.Date) > 0)
                {
                    //The date has passed, set status to "Complete"
                    status = "Complete";
                    complete++;
                }
                else
                {
                    //The date has not passed, set status to "Upcoming"
                    status = "UpComing";
                    upComing++;
                }
                vm.Add(new SitterDashboardVM
                {
                    petParent = b.FirstName + " " + b.LastName,
                    startDate = startDateString,
                    endDate = endDateString,
                    bookingId = b.BookingId,
                    petType = b.PetType,
                    sitter = sitter.FirstName + " " + sitter.LastName,
                    status = status,
                    upComingNbr = upComing,
                    completeNbr = complete,
                    reviewsNbr = reviews


                });
            }

            return vm;

        }
        public SitterDashboardVM GetBookingDetails(int bookingId)
        {
            var booking = (from b in _db.Bookings
                           join s in _db.Sitters on b.SitterId equals s.SitterId
                           join u in _db.Users on b.UserId equals u.UserId
                           join bp in _db.BookingPets on b.BookingId equals bp.BookingId
                           join p in _db.Pets on bp.PetId equals p.PetId
                           where b.BookingId == bookingId
                           select new
                           {
                               b.StartDate,
                               b.EndDate,
                               b.BookingId,
                               u.UserId,
                               p.PetType,
                               b.Review,


                           }).FirstOrDefault();

            User user = getUser(booking.UserId);
            SitterDashboardVM vm = new SitterDashboardVM
            {
                bookingId = booking.BookingId,
                user = user,
                startDate = booking.StartDate.Value.ToString("MM/dd/yyyy"),
                endDate = booking.EndDate.Value.ToString("MM/dd/yyyy"),
                review = booking.Review,
                petType = booking.PetType,
                petParent = user.FirstName + " " + user.LastName




            };


            return vm;

        }
        public Tuple<int, string> AddAvailability(SitterAvailabilityVM availabilityVM)

        {
            Availability availability = new Availability
            {
                StartDate = availabilityVM.StartDate,
                EndDate = availabilityVM.EndDate

            };
            string message = String.Empty;

            try
            {
                // Add the availability to the database
                _db.Availabilities.Add(availability);
                _db.SaveChanges();

                // Get the current sitter and add the availability to their list of availabilities
                var currentSitter = _db.Sitters.Include(s => s.Availabilities).FirstOrDefault(s => s.SitterId == availabilityVM.SitterId);
                currentSitter.Availabilities.Add(availability);
                _db.SaveChanges();


                message = $"Success adding new availability";

            }
            catch (Exception e)
            {
                availability.AvailabilityId = -1;

                message = e.Message + " "
    + "The sitter account may not exist or "
    + "there could be a foreign key restriction.";

            }

            return Tuple.Create(availability.AvailabilityId, message);
        }





        public List<ReviewVM> GetReviews(int sitterId)
        {
            SitterProfileVM sitter = GetSitterById(sitterId);

            List<ReviewVM> vm = new List<ReviewVM>();

            //SitterProfileVM sitter = GetSitterByEmail(email);
            var reviews = (from b in _db.Bookings
                           join u in _db.Users on b.UserId equals u.UserId
                           where b.SitterId == sitterId && b.Review != null
                           select new
                           {
                               u.FirstName,
                               u.LastName,
                               b.Review,
                               b.Rating,
                               b.StartDate,
                               b.EndDate
                           });

            foreach (var r in reviews)
            {
                vm.Add(new ReviewVM
                {
                    petParent = r.FirstName + " " + r.LastName,
                    startDate = r.StartDate,
                    endDate = r.EndDate,
                    rating = r.Rating,
                    review = r.Review

                });


            }
            return vm;
        }





        //public List<ReviewVM> GetReviews(int sitterId)
        //{
        //    SitterProfileVM sitter = GetSitterById(sitterId);

        //    List<ReviewVM> vm = new List<ReviewVM>();

        //    //SitterProfileVM sitter = GetSitterByEmail(email);
        //    var reviews = (from b in _db.Bookings
        //                   join u in _db.Users on b.UserId equals u.UserId
        //                   where b.SitterId == sitterId && b.Review != null
        //                   select new
        //                   {
        //                       u.FirstName,
        //                       u.LastName,
        //                       b.Review,
        //                       b.Rating,
        //                       b.StartDate,
        //                       b.EndDate
        //                   });

        //    foreach (var r in reviews)
        //    {
        //        vm.Add(new ReviewVM
        //        {
        //            petParent = r.FirstName + " " + r.LastName,
        //            startDate = r.StartDate,
        //            endDate = r.EndDate,
        //            rating = r.Rating,
        //            review = r.Review

        //        });


        //    }


        //    return vm;





        //}
    }
}
