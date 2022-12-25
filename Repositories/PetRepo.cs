using Microsoft.AspNetCore.Hosting;
using PetSitter.Data;
using PetSitter.Models;
using PetSitter.ViewModels;
using System.Drawing;
using static Humanizer.In;
using System.IO;

namespace PetSitter.Repositories
{
    public class PetRepo
    {
        PetSitterContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public PetRepo(PetSitterContext context, IWebHostEnvironment webHost)
        {
            _db = context;
            webHostEnvironment = webHost;
        }

        public Tuple<int, string> CreatePetRecord(PetVM petVM, int userID)
        {
            Pet pet = new Pet();
            string message;
            string stringFileName = UploadPetFile(petVM);

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
                    PetType = petVM.PetType,
                    PetImage = stringFileName
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

        public IEnumerable<Pet> GetPetData(int id)
        {

            var pets = from p in _db.Pets where p.UserId == id select p;
            return pets;
        }


        public PetVM GetPetRecord(int petID, int userID)
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
                PetType = singlePet.PetType,
                
            };

            return vm;
        }

        public IEnumerable<Pet> GetPetImg(int petID)
        {

            var pets = from p in _db.Pets where p.PetId == petID select p;
            return pets;
        }

        public Tuple<int, string> EditPet(PetVM petVM, int userID)
        {
            string updateMessage;

            string stringFileName = UploadPetFile(petVM);

            Pet pet = new Pet
            {
                PetId = petVM.PetId,
                Name = petVM.Name,
                BirthYear = petVM.BirthYear,
                Sex = petVM.Sex,
                PetSize = petVM.PetSize,
                Instructions = petVM.Instructions,
                UserId = userID,
                PetType = petVM.PetType,
                PetImage = stringFileName
            };

            try
            {
                _db.Update(pet);
                _db.SaveChanges();

                updateMessage = $"Success editing {pet.Name} pet account " + $"Your edited pet number is: {pet.PetId}";
               
            }
            catch (Exception ex)
            {
                updateMessage = ex.Message;
            }
            return Tuple.Create(pet.PetId, updateMessage);
        }

        private string UploadPetFile(PetVM petVM)
        {
            string fileName = null;
            if (petVM.PetImage != null)
            {
                string uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "images");
                fileName = Guid.NewGuid().ToString() + "_" + petVM.PetImage.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    petVM.PetImage.CopyTo(fileStream);
                }
            }
            return fileName;
        }

        public string DeletePetRecord(int petID)
        {
            string deleteMessage;

            var pets = _db.Pets.Where(p => p.PetId == petID).FirstOrDefault();

            try
            {
                _db.Remove(pets);
                _db.SaveChanges();

                deleteMessage = $"Success deleting {pets.Name} pet account, " + $"Your deleted pet number is: {pets.PetId}";
            }
            
            catch (Exception ex)
            {

                deleteMessage = ex.Message;
            }
            return deleteMessage;
        }
    }
}
