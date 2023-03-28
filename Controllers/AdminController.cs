using Microsoft.AspNetCore.Mvc;
using PetSitter.Models;
using PetSitter.Repositories;

namespace PetSitter.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly PetSitterContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly SitterRepos sitterRepos;
        private readonly CustomerRepo customerRepo;
        private readonly PetRepo petRepo;
        private readonly AdminRepo adminRepo;
        

        public AdminController(ILogger<AdminController> logger, PetSitterContext context, IWebHostEnvironment webHost)
        {
            _logger            = logger;
            _db                = context;
            webHostEnvironment = webHost;

            sitterRepos  = new SitterRepos(context, webHost);
            customerRepo = new CustomerRepo(context, webHost);
            petRepo      = new PetRepo(context, webHost);
            adminRepo    = new AdminRepo(context, webHost);

        }

        public IActionResult AdminDashboard()
        {
            IQueryable allUsers = adminRepo.GetAllUsers();

            return View(allUsers);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
