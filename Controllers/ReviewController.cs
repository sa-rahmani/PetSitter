using Microsoft.AspNetCore.Mvc;
using PetSitter.Models;

namespace PetSitter.Controllers
{
    public class ReviewController : Controller
    {

        private readonly PetSitterContext _db;

        public ReviewController(PetSitterContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }



    }
}
