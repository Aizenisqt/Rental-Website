using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RentalSite.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        
        private readonly UserManager<IdentityUser> _userManager;
        public List<Item> Items { get; set; } = new List<Item>();
        public List<UserList> users { get; set; } = new List<UserList>();


        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            
        }

        public string id { get; set; }

        public async Task OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            id = user.Id;
            string connectionString = "server=localhost;database=rentalsite;user=root;password=aizenisqt18;port=3307";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string queryUser = "SELECT * FROM users WHERE UserLoginId = @Id";
                using (MySqlCommand cmd = new MySqlCommand(queryUser, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new UserList
                            {
                                Id = reader.GetInt32("id"),
                                UserName = reader.GetString("Username"),
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



                string query = "SELECT * FROM iteminfo WHERE UserLoginId <> @UserLoginId";
                using (MySqlCommand cmd = new MySqlCommand(query,conn))
                {
                    cmd.Parameters.AddWithValue("@UserLoginId", id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Items.Add(new Item
                        {
                            Id = reader.GetInt32("Id"),
                            UserId = reader.GetInt32("UserId"),
                            UserLoginId = reader.GetString("UserLoginId"),
                            Title = reader.GetString("Title"),
                            ItemPhoto = reader.GetString("ItemPhoto"),
                            ItemDescription = reader.GetString("Description"),
                            Inclusion = reader.GetString("Inclusion"),
                            price = reader.GetString("Price")
                        });
                    }
                }
            }
        }
    }

    public class Item
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserLoginId { get; set; }
        public string Title { get; set; }
        public string ItemPhoto { get; set; }
        public string ItemDescription { get; set; }
        public string Inclusion { get; set; }
        public string price { get; set; }

        public List<string> ItemPhotos =>
            string.IsNullOrEmpty(ItemPhoto)
             ? new List<string>() 
            : ItemPhoto.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public class UserList()
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
