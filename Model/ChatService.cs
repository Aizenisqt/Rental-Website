using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using RentalSite.Pages.DbContext;
using System.Data;

namespace RentalSite.Pages.Model
{
    public class ChatService
    {


        // Save message to DB
        public async Task SaveMessage(string senderId, string receiverId, string message)
        {
            string connStr = "server=localhost;database=rentalsite;user=root;password=aizenisqt18;port=3307";
            using var connection = new MySqlConnection(connStr);
            await connection.OpenAsync();

            var sql = "INSERT INTO message (SenderId, ReceiverId, Content) VALUES (@sender, @receiver, @msg)";
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@sender", senderId);
            cmd.Parameters.AddWithValue("@receiver", receiverId);
            cmd.Parameters.AddWithValue("@msg", message);

            await cmd.ExecuteNonQueryAsync();
        }

        // Get message history between two users
        public async Task<List<Message>> GetMessages(string user1, string user2)
        {
            var messages = new List<Message>();
            string connStr = "server=localhost;database=rentalsite;user=root;password=aizenisqt18;port=3307";
            using var connection = new MySqlConnection(connStr);
            await connection.OpenAsync();

            var sql = @"
            SELECT * FROM message
            WHERE (SenderId = @user1 AND ReceiverId = @user2)
               OR (SenderId = @user2 AND ReceiverId = @user1)
            ORDER BY Timestamp";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@user1", user1);
            cmd.Parameters.AddWithValue("@user2", user2);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                messages.Add(new Message
                {
                    Id = reader.GetInt32("Id"),
                    SenderId = reader.GetString("SenderId"),
                    ReceiverId = reader.GetString("ReceiverId"),
                    Content = reader.GetString("Content"),
                    Timestamp = reader.GetDateTime("Timestamp") 
                });
            }

            return messages;
        }
    }
}
