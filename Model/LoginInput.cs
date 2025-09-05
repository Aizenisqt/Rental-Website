using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RentalSite.Pages.Model
{
    public class LoginInput
    {
        // 🔹 Login Fields
        [Required, MaxLength(50)]
        public string LoginUserName { get; set; }

        [Required, DataType(DataType.Password)]
        public string LoginPassword { get; set; }
    }
}
