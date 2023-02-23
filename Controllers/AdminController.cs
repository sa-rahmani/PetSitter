using Microsoft.AspNetCore.Mvc;
using PetSitter.Models;

namespace PetSitter.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly PetSitterContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;
     

        public AdminController(ILogger<AdminController> logger, PetSitterContext context, IWebHostEnvironment webHost)
        {
            _logger = logger;
            _db = context;
            webHostEnvironment = webHost;


        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
