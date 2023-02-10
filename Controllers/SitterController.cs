using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetSitter.Models;
using PetSitter.Repositories;
using PetSitter.ViewModels;
using System.Drawing.Drawing2D;

namespace PetSitter.Controllers
{
    public class SitterController : Controller
    {
        private readonly PetSitterContext _db;

        public static int clientID;
        public SitterController(PetSitterContext db)
        {
            _db = db;


        }
        public IActionResult Dashboard()
        {
            SitterRepos sitterRepos = new SitterRepos(_db);
            IEnumerable<SitterDashboardVM> bookings = sitterRepos.GetBooking(User.Identity.Name);
            ViewData["UpComing"] = bookings.Select(b => b.upComingNbr).LastOrDefault();
            ViewData["Complete"] = bookings.Select(b => b.completeNbr).LastOrDefault();
            ViewData["Reviews"] = bookings.Select(b => b.reviewsNbr).LastOrDefault();


            return View(bookings);
        }
        public IActionResult Details(int id)
        {
            SitterRepos sitterRepos = new SitterRepos(_db);
            SitterDashboardVM booking = sitterRepos.GetBookingDetails(id);

            return View(booking);
        }

        public IActionResult Profile(string message)
        {
            SitterRepos sitterRepos = new SitterRepos(_db);
            SitterProfileVM sitterProfileVM = sitterRepos.GetSitterByEmail(User.Identity.Name);
            //sitterProfileVM.PetTypesAvailable = sitterRepos.getPetTypes();
            //sitterProfileVM.SelectedPetTypes = sitterRepos.getPetTypeSitter(sitterProfileVM.SitterId).Select(p => p.PetType1).ToList();
            sitterProfileVM.Message = message;
            return View(sitterProfileVM);
        }
        [HttpPost]
        public IActionResult Profile(SitterProfileVM sitterProfileVM)
        {
                sitterProfileVM.Message = "Invalid entry please try again";
           
            SitterRepos sitterRepos = new SitterRepos(_db);


            if (ModelState.IsValid)
            {
                //sitterProfileVM.SelectedPetTypes = PetTypes?.ToList() ?? new List<string>();


                Tuple<int, string> response = sitterRepos.EditSitter(sitterProfileVM);

                if (response.Item1 < 0)
                {
                    sitterProfileVM.Message = response.Item2;
                }
                else
                {
                    return RedirectToAction("Profile", "Sitter", new { message = response.Item2 });
                }
            }
            //sitterProfileVM.PetTypesAvailable = sitterRepos.getPetTypes();

            //sitterProfileVM.SelectedPetTypes = sitterRepos.getPetTypeSitter(sitterProfileVM.SitterId).Select(p => p.PetType1).ToList();

            return View(sitterProfileVM);
        }

        public IActionResult Delete(int id)
        {
            SitterRepos sitterRepos = new SitterRepos(_db);
            SitterProfileVM sitterProfileVM = sitterRepos.GetSitterByEmail(User.Identity.Name);

            return View(sitterProfileVM);
        }
        [HttpPost]
        public IActionResult Delete(SitterProfileVM sitterProfileVM)
        {
            sitterProfileVM.Message = "Invalid entry please try again";
            if (ModelState.IsValid)
            {
                SitterRepos sitterRepos = new SitterRepos(_db);
                Tuple<int, string> response = sitterRepos.DeleteProfile(sitterProfileVM);

                if (response.Item1 < 0)
                {
                    sitterProfileVM.Message = response.Item2;
                }
                else
                {
                   // var authenticationProperties = new AuthenticationProperties { RedirectUri = "/Home/Index" };
                     HttpContext.SignOutAsync();
                    //return Redirect(authenticationProperties.RedirectUri);
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }

            return View(sitterProfileVM);

        }


    }
}
