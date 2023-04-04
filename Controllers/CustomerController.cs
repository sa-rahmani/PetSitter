using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetSitter.Data;
using PetSitter.Models;
using PetSitter.Repositories;
using PetSitter.ViewModels;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;

namespace PetSitter.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly PetSitterContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;
        public CustomerController(ILogger<CustomerController> logger, PetSitterContext context, IWebHostEnvironment webHost)
        {
            _logger = logger;
            _db = context;
            webHostEnvironment = webHost;
        }


        public IActionResult GetProfile()
        {
            PetRepo petRepo = new PetRepo(_db, webHostEnvironment);
            ViewData["UserName"] = HttpContext.Session.GetString("UserName");

            int customerID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            CustomerRepo customerRepo = new CustomerRepo(_db, webHostEnvironment);

            CustomerVM vm = customerRepo.GetProfile(customerID);

            ViewData["PetLists"] = petRepo.GetPetLists(customerID);
            ViewData["UserData"] = customerRepo.GetUserData(customerID);

            return View(vm);
        }

        public IActionResult EditProfile()
        {
            CustomerRepo customerRepo = new CustomerRepo(_db, webHostEnvironment);
            PetRepo petRepo = new PetRepo(_db, webHostEnvironment);

            int customerID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            CustomerVM vm = customerRepo.GetProfile(customerID);

            ViewData["UserName"] = HttpContext.Session.GetString("UserName");

            ViewData["PetLists"] = petRepo.GetPetLists(customerID);

            return View(vm);
        }


        [HttpPost]
        public IActionResult EditProfile(CustomerVM customerVM)
        {
            string updateMessage; 

            int customerID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            CustomerRepo customerRepo = new CustomerRepo(_db, webHostEnvironment);

            Tuple<int, string> editCustomerRecord = customerRepo.EditProfile(customerVM, customerID);

            updateMessage = editCustomerRecord.Item2;

            return RedirectToAction("GetProfile", "Customer",
                 new { id = customerID, message = updateMessage });
        }
        
       
        public IActionResult CreatePet()
        {
            ViewData["UserName"] = HttpContext.Session.GetString("UserName");

            PetVM vm = new PetVM()
            {
                AvailableSizes = new List<string>() { "0-15lb", "15lb-40lb", "40lb-70lb", "70lb-100lb", "100lb-150lb", "150lb-" }
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult CreatePet(PetVM petVM)
        {

            int customerID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            PetRepo petRepo = new PetRepo(_db, webHostEnvironment);

            Tuple<int, string> response =
                petRepo.CreatePetRecord(petVM, customerID);

            int petID = response.Item1;
            string createMessage = response.Item2;


            return RedirectToAction("GetProfile", "Customer",
                 new { id = petID, message = createMessage });
        }

        public IActionResult GetPet(int id)
        {
            PetRepo petRepo = new PetRepo(_db, webHostEnvironment);

            int customerID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            Pet pet = petRepo.GetPetDetailRecord(id, customerID);

            ViewData["UserName"] = HttpContext.Session.GetString("UserName");
            ViewData["PetData"] = petRepo.GetPetData(id);


            return View(pet);
        }

        public IActionResult EditPet(int id)
        {
            PetRepo petRepo = new PetRepo(_db, webHostEnvironment);

            int customerID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            PetVM vm = petRepo.GetPetEditRecord(id, customerID);
            vm.AvailableSizes = new List<string>() { "0-15lb", "15lb-40lb", "40lb-70lb", "70lb-100lb", "100lb-150lb", "150lb-" };

            ViewData["UserName"] = HttpContext.Session.GetString("UserName");
            ViewData["PetData"] = petRepo.GetPetData(id);

           
            return View(vm);
        }

        [HttpPost]
        public IActionResult EditPet(PetVM petVM)
        {

            int customerID = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            PetRepo petRepo = new PetRepo(_db, webHostEnvironment);

            Tuple<int, string> editPetRecord = petRepo.EditPet(petVM, customerID);

            int petID = editPetRecord.Item1;
            string updateMessage = editPetRecord.Item2;

         

            return RedirectToAction("GetPet", "Customer",
                 new { id = petID, message = updateMessage });
        }


        public IActionResult DeletePet(int id)
        {
            string deleteMessage;

            PetRepo petRepo = new PetRepo(_db, webHostEnvironment);

            deleteMessage = petRepo.DeletePetRecord(id);

            return RedirectToAction("GetProfile", "Customer",
                 new { message = deleteMessage });
        }

    }
}
