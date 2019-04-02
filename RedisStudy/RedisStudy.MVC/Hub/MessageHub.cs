using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RedisStudy.Hub
{
    public class MessageHub: Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
