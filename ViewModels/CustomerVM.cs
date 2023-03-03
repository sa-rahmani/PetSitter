using PetSitter.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetSitter.ViewModels
{
    public class CustomerVM
    { 
        public string? Message { get; set; }

        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string UserType { get; set; }

        public IFormFile? ProfileImage { get; set; }


    }
}
