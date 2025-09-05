using System.ComponentModel.DataAnnotations;

namespace RentalSite.Pages.Model
{
    public class RegisterInput
    {
        [Required(ErrorMessage = "Invalid Username"),MaxLength(50)]
        public string UserName { get; set; }
        [Required, MaxLength(50)]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Invalid Email"), EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Birthday is Required")]
        public DateOnly Birthday { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(09\d{9}|\+639\d{9})$",
        ErrorMessage = "Invalid phone number. Use 09xxxxxxxxx or +639xxxxxxxxx")]
        public string PhoneNumber { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
