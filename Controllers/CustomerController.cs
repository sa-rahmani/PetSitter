using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSitter.Models;
using PetSitter.Repositories;
using PetSitter.ViewModels;

namespace PetSitter.Controllers
{
    public class CustomerController : Controller
    {
        private readonly PetSitterContext _db;

        public CustomerController(PetSitterContext db)
        {
            _db = db;
        }   

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FindASitter(int? page)
        {
            // Get an IQueryable of all sitters.
            SitterRepo sitterRepo = new SitterRepo(_db);
            IQueryable<SitterVM> allSitters = sitterRepo.GetAllSitters();

            // Display 10 sitters per page.
            int pageSize = 10;

            return View(PaginatedList<SitterVM>.Create(allSitters.AsNoTracking(), page ?? 1, pageSize));
        }
    }
}
