using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using RentalSite.Pages.Model;
using System.ComponentModel.DataAnnotations;

namespace RentalSite.Pages.Account
{
    public class LoginAndRegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginAndRegisterModel> _logger;

        //for user login and registration
        public LoginAndRegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<LoginAndRegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty] 
        public RegisterInput Register { get; set; }
        [BindProperty] 
        public LoginInput Login { get; set; }



        //This is for User Register
        public async Task<IActionResult> OnPostCreateUserAsync()
        {
            ModelState.Remove(nameof(Login.LoginUserName));
            ModelState.Remove(nameof(Login.LoginPassword));

            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = Register.UserName,
                    Email = Register.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, Register.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Insert extra info into custom MySQL table
                    string connectionString = "server=localhost;database=rentalsite;user=root;password=aizenisqt18;port=3307";
                    using var conn = new MySqlConnection(connectionString);
                    await conn.OpenAsync();

                    string query = @"INSERT INTO users 
                             (Username,Email,UserLoginId,FullName,Gender,Birthday,PhoneNumber) 
                             VALUES 
                             (@Username,@Email,@Id,@FullName,@Gender,@Birthday,@PhoneNumber)";
                    using var cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Username", Register.UserName);
                    cmd.Parameters.AddWithValue("@Email", Register.Email);
                    cmd.Parameters.AddWithValue("@Id", user.Id);
                    cmd.Parameters.AddWithValue("@FullName", Register.FullName);
                    cmd.Parameters.AddWithValue("@Gender", Register.Gender);
                    cmd.Parameters.AddWithValue("@Birthday", Register.Birthday);
                    cmd.Parameters.AddWithValue("@PhoneNumber", Register.PhoneNumber);
                    await cmd.ExecuteNonQueryAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToPage("/UserProfile");
                }

                // If Identity creation fails
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Registration failed: {Code} - {Description}", error.Code, error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }



        //this is for User Loging
        public async Task<IActionResult> OnPostUserLoginAsync()
        {



            ModelState.Remove(nameof(Register.FullName));
            ModelState.Remove(nameof(Register.UserName));
            ModelState.Remove(nameof(Register.Gender));
            ModelState.Remove(nameof(Register.PhoneNumber));
            ModelState.Remove(nameof(Register.Birthday));
            ModelState.Remove(nameof(Register.Email));
            ModelState.Remove(nameof(Register.Password));
            ModelState.Remove(nameof(Register.ConfirmPassword));

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync
                    (Login.LoginUserName,
                     Login.LoginPassword,
                    isPersistent: false,
                    lockoutOnFailure:  false);

                if (result.Succeeded)
                {
                    //var user = await _userManager.FindByNameAsync(Login.LoginUserName);
                    //return RedirectToPage("/UserProfile", new { id = user.Id });



                    return RedirectToPage("/UserProfile");
                }

                ModelState.AddModelError("","Invalid Login Attempt");
            }

            return Page();
        }
        public void OnGet()
        {
        }
    }
}
