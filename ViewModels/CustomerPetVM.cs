namespace PetSitter.ViewModels
{
    public class CustomerPetVM
    {
        public string? Message { get; set; }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string UserType { get; set; }

        public int PetId { get; set; }
        public string Name { get; set; }
        public int BirthYear { get; set; }
        public string Sex { get; set; }
        public string PetSize { get; set; }
        public string Instructions { get; set; }
        public string PetType { get; set; }
    }
}
