namespace RentalSite.Pages.Model
{
    public class UserProfileInput
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public IFormFile ProfilePhoto { get; set; }
        public IFormFile BackgroundPhoto { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }
        public string Gender { get; set; }
        public string Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public string FavoriteFandom { get; set; }
        public string EmergencyContact { get; set; }

    }
}
