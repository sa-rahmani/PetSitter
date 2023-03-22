using PetSitter.Models;
using PetSitter.ViewModels;

namespace PetSitter.Repositories
{
    public class AdminRepo
    {
        private readonly PetSitterContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminRepo(PetSitterContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IQueryable GetAllUsers() 
        {
          var allUsers = (from user in _db.Users
                            select new AdminDashboardVM
                            {
                                UserID = user.UserId,
                                Email = user.Email,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Role = user.UserType
                            });

            return allUsers;

           
        }
    }
}
