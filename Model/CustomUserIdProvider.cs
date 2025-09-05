using Microsoft.AspNetCore.SignalR;

namespace RentalSite.Pages.Model
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("Id")?.Value;
        }
    }
}
