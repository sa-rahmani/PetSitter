using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetSitter.Data;
using PetSitter.Models;
using PetSitter.Repositories;
using PetSitter.ViewModels;
using System.Diagnostics;
using System.Dynamic;

namespace PetSitter.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly PetSitterContext _db;

        public CustomerController(ILogger<CustomerController> logger, PetSitterContext context)
        {
            _logger = logger;
            _db = context;
        }


        public IActionResult GetProfile(string? message)
        {
            CustomerRepo customerRepo = new CustomerRepo(_db);
            PetRepo petRepo = new PetRepo(_db);
            ViewData["UserName"] = HttpContext.Session.GetString("UserName");

            int customerID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            CustomerVM vm = customerRepo.GetProfile(customerID);

            if (message == null)
            {
                message = "";
            }

            vm.Message = message;

            ViewBag.Pets = petRepo.GetPet();

            return View(vm);
        }

        public IActionResult EditProfile()
        {
            CustomerRepo customerRepo = new CustomerRepo(_db);

            int customerID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            CustomerVM vm = customerRepo.GetProfile(customerID);

            ViewData["UserName"] = HttpContext.Session.GetString("UserName");

            return View(vm);
        }

        [HttpPost]
        public IActionResult EditProfile(CustomerVM customerVM)
        {
            string updateMessage;

            int customerID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            CustomerRepo customerRepo = new CustomerRepo(_db);

            Tuple<int, string> editCustomerRecord = customerRepo.EditProfile(customerVM, customerID);

            updateMessage = editCustomerRecord.Item2;

            
            return RedirectToAction("GetProfile", "Customer",
                 new { id = customerID, message = updateMessage });
        }

        public IActionResult CreatePet()
        {
            ViewData["UserName"] = HttpContext.Session.GetString("UserName");

            return View();
        }

        [HttpPost]
        public IActionResult CreatePet(PetVM petVM)
        {
            //int petID = 0;

            //string createMessage = "";

            int customerID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            
                PetRepo petRepo = new PetRepo(_db);

                Tuple<int, string> response =
                    petRepo.CreatePetRecord(petVM, customerID);

                int petID = response.Item1;
                string createMessage = response.Item2;
            

            return RedirectToAction("GetProfile", "Customer",
                 new { id = petID, message = createMessage });
        }
    }
}
