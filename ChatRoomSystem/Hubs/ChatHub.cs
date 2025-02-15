using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;
using ChatRoomSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoomSystem.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _context;

        public ChatHub(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<JsonResult> SendMessage(Guid SenderId, string message)
        {
            var sender = _context.Users.FirstOrDefault(u => u.Id == SenderId);
            if (sender == null)
            {
                return new JsonResult(new { success = false, message = "User not found" });
            }

            string name = sender.UserName; 
           
            //var msg = new Message
            //{
            //    Content = message,
            //    SenderId = sender.Id,
            //    Timestamp = DateTime.Now
            //};

            //_context.Messages.Add(msg);
            //await _context.SaveChangesAsync(); 


            await Clients.All.SendAsync("ReceiveMessage",sender.Id, name, message);

            return new JsonResult(new { success = true, message = "Message sent successfully" });
        }

    }
}
