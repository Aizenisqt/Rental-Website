using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography.X509Certificates;
using MySql.Data.MySqlClient;
using RentalSite.Pages.Model;
namespace RentalSite.Pages.Account
{
    public class AddItemModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AddItemModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            
        }

        public string UserID { get; set; }  
        public int id { get; set; }
        [BindProperty]
        public AddInputModel AddItem { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            UserID = user.Id;

            string connectionString = "server=localhost;database=rentalsite;user=root;password=aizenisqt18;port=3307";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
;
                    string Userquery = "SELECT id FROM users WHERE UserLoginId = @UserLoginId";
                    using (var Usercmd = new MySqlCommand(Userquery, conn))
                    {
                        Usercmd.Parameters.AddWithValue("@UserLoginId", UserID);

                        using (var reader = Usercmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                id = reader.GetInt32("id");
                            }
                        }
                    }


                    List<String> savepath = new List<string>();
                    if (AddItem.ProductPhoto != null && AddItem.ProductPhoto.Count > 0)
                    {
                        foreach (var file in AddItem.ProductPhoto)
                        {
                            if (file.Length > 0)
                            {
                                var filename = Path.GetFileName(file.FileName);
                                var filepath = Path.Combine("wwwroot/Uploads", filename);

                                using (var stream = new FileStream(filepath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }
                                savepath.Add("Uploads/" + filename);
                            }

                        }

                       
                    }
                    string photoString = string.Join(",", savepath);


                    string query = @"INSERT INTO iteminfo (UserLoginId,UserId,Title,ItemPhoto,Description,Inclusion,Price)
                                     VALUES
                                     (@UserLoginId,@UserId,@Title,@ItemPhoto,@Description,@Inclusion,@Price)";
                    using (MySqlCommand cmd = new MySqlCommand(query,conn))
                    {
                        cmd.Parameters.AddWithValue("@UserLoginId", UserID);
                        cmd.Parameters.AddWithValue("@UserId", id);
                        cmd.Parameters.AddWithValue("@Title", AddItem.Title);
                        cmd.Parameters.AddWithValue("@ItemPhoto", photoString);
                        cmd.Parameters.AddWithValue("@Description", AddItem.Description);
                        cmd.Parameters.AddWithValue("@Inclusion", AddItem.Inclusion);
                        cmd.Parameters.AddWithValue("@Price", AddItem.Price);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    
                }
                return RedirectToPage("/UserProfile");
            }
        }
        public void OnGet()
        {
        }
    }
}
