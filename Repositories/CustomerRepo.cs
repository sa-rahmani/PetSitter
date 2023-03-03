﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using PetSitter.Data;
using PetSitter.Models;
using PetSitter.ViewModels;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;

namespace PetSitter.Repositories
{
    public class CustomerRepo
    {
        PetSitterContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CustomerRepo(PetSitterContext context, IWebHostEnvironment webHost)
        {
            _db = context;
            webHostEnvironment = webHost;
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

        public CustomerVM GetProfile(int customerID)
        {
            var singleUser = _db.Users.Where(u => u.UserId == customerID).FirstOrDefault();

            CustomerVM vm = new CustomerVM
            {
                CustomerId = customerID,
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

        public IEnumerable<User> GetUserData(int id)
        {

            var users = from u in _db.Users where u.UserId == id select u;
            return users;
        }

        public Tuple<int, string> EditProfile(CustomerVM customerVM, int userID)
        {
            string updateMessage;

            string stringFileName = UploadCustomerFile(customerVM);

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
                UserType = customerVM.UserType,
                ProfileImage = stringFileName
            };

            try
            {
                _db.Entry(user).State = EntityState.Modified;
                if (user.ProfileImage == null)
                {
                    _db.Entry(user).Property(u => u.ProfileImage).IsModified = false;

                }
                _db.SaveChanges();

                updateMessage = $"Success editing {user.FirstName} user account " + $"Your edited user number is: {user.UserId}";
            }
            catch (Exception ex)
            {
                updateMessage = ex.Message;
            }
            return Tuple.Create(user.UserId, updateMessage);
        }


        private string UploadCustomerFile(CustomerVM customerVM)
        {
            string fileName = null;
            if (customerVM.ProfileImage != null)
            {
                string uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "images");
                fileName = Guid.NewGuid().ToString() + "_" + customerVM.ProfileImage.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    customerVM.ProfileImage.CopyTo(fileStream);
                }
            } 
            return fileName;
        }
    }
}

