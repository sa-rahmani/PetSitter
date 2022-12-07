using System.ComponentModel;

namespace PetSitter.ViewModels
{
    public class SitterVM
    {
        public int SitterId { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Rate per Day per Pet")]
        public decimal Rate { get; set; }

        [DisplayName("About")]
        public string ProfileBio { get; set; }

        [DisplayName("Average Rating")]
        public double? AvgRating { get; set; }

        public List<string>? Reviews { get; set; }

    }
}
