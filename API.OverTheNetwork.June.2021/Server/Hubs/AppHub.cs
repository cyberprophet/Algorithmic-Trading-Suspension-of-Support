using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

namespace ShareInvest.Hubs
{
    public class AppHub : Hub
    {
        public async Task SendMessage(string user, string message) => await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}