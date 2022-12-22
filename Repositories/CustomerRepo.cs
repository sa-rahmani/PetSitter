using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using PetSitter.Data;
using PetSitter.Models;
using PetSitter.ViewModels;
using System.Drawing;

namespace PetSitter.Repositories
{
    public class CustomerRepo
    {
        PetSitterContext _db;

        public CustomerRepo(PetSitterContext context)
        {
            _db = context;
        }

        public void AddUser(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public User GetCustomerId(string email)
        {
            var customers = _db.Users.Where(u => u.Email == email).FirstOrDefault();
           
            return customers;
        }

        public CustomerPetVM GetProfile(int userID)
        {
            var singleUser = _db.Users.Where(u => u.UserId == userID).FirstOrDefault();

            CustomerPetVM vm = new CustomerPetVM
            {
                UserId = userID,
                FirstName = singleUser.FirstName,
                LastName = singleUser.LastName,
                Email = singleUser.Email,
                PostalCode = singleUser.PostalCode,
                PhoneNumber = singleUser.PhoneNumber,
                StreetAddress = singleUser.StreetAddress,
                City = singleUser.City,
                UserType = singleUser.UserType,

            };

            return vm;
        }


        //public List<CustomerPetVM> GetUserAndPetRecords(int id)
        //{
        //    List<CustomerPetVM> vm = new List<CustomerPetVM>();

        //    var userAccounts =
        //                  _db.Users
        //                  .Join(_db.Pets, u => u.UserId, p => p.UserId,
        //                  (u, p) => new {
        //                       UserId = u.UserId,
        //                       FirstName = u.FirstName,
        //                       LastName = u.LastName,
        //                       Email = u.Email,
        //                       PostalCode = u.PostalCode,
        //                       PhoneNumber = u.PhoneNumber,
        //                       StreetAddress = u.StreetAddress,
        //                       City = u.City,
        //                       UserType = u.UserType,
        //                       PetId = p.PetId,
        //                       Name = p.Name,
        //                       BirthYear = p.BirthYear,
        //                       Sex = p.Sex,
        //                       PetSize = p.PetSize,
        //                       Instructions = p.Instructions,
        //                       PetType = p.PetType
        //                  }).Where(p=> p.UserId == id);

            
        //    foreach (var userAccount in userAccounts)
        //    {

        //        vm.Add(new CustomerPetVM
        //        {
        //            UserId = userAccount.UserId,
        //            FirstName = userAccount.FirstName,
        //            LastName = userAccount.LastName,
        //            Email = userAccount.Email,
        //            PostalCode = userAccount.PostalCode,
        //            PhoneNumber = userAccount.PhoneNumber,
        //            StreetAddress = userAccount.StreetAddress,
        //            City = userAccount.City,
        //            UserType = userAccount.UserType,
        //            PetId = userAccount.PetId,
        //            Name = userAccount.Name,
        //            BirthYear = (int)userAccount.BirthYear,
        //            Sex = userAccount.Sex,
        //            PetSize = userAccount.PetSize,
        //            Instructions = userAccount.Instructions,
        //            PetType = userAccount.PetType
        //        });
        //    }

        //    //CustomerVM vm = new CustomerVM
        //    //{
        //    //    UserId = userID,
        //    //    FirstName = singleUser.FirstName,
        //    //    LastName = singleUser.LastName,
        //    //    Email = singleUser.Email,
        //    //    PostalCode = singleUser.PostalCode,
        //    //    PhoneNumber = singleUser.PhoneNumber,
        //    //    StreetAddress = singleUser.StreetAddress,
        //    //    City = singleUser.City,
        //    //    UserType = singleUser.UserType,
        //    //    PetId = pets.PetId,
        //    //    Name = pets.Name,

        //    //};

        //    return vm;
        //}

        //public IEnumerable<CustomerPetVM> getAllLists(int id)
        //{
        //    var vmList = from u in _db.Users
        //                 join p in _db.Pets
        //                         on u.UserId equals p.UserId into up
        //                 from petResult in up
        //                 where u.UserId == id

        //                 select new CustomerPetVM
        //                 {
        //                     UserId = u.UserId,
        //                     FirstName = u.FirstName,
        //                     LastName = u.LastName,
        //                     Email = u.Email,
        //                     PostalCode = u.PostalCode,
        //                     PhoneNumber = u.PhoneNumber,
        //                     StreetAddress = u.StreetAddress,
        //                     City = u.City,
        //                     UserType = u.UserType,
        //                     PetId = petResult.PetId,
        //                     Name = petResult.Name,
        //                     BirthYear = (int)petResult.BirthYear,
        //                     Sex = petResult.Sex,
        //                     PetSize = petResult.PetSize,
        //                     Instructions = petResult.Instructions,
        //                     PetType = petResult.PetType
        //                 };

        //    return vmList;
        //}

        public Tuple<int, string> EditProfile(CustomerVM customerVM, int userID)
        {
            string updateMessage;
            User user = new User
            {
                UserId = userID,
                FirstName = customerVM.FirstName,
                LastName = customerVM.LastName,
                Email = customerVM.Email,
                PostalCode = customerVM.PostalCode,
                PhoneNumber = customerVM.PhoneNumber,
                StreetAddress = customerVM.StreetAddress,
                City = customerVM.City,
                UserType = customerVM.UserType
            };

            try
            {
                _db.Update(user);
                _db.SaveChanges();

                updateMessage = $"Success editing {user.FirstName} user account " + $"Your edited user number is: {user.UserId}";
            }
            catch (Exception ex)
            {
                updateMessage = ex.Message;
            }
            return Tuple.Create(user.UserId, updateMessage);
        }
    }
}
