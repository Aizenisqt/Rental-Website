using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MySql.Data.MySqlClient;
using RentalSite.Pages.DbContext;
using RentalSite.Pages.Model;
using System.Data;
using System.Security.Claims;


namespace RentalSite.Pages
{
    [Authorize]
    public class MessageModel : PageModel
    {
        [Microsoft.AspNetCore.Mvc.FromRoute]
        public string OtherUserId { get; set; }
        public string currentUser { get; set; }
        private readonly UserManager<IdentityUser> _userManager;
        private readonly string connStr = "server=localhost;database=rentalsite;user=root;password=aizenisqt18;port=3307";
        public List<UsersList> users { get; set; } = new List<UsersList>();
        public List<Messages> messages { get; set; } = new List<Messages>();
        public MessageModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

       public async Task OnGet(string otherUserId)
       {
            var user = await _userManager.GetUserAsync(User);
            string userId = user.Id;
            currentUser = user.Id;


            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string getUser = "SELECT * FROM users WHERE UserLoginId != @Me";

                    using (MySqlCommand cmd = new MySqlCommand(getUser, conn))
                    {
                        cmd.Parameters.AddWithValue("@Me", userId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new UsersList
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

                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }



        public async Task<JsonResult> OnGetMessages([FromQuery] string otherUserId)
        {
            var user = await _userManager.GetUserAsync(User);
            string userId = user.Id;

            var chatMessages = new List<Messages>();

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                await conn.OpenAsync();

                string query = @"
            SELECT Id, SenderId, ReceiverId, Content, IsRead,Timestamp
            FROM message
            WHERE (SenderId = @currentUser AND ReceiverId = @otherUser)
               OR (SenderId = @otherUser AND ReceiverId = @currentUser)
            ORDER BY Timestamp ASC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@currentUser", userId);
                    cmd.Parameters.AddWithValue("@otherUser", otherUserId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            chatMessages.Add(new Messages
                            {
                                Id = reader.GetInt32("Id"),
                                SenderId = reader.GetString("SenderId"),
                                ReceiverId = reader.GetString("ReceiverId"),
                                Content = reader.GetString("Content"),
                                IsRead = reader.GetBoolean("IsRead"),
                                Timestamp = reader.GetDateTime("Timestamp")
                            });
                        }
                    }
                }
      
                return new JsonResult(chatMessages);
            }

        }

        public IActionResult OnGetMarkAsRead(string otherUserId)
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            var userId = currentUser.Id;


            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"UPDATE message 
                               SET IsRead = TRUE 
                               WHERE SenderId = @other AND ReceiverId = @me AND IsRead = FALSE";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@me", userId);
                    cmd.Parameters.AddWithValue("@other", otherUserId);
                    cmd.ExecuteNonQuery();
                }
            }

            return new JsonResult(new { success = true });
        }

        public int GetUnreadCount(string otherUserId)
        {
            var currentUser = _userManager.GetUserAsync(User).Result; 
            var userid = currentUser.Id;  // ✅
            int count = 0;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"SELECT COUNT(*) 
                       FROM message 
                       WHERE SenderId  =  @Sender
                         AND ReceiverId  = @Reciever 
                         AND IsRead = FALSE";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Reciever", userid );
                    cmd.Parameters.AddWithValue("@Sender", otherUserId);

                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            return count;
        }


    }

    public class UsersList()
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
    public class Messages
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime Timestamp { get; set; }
    }
}