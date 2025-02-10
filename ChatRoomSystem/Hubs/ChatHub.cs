using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;
using ChatRoomSystem.Data;
using ChatRoomSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoomSystem.Hubs
{
    public class ChatHub : Hub
    {
        private readonly MyProjectContext _context;

        public ChatHub(MyProjectContext context)
        {
            _context = context;
        }

        public async Task<JsonResult> SendMessage(string user, string message)
        {
            var sender = _context.Users.FirstOrDefault(u => u.UserName == user);
            if (sender == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }

            var msg = new Message
            {
                Content = message,
                SenderId = sender.Id,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", sender.UserName, message);

            return new JsonResult(new { success = true, message = "Message sent successfully" });
        }
    }
}
