using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using MySql.Data.MySqlClient;

namespace RentalSite.Pages.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly string connStr = "server=localhost;database=rentalsite;user=root;password=aizenisqt18;port=3307";
        public string userId { get; set; }

        public ChatHub(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public override Task OnConnectedAsync()
        {
            var userId = _userManager.GetUserId(Context.User);
            Groups.AddToGroupAsync(Context.ConnectionId, userId);
            return base.OnConnectedAsync();
        }
         public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToGroup( string reciever, string message)
        {
            var SenderId = Context.UserIdentifier;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                   await conn.OpenAsync();

                    string query = @"INSERT INTO message 
                                     (SenderId, ReceiverId, Content, IsRead  ,Timestamp)
                                     VALUES
                                     (@SenderId, @ReceiverId, @Content,@IsRead ,@Timestamp)";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query,conn))
                    {
                        cmd.Parameters.AddWithValue("@SenderId", SenderId);
                        cmd.Parameters.AddWithValue("@ReceiverId", reciever);
                        cmd.Parameters.AddWithValue("@Content", message);
                        cmd.Parameters.AddWithValue("@IsRead", false);
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DB error" + ex.Message);
                }
            }

            // Send to receiver
            await Clients.Group(reciever).SendAsync("ReceiveMessage", SenderId, message);

            // Also send back to sender (so they see their message immediately)
            await Clients.Group(SenderId).SendAsync("ReceiveMessage", SenderId, message);
        }
    }
}
