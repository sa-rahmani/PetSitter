using PetSitter.Data;
using PetSitter.Models;
using PetSitter.ViewModels;

namespace PetSitter.Repositories
{
    public class PetRepo
    {
        PetSitterContext _db;

        public PetRepo(PetSitterContext context)
        {
            _db = context;
        }

        public Tuple<int, string> CreatePetRecord(PetVM petVM, int userID)
        {
            Pet pet = new Pet();
            string message;
            try
            {
                pet = new Pet
                {
                    Name = petVM.Name,
                    BirthYear = petVM.BirthYear,
                    Sex = petVM.Sex,
                    PetSize = petVM.PetSize,
                    Instructions = petVM.Instructions,
                    UserId = userID,
                    PetType = petVM.PetType
                };

                _db.Pets.Add(pet);
                _db.SaveChanges();

                message = $"Success creating your new pet. " +
                               $"Your new pet number is: {pet.PetId}";
            }
            catch(Exception e)
            {
                pet.PetId = -1;
                message = $"Error creating your new pet, error: {e.Message}";

            }

            return Tuple.Create(pet.PetId, message);
        }

        public IEnumerable<Pet> GetPet()
        {
            var pets = from p in _db.Pets select p;
            return pets;
        }
    }
}
