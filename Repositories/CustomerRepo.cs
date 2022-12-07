using PetSitter.Data;
using PetSitter.Models;

namespace PetSitter.Repositories
{
    public class CustomerRepo
    {
        PetSitterContext _db;

        public CustomerRepo(PetSitterContext context)
        {
            _db = context;
        }

        public void AddUser(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }
    }
}
