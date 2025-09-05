using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using RentalSite.Pages.Model;
using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace RentalSite.Pages
{
    [Authorize]
    public class UserProfileModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public List<Users> users { get; set; } = new List<Users>();
        public List<ItemInfo> Item { get; set; } = new List<ItemInfo>();
        public UserProfileModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public string id { get; set; }
    
        [BindProperty]
        public UserProfileInput UserInput { get; set; }
        [BindProperty]
        public EditItem editItem { get; set; }
        public async Task<IActionResult> OnPostEditInfoAsync()
        {
            string connectionString = "server=localhost;database=rentalsite;user=root;password=aizenisqt18;port=3307";

            var user = await _userManager.GetUserAsync(User);
            id = user.Id;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query;


                    string ProfileimagePath = null;
                    string BackgroundimagePath = null;


                    // Load existing user data first
                    string selectQuery = "SELECT * FROM users WHERE UserLoginId = @Id";
                    Users existingUser = null;
                    using (MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@Id", id);
                        using (var reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                existingUser = new Users
                                {
                                    UserName = reader.GetString("Username"),
                                    Email = reader.GetString("Email"),
                                    FullName = reader.GetString("FullName"),
                                    Bio = reader.GetString("Bio"),
                                    Gender = reader.GetString("Gender"),
                                    Birthday = reader.GetString("Birthday"),
                                    PhoneNumber = reader.GetString("PhoneNumber"),
                                    Address = reader.GetString("Address"),
                                    Fandom = reader.GetString("FavoriteFandom"),
                                    EmergencyContact = reader.GetString("EmergencyContact")
                                };
                            }
                        }
                    }


                    if (UserInput.ProfilePhoto != null && UserInput.ProfilePhoto.Length > 0)
                    {
                        string Profilefilename = Path.GetFileName(UserInput.ProfilePhoto.FileName);
                        string profileSavePath = Path.Combine("wwwroot/Image", Profilefilename);
                        ProfileimagePath = "Image/" + Profilefilename;

                        using (var Profilefilestream = new FileStream(profileSavePath, FileMode.Create))
                        {
                            await UserInput.ProfilePhoto.CopyToAsync(Profilefilestream);
                        }
                    }

                    if (UserInput.BackgroundPhoto != null && UserInput.BackgroundPhoto.Length > 0)
                    {
                        string Backgroundfilename = Path.GetFileName(UserInput.BackgroundPhoto.FileName);
                        string backgroundSavePath = Path.Combine("wwwroot/Image", Backgroundfilename);
                        BackgroundimagePath = "Image/" + Backgroundfilename;

                        using (var Backgroundfilestream = new FileStream(backgroundSavePath, FileMode.Create))
                        {
                            await UserInput.BackgroundPhoto.CopyToAsync(Backgroundfilestream);
                        }
                    }


                    query = @"UPDATE users SET Username = @Username, Email = @Email, FullName = @Fullname,Bio = @Bio,
                              Gender = @Gender,Birthday = @Birthday, PhoneNumber = @Phonenumber, Address = @Address,
                              EmergencyContact = @Emergency, FavoriteFandom = @FavoriteFandom";

                    if (UserInput.ProfilePhoto != null)
                    {
                        query += ",ProfilePhoto = @ProfilePhoto";
                    }
                    if (UserInput.BackgroundPhoto != null)
                    {
                        query += ",BackgroundPhoto = @BackgroundPhoto";
                    }

                    query += " WHERE UserLoginId = @Id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id",id);
                        cmd.Parameters.AddWithValue("@Username", 
                               string.IsNullOrEmpty(UserInput.UserName) ? existingUser.UserName : UserInput.UserName);
                        cmd.Parameters.AddWithValue("@Email",
                               string.IsNullOrEmpty(UserInput.Email) ? existingUser.Email: UserInput.Email);
                        cmd.Parameters.AddWithValue("@Fullname",
                               string.IsNullOrEmpty(UserInput.FullName) ? existingUser.FullName : UserInput.FullName);
                        cmd.Parameters.AddWithValue("@Bio",
                               string.IsNullOrEmpty(UserInput.Bio) ? existingUser.Bio : UserInput.Bio);
                        cmd.Parameters.AddWithValue("@Gender",
                               string.IsNullOrEmpty(UserInput.Gender) ? existingUser.Gender : UserInput.Gender);
                        cmd.Parameters.AddWithValue("@Birthday",
                               string.IsNullOrEmpty(UserInput.Birthday) ? existingUser.Birthday : UserInput.Birthday);
                        cmd.Parameters.AddWithValue("@Phonenumber",
                               string.IsNullOrEmpty(UserInput.PhoneNumber) ? existingUser.PhoneNumber : UserInput.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Address",
                               string.IsNullOrEmpty(UserInput.Address) ? existingUser.Address : UserInput.Address);
                        cmd.Parameters.AddWithValue("@FavoriteFandom",
                               string.IsNullOrEmpty(UserInput.FavoriteFandom) ? existingUser.Fandom : UserInput.FavoriteFandom);
                        cmd.Parameters.AddWithValue("@Emergency",
                               string.IsNullOrEmpty(UserInput.EmergencyContact) ? existingUser.EmergencyContact : UserInput.EmergencyContact);

                        if (UserInput.ProfilePhoto != null)
                        {
                            cmd.Parameters.AddWithValue("@ProfilePhoto", ProfileimagePath);

                        }
                        if (UserInput.BackgroundPhoto != null)
                        {
                            cmd.Parameters.AddWithValue("@BackgroundPhoto", BackgroundimagePath);

                        }

                        await cmd.ExecuteNonQueryAsync();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Page();
                }
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostEditItemAsync(int id)
        {
            string connectionString = "server=localhost;database=rentalsite;user=root;password=aizenisqt18;port=3307";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                   List<string> SavePath = new List<string>();

                    string selectQuery = "SELECT * FROM iteminfo WHERE Id = @Id";
                    ItemInfo existingItem = null;
                    using (var selectCmd = new MySqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@Id", id);
                        using (var reader = await selectCmd.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                existingItem = new ItemInfo
                                {
                                    Title = reader.GetString("Title"),
                                    Description = reader.GetString("Description"),
                                    Inclusion = reader.GetString("Inclusion"),
                                    Price = reader.GetString("Price"),
                                    ItemPhoto = reader.GetString("ItemPhoto")
                                };
                            }
                        }
                    }






                    string query;
                    if (editItem.ProductPhoto != null && editItem.ProductPhoto.Count > 0)
                    {
                        foreach (var file in editItem.ProductPhoto)
                        {
                            if (file.Length > 0)
                            {
                                var filename = Path.GetFileName(file.FileName);
                                var filepath = Path.Combine("wwwroot/Uploads", filename);

                                using (var filestream = new FileStream(filepath, FileMode.Create))
                                {
                                    await file.CopyToAsync(filestream);
                                }

                                SavePath.Add("Uploads/" + filename);
                            }
                          
                        }

                    }
                    string photoString = string.Join(",", SavePath);

                     query = @"UPDATE iteminfo SET Title = @Title, Description = @Description, 
                                            Inclusion = @Inclusion,
                                            Price = @Price ";
                   

                    if (editItem.ProductPhoto != null && editItem.ProductPhoto.Count > 0)
                    {
                        query += ", ItemPhoto = @ItemPhoto";
                    }

                    query += " WHERE Id = @Id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Title", string.IsNullOrEmpty(editItem.Title) ? existingItem.Title : editItem.Title);
                        cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(editItem.Description) ? existingItem.Description : editItem.Description);
                        cmd.Parameters.AddWithValue("@Inclusion", string.IsNullOrEmpty(editItem.Inclusion) ? existingItem.Inclusion : editItem.Inclusion);
                        cmd.Parameters.AddWithValue("@Price", string.IsNullOrEmpty(editItem.Price) ? existingItem.Price : editItem.Price);


                        if (editItem.ProductPhoto != null && editItem.ProductPhoto.Count > 0)
                        {
                            cmd.Parameters.AddWithValue("@ItemPhoto", photoString);
                        }

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Page();
                }

                return RedirectToPage("UserProfile");
            }
        }

        public async Task<IActionResult> OnPostDeleteItemAsync(int id)
        {

            string connectionString = "server=localhost;database=rentalsite;user=root;password=aizenisqt18;port=3307";


            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = "DELETE FROM iteminfo WHERE Id = @Id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToPage("/UserProfile", new { message = "Item not found" });
                }
                return RedirectToPage("/UserProfile");
            }
        }

        public async Task OnGet()
        {
            var user = await _userManager.GetUserAsync(User);   
            id = user.Id;
            
       
            Console.WriteLine("User.Id: " + user.Id);

            //CurrentUser = await _userManager.FindByIdAsync(id);

            string connectionString = "server=localhost;database=rentalsite;user=root;password=aizenisqt18;port=3307";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM users WHERE UserLoginId = @Id";
                using (MySqlCommand cmd = new MySqlCommand(query,conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new Users
                            {
                                Id = reader.GetInt32("id"),
                                UserName =reader.GetString("Username"),
                                Email = reader.GetString("Email"),
                                UserLoginId = reader.GetString("UserLoginId"),
                                ProfilePhoto = reader.GetString("ProfilePhoto"),
                                BackgroundPhoto = reader.GetString("BackgroundPhoto"),
                                FullName = reader.GetString("FullName"), 
                                Bio = reader.GetString("Bio"),
                                Gender = reader.GetString("Gender"),
                                Birthday = reader.GetString("Birthday"),
                                PhoneNumber = reader.GetString("PhoneNumber"),
                                Address = reader.GetString("Address"),
                                Fandom = reader.GetString("FavoriteFandom"),
                                EmergencyContact = reader.GetString("EmergencyContact")
                            });
                        }
                    }
                }


                string querys = "SELECT * FROM iteminfo WHERE UserLoginId = @id ";
                using (MySqlCommand cmd = new MySqlCommand(querys, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Item.Add(new ItemInfo
                            {
                                Id = reader.GetInt32("Id"),
                                UserId = reader.GetInt32("UserId"),
                                UserLoginId = reader.GetString("UserLoginId"),
                                Title = reader.GetString("Title"),
                                ItemPhoto = reader.GetString("ItemPhoto"),
                                Description = reader.GetString("Description"),
                                Inclusion = reader.GetString("Inclusion"),
                                Price = reader.GetString("Price")
                            });
                        }
                    }
                }
            }



        }
       


    }

    public class ItemInfo()
    {
        public int Id { get; set; }
        public string UserLoginId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string ItemPhoto { get; set; }
      
        public string Description { get; set; }
        public string Inclusion { get; set; }
        public string Price { get; set; }
        public  List<string> ItemPhotos =>
          string.IsNullOrEmpty(ItemPhoto)
          ? new List<string>()
          : ItemPhoto.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
    }
    public class Users()
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string UserLoginId { get; set; }
        public string ProfilePhoto { get; set; }
        public string BackgroundPhoto { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }
        public string Gender { get; set; }
        public string Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Fandom { get; set; }
        public string EmergencyContact { get; set; }
    }
}
