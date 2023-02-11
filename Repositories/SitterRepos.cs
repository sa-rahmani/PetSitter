﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PetSitter.Data;
using PetSitter.Models;
using PetSitter.ViewModels;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace PetSitter.Repositories
{
    public class SitterRepos
    {
        private readonly PetSitterContext _db;

        public SitterRepos(PetSitterContext db)
        {
            _db = db;
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
        public Sitter getSitterById(int sitterId)
        {
            var sitter = (from s in _db.Sitters
                          where s.SitterId == sitterId
                          select s).FirstOrDefault();
            return sitter;
        }
        public User getUser(int? userId)
        {
            var user = (from u in _db.Users
                        where u.UserId == userId
                        select u).FirstOrDefault();
            return user;
        }
        public SitterProfileVM GetSitterByEmail(string email)
        {
            var sitter = (from u in _db.Users
                          from s in _db.Sitters
                          from p in s.PetTypes
                          where u.Email == email
                          select new {
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
                              ProfileBio = s.ProfileBio }).FirstOrDefault();

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
                SelectedPetTypes = petTypeSitter.Select(p=>p.PetType1).ToList()

            };


            return sitterProfileVM;
        }
        public Tuple<int, string> EditSitter(SitterProfileVM sitterProfileVM)
        {
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
                UserType = sitterProfileVM.UserType


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
        public Tuple<int, string> DeleteProfile(SitterProfileVM sitterProfileVM)
        {
            string message = String.Empty;
            Sitter sitter = getSitterById(sitterProfileVM.SitterId);
            User user = getUser(sitter.UserId);
            PetType petType = getPetTypeSitter(sitterProfileVM.SitterId).FirstOrDefault();
           // ClientAccountRepo clientAccountRepo = new ClientAccountRepo(_context);
           // ClientAccount clientAccount = clientAccountRepo.GetClientAccount(bankAccountVM.AccountNum);
            try
            {
                //foreach (var p in petType) {
              

                    //sitter.PetTypes.Remove(petType);

                
                // }
               // _db.SaveChanges();

                _db.Sitters.Remove(sitter);

                _db.Users.Remove(user);

                _db.SaveChanges();

                //bankAccount = new BankAccount
                //{
                //    AccountNum = bankAccountVM.AccountNum,
                //    AccountType = bankAccountVM.AccountType,
                //    Balance = bankAccountVM.Balance
                //};

              //  _context.ClientAccounts.Remove(clientAccount);
               // _context.BankAccounts.Remove(bankAccount);
                //_context.SaveChanges();

                message = $"your account sitter Id:  {sitterProfileVM.SitterId}  deleted successfully";
            }
            catch (Exception e)
            {
                sitterProfileVM.SitterId = -1;
                message = $"Error deleting your bankAccount, error: {e.Message} ";
            }

            return Tuple.Create(sitterProfileVM.SitterId, message);
        }
        public List<SitterDashboardVM> GetBooking(string email)
        {
            SitterProfileVM sitter = GetSitterByEmail(email);

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
                         where b.SitterId == 1 && b.Review != null
                         select b.Review).Count();
            foreach (var b in bookings)
            {
                var endDateString = b.EndDate.HasValue ? b.EndDate.Value.ToString("MM/dd/yyyy") : "";
                var startDateString = b.StartDate.HasValue ? b.StartDate.Value.ToString("MM/dd/yyyy") : "";

                //Check the current date
                DateTime currentDate = DateTime.Now;
                string status ="";
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
                   petParent = b.FirstName +" "+ b.LastName,
                   startDate = startDateString,
                   endDate = endDateString,
                   bookingId = b.BookingId,
                   petType =b.PetType,
                   sitter = sitter.FirstName+" "+sitter.LastName,
                   status = status,
                   upComingNbr=upComing,
                   completeNbr=complete,
                   reviewsNbr=reviews
                 

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
            SitterDashboardVM vm = new SitterDashboardVM { 
            bookingId= booking.BookingId,
            user=user,
            startDate=booking.StartDate.Value.ToString("MM/dd/yyyy"),
            endDate=booking.EndDate.Value.ToString("MM/dd/yyyy"),
            review=booking.Review,
            petType=booking.PetType,
            petParent=user.FirstName +" "+user.LastName
            



            };


            return vm;

        }

    }
}