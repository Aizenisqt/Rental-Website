using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Reflection.PortableExecutable;
namespace RentalSite.Pages
{
    public class OtherUserProfileModel : PageModel
    {
        public List<User> users { get; set; } = new List<User>();
        public List<ItemInfos> Item { get; set; } = new List<ItemInfos>();
        private readonly UserManager<IdentityUser> _userManager;

        public OtherUserProfileModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            
        }
        public string UserLoginId { get; set; }
        public async Task OnGet(int id)
        {
          


            string connectionString = "server=localhost;database=rentalsite;user=root;password=aizenisqt18;port=3307";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM users WHERE id = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
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

                string querys = "SELECT * FROM iteminfo WHERE UserId = @id ";
                using (MySqlCommand cmd = new MySqlCommand(querys, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Item.Add(new ItemInfos
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

    public class ItemInfos()
    {
        public int Id { get; set; }
        public string UserLoginId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string ItemPhoto { get; set; }

        public string Description { get; set; }
        public string Inclusion { get; set; }
        public string Price { get; set; }
        public List<string> ItemPhotos =>
          string.IsNullOrEmpty(ItemPhoto)
          ? new List<string>()
          : ItemPhoto.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public class User
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

