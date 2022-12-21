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

        public IEnumerable<Pet> GetPetNameLists()
        {
            var pets = from p in _db.Pets select p;
            return pets;
        }


        public PetVM GetPetInform(int petID, int userID)
        {
            var singlePet = _db.Pets.Where(p => p.PetId == petID).FirstOrDefault();

            PetVM vm = new PetVM
            {
                PetId = singlePet.PetId,
                Name = singlePet.Name,
                BirthYear = (int)singlePet.BirthYear,
                Sex = singlePet.Sex,
                PetSize = singlePet.PetSize,
                Instructions = singlePet.Instructions,
                UserId = userID,
                PetType = singlePet.PetType
            };

            return vm;
        }

        public Tuple<int, string> EditPet(PetVM petVM, int userID)
        {
            string updateMessage;
            Pet pet = new Pet
            {
                PetId = petVM.PetId,
                Name = petVM.Name,
                BirthYear = petVM.BirthYear,
                Sex = petVM.Sex,
                PetSize = petVM.PetSize,
                Instructions = petVM.Instructions,
                UserId = userID,
                PetType = petVM.PetType
            };

            try
            {
                _db.Update(pet);
                _db.SaveChanges();

                updateMessage = $"Success editing {pet.Name}'s account No.{pet.PetId}";
            }
            catch (Exception ex)
            {
                updateMessage = ex.Message;
            }
            return Tuple.Create(pet.PetId, updateMessage);
        }
    }
}
