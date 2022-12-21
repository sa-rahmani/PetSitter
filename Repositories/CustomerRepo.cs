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

        public CustomerVM GetProfile(int userID)
        {
            var singleUser = _db.Users.Where(u => u.UserId == userID).FirstOrDefault();

            CustomerVM vm = new CustomerVM
            {
                UserId = userID,
                FirstName = singleUser.FirstName,
                LastName = singleUser.LastName,
                Email = singleUser.Email,
                PostalCode = singleUser.PostalCode,
                PhoneNumber = singleUser.PhoneNumber,
                StreetAddress = singleUser.StreetAddress,
                City = singleUser.City,
                UserType = singleUser.UserType
            };

            return vm;
        }

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
