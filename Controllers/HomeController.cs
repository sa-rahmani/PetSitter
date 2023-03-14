using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetSitter.Data;
using PetSitter.Models;
using System.Diagnostics;

namespace PetSitter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PetSitterContext _db;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, PetSitterContext context, IConfiguration configuration)
        {
            _logger = logger;
            _db = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var sendgridApiKey = _configuration["SendGrid:ApiKey"];
            var connectionString = _configuration["ConnectionStrings:DefaultConnection"];

            ViewData["UserName"] = HttpContext.Session.GetString("UserName");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        //[Authorize]
        //public IActionResult SecureArea()
        //{
        //    Get user name of user who is logged in.
        //     This line must be in the controller.
        //    string userName = User.Identity.Name;

        //    Usually this section would be in a repository.
        //    var registeredUser = _db.Users
        //                                 .Where(ru => ru.Email == userName)
        //                                 .FirstOrDefault();  // return one item

        //    return View(registeredUser);
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}