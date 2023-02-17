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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}